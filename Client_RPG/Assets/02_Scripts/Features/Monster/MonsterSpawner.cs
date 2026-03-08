using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class MonsterSpawner : MonoBehaviour
{
    [Header("에디터 시각화용 (로직은 서버에서 실행됨)")]
    public int spawnerID; // DB의 SpawnerID와 매칭
    public float spawnRadius = 3f;

    // 런타임 로직(InvokeRepeating 등)은 모두 삭제!

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 1, 0.3f); // 하늘색 반투명
        Gizmos.DrawSphere(transform.position, 0.5f);
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}
