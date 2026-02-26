using Microsoft.EntityFrameworkCore;
using Server_RPG.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. 서비스 등록
builder.Services.AddControllers();

// [스웨거 추가 1] API 문서화를 위한 서비스 등록
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 2. DB 연결
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

var app = builder.Build();

// [스웨거 추가 2] 스웨거 UI 미들웨어 활성화
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 4. HTTP 요청 파이프라인 설정
app.UseAuthorization();
app.MapControllers();

app.Run();