using Microsoft.Extensions.Hosting;

namespace Server_RPG.Services
{
    public class RegenService : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken); // 1초마다 체크

                while (MonsterManager.RegenQueue.TryPeek(out var info))
                {
                    // 시간이 되었는지 확인
                    if (DateTime.Now >= info.RespawnTime)
                    {
                        if (MonsterManager.RegenQueue.TryDequeue(out var ready))
                        {
                            MonsterManager.Spawn(ready.MonsterTemplate, ready.Spawner);
                            Console.WriteLine($"[Regen] {ready.MonsterTemplate.Name} 부활 완료!");
                        }
                    }
                    else break; // 맨 앞 녀석이 아니면 루프 탈출
                }
            }
        }
    }
}