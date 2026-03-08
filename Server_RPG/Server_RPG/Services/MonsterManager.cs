using System.Collections.Concurrent;
using Server_RPG.Models;

namespace Server_RPG.Services
{
    public static class MonsterManager
    {
        // 실시간 몬스터 목록 (Thread-safe 딕셔너리)
        public static ConcurrentDictionary<int, MonsterInstance> LiveMonsters = new ConcurrentDictionary<int, MonsterInstance>();

        private static int _instanceIdCounter = 1;

        public static void Spawn(Monster template, MonsterSpawner spawner)
        {
            // 스폰 범위 내 랜덤 좌표 계산
            float randomX = spawner.PosX + (Random.Shared.NextSingle() * spawner.SpawnRange * 2 - spawner.SpawnRange);

            var newInstance = new MonsterInstance
            {
                InstanceID = _instanceIdCounter++,
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
    }
}