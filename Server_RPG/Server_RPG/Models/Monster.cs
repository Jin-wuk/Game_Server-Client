using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server_RPG.Models
{
    [Table("monster")]
    public class Monster
    {
        [Key]
        public int MonsterID { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Level { get; set; }
        public int MaxHP { get; set; }
        public int CurrentHP { get; set; }
        public int AttackPower { get; set; }

        // 보상 관련
        public int ExpReward { get; set; }
        public int GoldReward { get; set; }
    }
}