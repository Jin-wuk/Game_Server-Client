using System;
using UnityEngine;

public class MonsterState : MonoBehaviour
{
    [Header("몬스터 정보")]
    public int monsterID;
    public string monsterName;
    public int level;

    [Header("체력 관리")]
    public float currentHP;
    public float maxHP;

    [Header("보상 정보")]
    public int goldReward;
    public int expReward;

    //  스포너에게 알리기 위한 이벤트
    public event Action OnDeath;

    // 1. MonsterManager로부터 데이터를 전달받아 초기화
    public void Setup(MonsterDto data)
    {
        monsterID = data.monsterID;
        monsterName = data.name;
        level = data.level;
        maxHP = data.maxHP;
        currentHP = maxHP;
        goldReward = data.goldReward;
        expReward = data.expReward;

        gameObject.name = $"{monsterName} (Lv.{level})";
    }

    // 2. 데미지를 입었을 때 호출되는 함수
    public void TakeDamage(float damage)
    {
        if (currentHP <= 0) return;

        currentHP -= damage;
        Debug.Log($"{monsterName}가 {damage}의 데미지를 입음! 남은 HP: {currentHP}");

        if (currentHP <= 0)
        {
            Die();
        }
    }

    // 3. 사망 처리
    private void Die()
    {
        //Debug.Log($"{monsterName} 사망!");

        //// Manager에 보상 처리 요청
        //MonsterManager.Instance.OnMonsterKilled(monsterID);

        ////  지점장(Spawner)에게 사망 알림
        //OnDeath?.Invoke();

        //// 오브젝트 제거 (나중에 오브젝트 풀링을 쓰면 비활성화로 변경)
        //Destroy(gameObject);
    }
}