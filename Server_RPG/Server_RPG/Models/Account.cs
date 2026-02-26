namespace Server_RPG.Models
{
    public class Account
    {
        public int AcoountID { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        // 캐릭터가 아직 생성되지 않았을 수도 있으니 ?(Nullable) 처리
        public Character? Character { get; set; }
    }
}
