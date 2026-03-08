namespace Server_RPG.Configs
{
    public static class GameConfig
    {
        // 공통 스텟
       
        public const float BASE_MOVE_SPEED = 2.0f;  //이동속도

        public const float BASE_ATTACK_RANGE = 1.5f;    //공격 사거리

        public const float RANGE_MARGIN = 0.5f; //네트워크 지연을 고려한 허용 오차
        
        public const float SYNC_INTERVAL = 5.0f;     // 클라의 DB 저장 주기 (5초)
    }
}
