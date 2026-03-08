using UnityEngine;

public class AuthHandler : MonoBehaviour
{
    private readonly string serverUrl = "http://localhost:5043/api/Auth/login";
    public AuthState State { get; set; }

    public void SendLoginCommand(string email, string password)
    {
        var requestDto = new LoginRequestDto { email = email, password = password };
        string jsonBody = JsonUtility.ToJson(requestDto);

        // [수정] <LoginResponseDto> 형식을 명시해줍니다.
        // 이제 onSuccess의 매개변수는 json(string)이 아니라 실제 객체(response)가 됩니다.
        StartCoroutine(NetworkManager.Instance.PostJson<LoginResponseDto>(
            serverUrl,
            jsonBody,
            onSuccess: (response) =>
            {
                // 1. [중요] 나중에 어디서든 쓰기 위해 인스턴스에 저장 (아까 약속한 방식)
                LoginResponseDto.Instance = response;

                // 2. State에 결과 전달
                State.ApplyLoginResult(response);

                Debug.Log($"{response.nickname}님 로그인 성공!");
            },
            onError: (errorMsg) =>
            {
                State.ApplyLoginError("로그인 실패: " + errorMsg);
            }
        ));
    }
}