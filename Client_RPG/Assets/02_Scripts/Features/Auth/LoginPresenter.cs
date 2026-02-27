using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoginPresenter : MonoBehaviour
{
    [Header("UI 설정")]
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public Button loginButton;
    public TextMeshProUGUI statusText;

    [Header("의존성")]
    public AuthHandler authHandler;

    private AuthState authState;

    private void Awake()
    {
        authState = new AuthState();
        authHandler.State = authState;

        authState.OnLoginSuccess += OnLoginSuccess;
        authState.OnLoginFailed += OnLoginFailed;

        loginButton.onClick.AddListener(OnLoginButtonClicked);
    }

    private void OnDestroy()
    {
        if (authState != null)
        {
            authState.OnLoginSuccess -= OnLoginSuccess;
            authState.OnLoginFailed -= OnLoginFailed;
        }
    }

    private void OnLoginButtonClicked()
    {
        statusText.text = "서버와 통신 중...";
        statusText.color = Color.yellow;
        authHandler.SendLoginCommand(emailInput.text, passwordInput.text);
    }

    private void OnLoginSuccess(LoginResponseDto responseData)
    {
        statusText.text = $"환영합니다, {responseData.nickname}님!";
        statusText.color = Color.green;

        GameManager.Instance.SetPlayerData(responseData);

        StartCoroutine(LoadMainSceneDelay());
    }

    private void OnLoginFailed(string errorMsg)
    {
        statusText.text = errorMsg;
        statusText.color = Color.red;
    }
    
    private IEnumerator LoadMainSceneDelay()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Main");
        asyncLoad.allowSceneActivation = false;

        yield return new WaitForSeconds(1.0f);

        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }

        asyncLoad.allowSceneActivation = true;
    }
}