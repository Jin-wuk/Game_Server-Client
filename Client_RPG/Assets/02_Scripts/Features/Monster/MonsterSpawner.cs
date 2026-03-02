using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class MonsterSpawner : MonoBehaviour
{
    [Header("스폰 설정")]
    public int monsterID = 1;
    public float spawnRadius = 3f;
    public int maxCount = 5;
    public float spawnInterval = 3f;

    private int currentCount = 0;

    private void Start()
    {
        InvokeRepeating(nameof(TrySpawn), 1f, spawnInterval);
    }

    private void TrySpawn()
    {
        // 1. 이미 최대 마릿수만큼 있다면 소환하지 않음
        if (currentCount >= maxCount) return;

        // 2. 소환 위치 결정 (스포너 중심 기준 원형 랜덤 좌표)
        Vector2 spawnOffset = Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPos = transform.position + (Vector3)spawnOffset;

        // 3.  MonsterManager에게 소환 요청
        GameObject monsterObj = MonsterManager.Instance.SpawnMonster(monsterID, spawnPos);

        if (monsterObj != null)
        {
            // 4. 몬스터의 지능(Controller) 설정
            MonsterController controller = monsterObj.GetComponent<MonsterController>();
            if (controller != null)
            {
                // 스포너의 위치와 반지름을 알려주어, 이 원 안에서만 움직이게 함
                controller.Init(transform.position, spawnRadius);
            }

            // 5. 몬스터 사망 시 카운트 관리를 위한 연결
            // 몬스터가 죽을 때 스포너에게 알려주도록 이벤트를 연결합니다.
            MonsterState state = monsterObj.GetComponent<MonsterState>();
            if (state != null)
            {
                // 몬스터 사망 이벤트 발생 시 currentCount를 줄임
                state.OnDeath += () => { currentCount--; };
            }

            currentCount++;
        }
    }

    // 유니티 에디터 화면에서 스폰 범위를 시각적으로 확인 (하늘색 원)
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}
