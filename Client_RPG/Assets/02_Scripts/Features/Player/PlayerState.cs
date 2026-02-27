using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerState : MonoBehaviour
{
    // UI가 듣고 화면을 갱신할 이벤트들
    public event Action<int, int> OnHpChanged;  //현재HP, 최대HP
    public event Action<long> OnGoldChanged;    //현재 골드

    // 인게임에서 실시간으로 변하는 상태값들
    public int MaxHP { get; private set; }
    public int CurrentHP { get; private set; }
    public int AttackPower { get; private set; }
    public long Gold {  get; private set; }

    private readonly string syncUrl = "http://localhost:5043/api/Player/sync-position";

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            MaxHP = GameManager.Instance.MaxHP;
            CurrentHP = GameManager.Instance.CurrentHP;
            AttackPower = GameManager.Instance.AttackPower;
            Gold = GameManager.Instance.Gold;

            OnHpChanged?.Invoke(MaxHP, CurrentHP);
            OnGoldChanged?.Invoke(Gold);

            //  씬 시작 시: 서버에서 가져온 내 원래 위치로 캐릭터를 순간이동!
            transform.position = new Vector3(GameManager.Instance.PosX, GameManager.Instance.PosY, 0f);

            //  5초 주기 자동 저장 코루틴 시작!
            StartCoroutine(AutoSaveRoutine());
        }
    }

    public void TakeDamage(int damage)
    {
        CurrentHP -= damage;
        if (CurrentHP < 0) CurrentHP = 0;

        OnHpChanged?.Invoke(CurrentHP, MaxHP);

        if (CurrentHP == 0)
        {
            Die();
        }

    }
    // 골드를 획득했을 때 (나중에 몬스터 잡고 쓸 예정)
    public void AddGold(long amount)
    {
        Gold += amount;
        OnGoldChanged?.Invoke(Gold);
    }

    private void Die()
    {
        Debug.Log("플레이어 사망! 체력이 0이 되었습니다.");
        // 나중에 여기에 부활 로직이나 마을 귀환 처리
    }

    private IEnumerator AutoSaveRoutine()
    {
        while (true)
        {
            // 5초마다 대기
            yield return new WaitForSeconds(5.0f);

            // 현재 내 유니티 상의 위치값 가져오기
            var syncDto = new PositionSyncDto
            {
                characterID = GameManager.Instance.CharacterID,
                posX = transform.position.x,
                posY = transform.position.y
            };
            
            string jsonBody = JsonUtility.ToJson(syncDto);

            // NetworkManager을 통해 서버로 전송
            StartCoroutine(NetworkManager.Instance.PostJson(
                syncUrl,
                jsonBody,
                onSuccess: (response) =>
                {
                    Debug.Log($"[자동 저장] 서버에 위치 기록됨: X({syncDto.posX}), Y({syncDto.posY})");
                },
                onError: (error) =>
                {
                    Debug.LogWarning($"[자동 저장 실패] {error}");
                }
            ));

        }
    }
}
