using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server_RPG.Data;
using Server_RPG.Models;
using Server_RPG.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Server_RPG.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MonsterController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MonsterController(AppDbContext context)
        {
            _context = context;
        }

        // 몬스터 전체 도감 정보 가져오기
        // GET: api/Monster
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Monster>>> GetMonsters()
        {
            return await _context.Monsters.ToListAsync();
        }

        // 특정 ID의 몬스터 정보 가져오기
        // GET: api/Monster/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Monster>> GetMonster(int id)
        {
            var monster = await _context.Monsters.FindAsync(id);

            if (monster == null)
            {
                return NotFound("해당 몬스터 정보를 찾을 수 없습니다.");
            }

            return monster;
        }

        // 유니티에서 api/Monster/live-list?mapId=1 로 호출합니다.
        [HttpGet("live-list")]
        public IActionResult GetLiveMonsters(int mapId)
        {
            // 1. 메모리(MonsterManager)에 있는 실시간 몬스터들을 가져옵니다.
            // 2. 요청받은 MapID에 해당하는 몬스터만 필터링합니다.
            var liveList = MonsterManager.LiveMonsters.Values
                .Where(m => m.MapID == mapId)
                .ToList();

            if (liveList == null || liveList.Count == 0)
            {
                // 몬스터가 없어도 빈 리스트([])를 주는 것이 클라 입장에서 편합니다.
                return Ok(new List<object>());
            }

            return Ok(liveList);
        }
    }
}