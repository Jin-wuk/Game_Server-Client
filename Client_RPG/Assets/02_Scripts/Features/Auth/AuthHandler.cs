using UnityEngine;

public class AuthHandler : MonoBehaviour
{
    private readonly string serverUrl = "http://localhost:5043/api/Auth/login";
    public AuthState State { get; set; }

    public void SendLoginCommand(string email, string password)
    {
        var requestDto = new LoginRequestDto { email = email, password = password };
        string jsonBody = JsonUtility.ToJson(requestDto);

        // NetworkManager(우체국)에게 배달(POST) 요청!
        StartCoroutine(NetworkManager.Instance.PostJson(
            serverUrl,
            jsonBody,
            onSuccess: (jsonResponse) =>
            {
                // 답장(JSON)이 오면 DTO로 풀어서 State에 넘김
                var response = JsonUtility.FromJson<LoginResponseDto>(jsonResponse);
                State.ApplyLoginResult(response);
            },
            onError: (errorMsg) =>
            {
                State.ApplyLoginError("로그인 실패: " + errorMsg);
            }
        ));
    }
}