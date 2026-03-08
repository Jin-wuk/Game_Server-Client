using System.Collections.Concurrent;
using Server_RPG.Models;

namespace Server_RPG.Services
{
    public static class MonsterManager
    {
        // 실시간 몬스터 목록
        public static ConcurrentDictionary<int, MonsterInstance> LiveMonsters = new();

        // 리젠 예약 대기열
        public static ConcurrentQueue<RegenInfo> RegenQueue = new();

        private static int _instanceIdCounter = 1;

        public static void Spawn(Monster template, MonsterSpawner spawner)
        {
            int newId = Interlocked.Increment(ref _instanceIdCounter);

            var newInstance = new MonsterInstance
            {
                InstanceID = newId,
                MonsterID = template.MonsterID,
                SpawnerID = spawner.SpawnerID,
                MapID = spawner.MapID,
                Name = template.Name,
                MaxHP = template.MaxHP,
                CurrentHP = template.MaxHP,
                SpawnX = spawner.PosX,
                SpawnY = spawner.PosY,
                SpawnRange = spawner.SpawnRange,
                PosX = spawner.PosX,
                PosY = spawner.PosY
            };

            LiveMonsters.TryAdd(newInstance.InstanceID, newInstance);
        }

        public class RegenInfo
        {
            public Monster MonsterTemplate { get; set; } = null!;
            public MonsterSpawner Spawner { get; set; } = null!;
            public DateTime RespawnTime { get; set; }
        }
    }
}