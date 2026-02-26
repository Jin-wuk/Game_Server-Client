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

        public Account Account { get; set; } = null!;
    }
}
