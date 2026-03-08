using Microsoft.EntityFrameworkCore;
using Server_RPG.Data;
using Server_RPG.Services;

var builder = WebApplication.CreateBuilder(args);

// --- 1. 서비스 등록 영역 (DI Container) ---
builder.Services.AddControllers();

// [스웨거 설정]
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// [DB 연결 설정]
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// [핵심 수정] SpawnService 등록은 반드시 Build() 이전에 해야 합니다!
builder.Services.AddHostedService<SpawnService>();

// ------------------------------------------

var app = builder.Build(); // 여기서 앱이 고정(Build)됩니다.

// --- 2. HTTP 요청 파이프라인 설정 (Middleware) ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

app.Run();