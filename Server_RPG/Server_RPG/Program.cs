using Microsoft.EntityFrameworkCore;
using Server_RPG.Data; // AppDbContext가 있는 네임스페이스 확인 필요

var builder = WebApplication.CreateBuilder(args);

// 1. 서비스 등록
builder.Services.AddControllers();

// 2. DB 연결 문자열 가져오기 (User Secrets 또는 appsettings에서 조회)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 3. MySQL DbContext 등록
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

var app = builder.Build();

// 4. HTTP 요청 파이프라인 설정
app.UseAuthorization();
app.MapControllers();

app.Run();