using Microsoft.EntityFrameworkCore;
using Server_RPG.Data;

namespace Server_RPG.Services
{
    public class SpawnService : IHostedService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public SpawnService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                // 1. DB에서 모든 스포너와 연결된 몬스터 도감 정보를 가져옵니다.
                var spawners = await context.MonsterSpawners
                    .Include(s => s.MonsterTemplate)
                    .ToListAsync();

                foreach (var spawner in spawners)
                {
                    if (spawner.MonsterTemplate == null) continue;

                    // 2. 설정된 MaxCount만큼 몬스터를 생성합니다.
                    for (int i = 0; i < spawner.MaxCount; i++)
                    {
                        MonsterManager.Spawn(spawner.MonsterTemplate, spawner);
                    }
                }
            }
            await Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}