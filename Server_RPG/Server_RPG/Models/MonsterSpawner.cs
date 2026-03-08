using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server_RPG.Models
{
    [Table("monster_spawner")]
    public class MonsterSpawner
    {
        [Key]
        public int SpawnerID { get; set; }

        public int MonsterID { get; set; }
        public int MapID { get; set; }

        public float PosX { get; set; }
        public float PosY { get; set; }
        public float SpawnRange { get; set; }

        public int MaxCount { get; set; }
        public float RespawnTime { get; set; }

        // Navigation Property: DB 조인 없이 Monster 정보를 바로 가져올 때 사용
        [ForeignKey("MonsterID")]
        public virtual Monster? MonsterTemplate { get; set; }
    }
}
