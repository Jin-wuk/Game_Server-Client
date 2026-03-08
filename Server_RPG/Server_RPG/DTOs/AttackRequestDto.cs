namespace Server_RPG.DTOs
{
    public class AttackRequestDto
    {
        public int CharacterID { get; set; }
        public int InstanceID { get; set; }

        public float PlayerPosX { get; set; }
        public float PlayerPosY { get; set; }
        public float MonsterPosX { get; set; }
        public float MonsterPosY { get; set; }
    }
}
