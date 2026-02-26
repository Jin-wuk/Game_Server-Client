using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LoginPresenter : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public Button loginButton;
    public TextMeshProUGUI statusText;

    [Header("Dependencies")]
    public AuthHandler authHandler;

    private AuthState authState;

    private void Awake()
    {
        // 1. State 생성 및 Handler에 주입
        authState = new AuthState();
        authHandler.State = authState;

        // 2. State의 "상태 변화 이벤트" 구독
        authState.OnLoginSuccess += OnLoginSuccess;
        authState.OnLoginFailed += OnLoginFailed;

        // 3. UI 버튼 클릭 이벤트 연결
        loginButton.onClick.AddListener(OnLoginButtonClicked);
    }

    private void OnDestroy()
    {
        // 메모리 누수 방지를 위해 이벤트 구독 해제 필수
        if (authState != null)
        {
            authState.OnLoginSuccess -= OnLoginSuccess;
            authState.OnLoginFailed -= OnLoginFailed;
        }
    }

    // --- View -> Command ---
    private void OnLoginButtonClicked()
    {
        statusText.text = "서버와 통신 중...";
        statusText.color = Color.yellow;
        // 유저 의도(Command)를 Handler로 전송
        authHandler.SendLoginCommand(emailInput.text, passwordInput.text);
    }

    // --- State -> View ---
    private void OnLoginSuccess(string nickname)
    {
        statusText.text = $"환영합니다, {nickname}님!";
        statusText.color = Color.green;
        // TODO: SceneManager.LoadScene("MainGameScene");
    }

    private void OnLoginFailed(string errorMsg)
    {
        statusText.text = errorMsg;
        statusText.color = Color.red;
    }
}