using Server_RPG.Configs;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server_RPG.Models
{
    [Table("characterinfo")]
    public class Character
    {
        [Key]
        public int CharacterID { get; set; }
        public int AccountID { get; set; }
        public string Nickname { get; set; } = string.Empty;
        public int Level { get; set; }
        public long Experience { get; set; }
        public long Gold { get; set; }
        public DateTime? LastLogoutAt { get; set; } // NULL 허용
        public float PosX { get; set; }
        public float PosY { get; set; }

        // 전투 스탯
        public int MaxHP { get; set; }
        public int CurrentHP { get; set; }
        public int AttackPower { get; set; }
        public float AttackSpeed { get; set; } = 1.0f;

        public float MoveSpeed => GameConfig.BASE_MOVE_SPEED;
        public float AttackRange => GameConfig.BASE_ATTACK_RANGE;


        public Account Account { get; set; } = null!;
    }
}
