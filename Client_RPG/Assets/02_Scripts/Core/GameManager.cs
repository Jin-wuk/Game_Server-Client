using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }


    public int CharacterID { get; private set; }
    public string Nickname { get; private set; }
    public int Level { get; private set; }
    public long Gold { get; private set; }
    public float PosX { get; private set; }
    public float PosY { get; private set; }


    public int MaxHP { get; private set; }
    public int CurrentHP { get; private set; }
    public int AttackPower { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    // LoginResponseDto를 통째로 받아서 한 번에 세팅합니다.
    public void SetPlayerData(LoginResponseDto data)
    {
        CharacterID = data.characterID;
        Nickname = data.nickname;
        Level = data.level;
        Gold = data.gold;
        PosX = data.posX;
        PosY = data.posY;
        MaxHP = data.maxHP;
        CurrentHP = data.currentHP;
        AttackPower = data.attackPower;

        Debug.Log($"[GameManager] 유저 데이터 세팅 완료! (닉네임: {Nickname}, 레벨: Lv.{Level}, 체력: {MaxHP}/{CurrentHP}, 공격력: {AttackPower}, 골드: {Gold}G)");
    }
}