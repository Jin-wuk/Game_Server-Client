using Microsoft.AspNetCore.Mvc;
using Server_RPG.Configs;
using Server_RPG.Data;
using Server_RPG.DTOs;
using Server_RPG.Models;
using System.Linq;
using Server_RPG.Services;

namespace Server_RPG.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class PlayerController : ControllerBase
    {
        private readonly AppDbContext _context;

        // 1. 생성자: 의존성 주입(DI)을 통해 DB 컨텍스트를 받아옵니다.
        public PlayerController(AppDbContext context)
        {
            _context = context;
        }

        // 2. 위치 동기화 API
        // 유니티에서 NetworkManager.Instance.PostJson(".../api/Player/sync-position", ...) 으로 호출하게 됩니다.
        [HttpPost("sync-position")]
        public IActionResult SyncPosition([FromBody] PositionSyncDto request)
        {
            // DB에서 요청 들어온 캐릭터 ID와 일치하는 캐릭터를 찾습니다.
            var character = _context.Characters.FirstOrDefault(c => c.CharacterID == request.CharacterID);

            if (character == null)
            {
                return NotFound("캐릭터를 찾을 수 없습니다.");
            }

            //  5초마다 들고 온 새로운 좌표로 기존 좌표를 덮어씌웁니다.
            character.PosX = request.PosX;
            character.PosY = request.PosY;

            // DB에 변경된 좌표를 최종 저장(Commit)합니다.
            _context.SaveChanges();

            // 유니티 쪽에 잘 저장되었다고 200 OK 사인을 보냅니다.
            return Ok(new { message = "위치 저장 완료" });
        }

        // 플레이어 공격 API
        [HttpPost("attack")]
        public IActionResult Attack([FromBody] AttackRequestDto request)
        {
            // 1. [변경] DB가 아니라 '메모리(MonsterManager)'에서 실제 몬스터 인스턴스를 찾습니다.
            if (!MonsterManager.LiveMonsters.TryGetValue(request.InstanceID, out var monster))
            {
                return NotFound("존재하지 않거나 이미 죽은 몬스터입니다.");
            }

            // 2. 캐릭터 정보는 영구 데이터이므로 DB에서 가져옵니다.
            var character = _context.Characters.FirstOrDefault(c => c.CharacterID == request.CharacterID);
            if (character == null) return NotFound("캐릭터 오류");

            // 3. [보안 1] 플레이어 이동 개연성 체크 (기존 로직 유지)
            float distFromLastSync = (float)Math.Sqrt(
                Math.Pow(request.PlayerPosX - character.PosX, 2) +
                Math.Pow(request.PlayerPosY - character.PosY, 2)
            );
            float maxPossibleMove = GameConfig.BASE_MOVE_SPEED * (GameConfig.SYNC_INTERVAL + 1.0f);
            if (distFromLastSync > maxPossibleMove) return BadRequest("핵 의심: 비정상적 이동");

            // 몬스터가 자기 집(SpawnX, Y)에서 너무 멀리 벗어났는지 확인합니다.
            float distFromSpawn = (float)Math.Sqrt(
                Math.Pow(request.MonsterPosX - monster.SpawnX, 2) +
                Math.Pow(request.MonsterPosY - monster.SpawnY, 2)
            );

            // 허용 범위(SpawnRange)에 통신 오차(Margin)를 조금 더해줍니다.
            if (distFromSpawn > monster.SpawnRange + 1.0f)
            {
                return BadRequest("핵 의심: 몬스터가 이동 범위를 벗어났습니다.");
            }

            // 5. [사거리 체크] 
            // 이제 서버는 클라이언트가 보고한 몬스터 위치(request.MonsterPosX)를 일단 믿어줍니다.
            // (위 4번에서 이미 집 근처인걸 확인했으니까요!)
            float attackDist = (float)Math.Sqrt(
                Math.Pow(request.PlayerPosX - request.MonsterPosX, 2) +
                Math.Pow(request.PlayerPosY - request.MonsterPosY, 2)
            );

            if (attackDist > GameConfig.BASE_ATTACK_RANGE + GameConfig.RANGE_MARGIN)
            {
                return BadRequest("사거리가 너무 멉니다.");
            }

            //  [위치 동기화] 공격이 성공했으므로, 서버 메모리의 몬스터 좌표를 최신화해줍니다.
            monster.PosX = request.MonsterPosX;
            monster.PosY = request.MonsterPosY;

            // 5. [전투 처리] 메모리의 몬스터 체력을 깎습니다. (DB 저장 X)
            int damage = 10;
            monster.CurrentHP -= damage;

            bool isDead = false;
            if (monster.CurrentHP <= 0)
            {
                monster.CurrentHP = 0;
                isDead = true;

                // 6. [사망 처리] 골드는 소중하니까 이때만 DB에 기록합니다.
                character.Gold += 100; // 실제론 monster.GoldReward 사용
                _context.SaveChanges();

                // 7. 메모리에서 몬스터 제거 (나중에 리젠 시스템이 다시 살려낼 겁니다)
                MonsterManager.LiveMonsters.TryRemove(request.InstanceID, out _);
            }

            return Ok(new
            {
                success = true,
                damage = damage,
                remainingHp = monster.CurrentHP,
                isDead = isDead,
                currentGold = character.Gold
            });
        }
    }
}