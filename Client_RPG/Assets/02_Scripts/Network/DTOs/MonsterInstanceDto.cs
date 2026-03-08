using System.Collections.Generic;

[System.Serializable]
public class MonsterInstanceDto
{
    // 1. 실시간 식별 정보
    public int instanceID; // 서버 메모리에서 부여한 고유 번호 (공격 시 필수!)
    public int monsterID;  // 도감 번호 (어떤 프리팹을 소환할지 결정)

    // 2. 실시간 상태 정보
    public string name;
    public int currentHP;
    public int maxHP;

    // 3. 실시간 위치 정보 (서버가 결정한 좌표)
    public float posX;
    public float posY;
}

// 여러 마리를 한꺼번에 받기 위한 리스트 래퍼
[System.Serializable]
public class MonsterListDto
{
    public List<MonsterInstanceDto> monsters;
}