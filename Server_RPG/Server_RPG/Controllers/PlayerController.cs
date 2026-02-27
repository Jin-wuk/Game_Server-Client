using Microsoft.AspNetCore.Mvc;
using Server_RPG.Data;
using Server_RPG.DTOs;
using Server_RPG.Models;
using System.Linq;

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
    }
}