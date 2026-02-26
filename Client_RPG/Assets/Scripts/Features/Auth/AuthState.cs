using System;

public class AuthState
{
    // Presenter가 구독할 상태 변화 이벤트
    public event Action<string> OnLoginSuccess;
    public event Action<string> OnLoginFailed;

    // 확정된 상태값 (외부에서는 읽기만 가능)
    public int CharacterID { get; private set; }
    public string Nickname { get; private set; }
    public int Level { get; private set; }

    // Handler가 호출하는 "상태 반영" 메서드
    public void ApplyLoginResult(LoginResponseDto result)
    {
        CharacterID = result.characterID;
        Nickname = result.nickname;
        Level = result.level;

        // 상태가 갱신되었음을 브로드캐스트
        OnLoginSuccess?.Invoke(Nickname);
    }

    public void ApplyLoginError(string errorMessage)
    {
        OnLoginFailed?.Invoke(errorMessage);
    }
}