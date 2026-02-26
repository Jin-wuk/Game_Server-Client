using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server_RPG.Data;
using Server_RPG.DTOs;
using BCrypt.Net;

namespace Server_RPG.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            // 1. DB에서 이메일로 계정 찾기 (연결된 1개의 캐릭터 정보도 함께 불러옴)
            var account = await _context.Accounts
                .Include(a => a.Character) // 1:1 관계의 캐릭터 로드
                .FirstOrDefaultAsync(a => a.Email == request.Email);

            // 계정이 없으면 401 에러 반환
            if (account == null)
            {
                return Unauthorized(new { Message = "계정을 찾을 수 없습니다." });
            }

            // 2. 비밀번호 검증 (설치한 BCrypt 사용)
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, account.PasswordHash);

            if (!isPasswordValid)
            {
                return Unauthorized(new { Message = "비밀번호가 일치하지 않습니다." });
            }

            // 3. 캐릭터 존재 여부 확인
            if (account.Character == null)
            {
                // 로그인(계정)은 성공했지만, 아직 캐릭터를 안 만든 경우
                return Ok(new { Message = "캐릭터가 존재하지 않습니다.", NeedsCharacterCreation = true });
            }

            // 4. 로그인 최종 성공 - 유니티에 넘겨줄 DTO 포장
            var responseDto = new LoginResponseDto
            {
                Message = "로그인 성공!",
                CharacterID = account.Character.CharacterID,
                Nickname = account.Character.Nickname,
                Level = account.Character.Level,
                Gold = account.Character.Gold,
                PosX = account.Character.PosX,
                PosY = account.Character.PosY
            };

            return Ok(responseDto);
        }
    }
}