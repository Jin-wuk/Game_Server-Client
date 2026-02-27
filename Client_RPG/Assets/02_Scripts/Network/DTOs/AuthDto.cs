using System;

[Serializable]
public class LoginRequestDto // Command (클라 -> 서버 의도)
{
    public string email;
    public string password;
}

[Serializable]
public class LoginResponseDto // Result (서버 -> 클라 확정 결과)
{
    public string message;
    public int characterID;
    public string nickname;
    public int level;
    public long gold;
    public float posX;
    public float posY;
}