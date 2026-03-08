using System;

[Serializable]
public class AttackRequestDto
{
    public int characterID;
    public int instanceID;
    public float playerPosX;
    public float playerPosY;
}

[Serializable]
public class AttackResponseDto
{
    public bool success;
    public int damage;
    public int remainingHp;
    public bool isDead;
    public int currentGold;
}