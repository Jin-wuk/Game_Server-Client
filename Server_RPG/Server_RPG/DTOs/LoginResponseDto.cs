namespace Server_RPG.DTOs
{
    public class LoginResponseDto
    {
        public string Message { get; set; } = string.Empty;

        //캐릭터 정보
        public int CharacterID { get; set; }
        public string Nickname { get; set; } = string.Empty;
        public int Level { get; set; }
        public long Gold { get; set; }
        public float PosX { get; set; }
        public float PosY { get; set; }

        public int MaxHP { get; set; }
        public int CurrentHP { get; set; }
        public int AttackPower { get; set; }
    }
}
