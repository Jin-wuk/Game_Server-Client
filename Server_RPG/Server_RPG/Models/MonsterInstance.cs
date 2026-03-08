namespace Server_RPG.Models
{
    public class MonsterInstance
    {
        // 서버 메모리에서만 사용하는 고유 번호
        public int InstanceID { get; set; }

        // 어떤 종류의 몬스터인지 (DB의 MonsterID)
        public int MonsterID { get; set; }
        public int MapID { get; set; }
        public int SpawnerID { get; set; }    // 어느 스포너 소속인지 (리젠용)
        public string Name { get; set; } = string.Empty;

        // 실시간 상태 값
        public int CurrentHP { get; set; }
        public int MaxHP { get; set; }

        // 실시간 좌표 (서버가 알고 있는 진짜 위치)
        public float PosX { get; set; }
        public float PosY { get; set; }

        // 스폰 기준 좌표 (이동 범위 제한용)
        public float SpawnX { get; set; }
        public float SpawnY { get; set; }
        public float SpawnRange { get; set; }

        public int GoldReward { get; set; }
        public int ExpReward { get; set; }
    }
}