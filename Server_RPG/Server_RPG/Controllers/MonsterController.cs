using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server_RPG.Data;
using Server_RPG.Models;
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
    }
}