using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerPresenter : MonoBehaviour
{
    [Header("플레이어 UI")]
    public Slider hpSlider; // 체력바 슬라이더
    public TextMeshProUGUI hpText; // "100 / 100" 텍스트
    public TextMeshProUGUI goldText; // "100 G" 텍스트

    public Button testHitButton; //  데미지 테스트용 버튼

    [Header("Dependencies")]
    public PlayerState playerState;

    private void Awake()
    {
        // State의 상태 변화 이벤트 구독 (귀 기울이기 시작)
        playerState.OnHpChanged += UpdateHpUI;
        playerState.OnGoldChanged += UpdateGoldUI;

        //  버튼을 누르면 PlayerState의 TakeDamage(10)을 실행하도록 연결!
        if (testHitButton != null)
        {
            testHitButton.onClick.AddListener(() => playerState.TakeDamage(10));
        }
    }

    private void OnDestroy()
    {
        // 오브젝트가 파괴될 때 구독 해제 (메모리 누수 방지)
        if (playerState != null)
        {
            playerState.OnHpChanged -= UpdateHpUI;
            playerState.OnGoldChanged -= UpdateGoldUI;
        }
    }

    // State가 "체력 변했어!" 하고 부르면 화면을 다시 그립니다.
    private void UpdateHpUI(int currentHp, int maxHp)
    {
        if (hpSlider != null)
        {
            hpSlider.maxValue = maxHp;
            hpSlider.value = currentHp;
        }

        if (hpText != null)
        {
            hpText.text = $"{currentHp} / {maxHp}";
        }
    }

    // State가 "골드 변했어!" 하고 부르면 화면을 다시 그립니다.
    private void UpdateGoldUI(long currentGold)
    {
        if (goldText != null)
        {
            goldText.text = $"{currentGold} G";
        }
    }
}