using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : MonoBehaviour
{
    public static MonsterManager Instance;

    [System.Serializable]
    public struct MonsterPrefabMapping
    {
        public int monsterID;
        public GameObject prefab;
    }

    public List<MonsterPrefabMapping> prefabMappings;
    private Dictionary<int, GameObject> monsterPrefabDict = new Dictionary<int, GameObject>();

    //  추가: 현재 화면에 소환된 실시간 몬스터들을 관리 (Key: InstanceID)
    private Dictionary<int, GameObject> liveMonsters = new Dictionary<int, GameObject>();

    private void Awake()
    {
        Instance = this;
        foreach (var mapping in prefabMappings)
        {
            monsterPrefabDict[mapping.monsterID] = mapping.prefab;
        }
    }

    private void Start()
    {
        // 서버에서 '실시간 몬스터 목록'을 가져옵니다.
        StartCoroutine(SyncLiveMonsters());
    }

    //  핵심: 서버 메모리의 LiveMonsters와 내 화면을 동기화
    private IEnumerator SyncLiveMonsters()
    {
        while (true) //  무한 루프로 주기적 동기화
        {
            string url = "http://localhost:5043/api/Monster/live-list?mapId=1";

            yield return StartCoroutine(NetworkManager.Instance.GetJson<List<MonsterInstanceDto>>(
                url,
                onSuccess: (monsterList) => {
                    // 1. 서버 목록에 있는 몬스터들 소환/업데이트
                    HashSet<int> serverInstanceIds = new HashSet<int>();

                    foreach (var m in monsterList)
                    {
                        serverInstanceIds.Add(m.instanceID);
                        SpawnOrUpdateMonster(m);
                    }

                    // 2.  중요: 서버 목록에는 없는데 내 화면(liveMonsters)에만 있다면? 
                    // 누군가 잡았거나 리젠 대기 중인 놈이니 지워줍니다.
                    List<int> toRemove = new List<int>();
                    foreach (var id in liveMonsters.Keys)
                    {
                        if (!serverInstanceIds.Contains(id))
                        {
                            toRemove.Add(id);
                        }
                    }

                    foreach (var id in toRemove)
                    {
                        RemoveMonster(id);
                    }
                },
                onError: (err) => Debug.LogError($"몬스터 동기화 실패: {err}")
            ));

            //  2~3초마다 한 번씩 서버에 물어봅니다. (너무 빠르면 서버 부하)
            yield return new WaitForSeconds(2.0f);
        }
    }

    private void SpawnOrUpdateMonster(MonsterInstanceDto data)
    {
        // 1. 이미 소환된 몬스터라면 위치/상태만 업데이트
        if (liveMonsters.ContainsKey(data.instanceID))
        {
            // (옵션) 위치 동기화가 필요하다면 여기서 처리
            return;
        }

        // 2. 처음 보는 InstanceID라면 새로 소환
        if (monsterPrefabDict.ContainsKey(data.monsterID))
        {
            Vector3 spawnPos = new Vector3(data.posX, data.posY, 0);
            GameObject obj = Instantiate(monsterPrefabDict[data.monsterID], spawnPos, Quaternion.identity);

            // 몬스터 컨트롤러에 서버 데이터(InstanceID 포함) 주입
            var controller = obj.GetComponent<MonsterController>();
            if (controller != null)
            {
                controller.Init(data);
            }

            // 관리 리스트에 추가
            liveMonsters.Add(data.instanceID, obj);
        }
    }

    //  몬스터 사망 시 호출 (딕셔너리에서 제거)
    public void RemoveMonster(int instanceId)
    {
        if (liveMonsters.ContainsKey(instanceId))
        {
            Destroy(liveMonsters[instanceId]);
            liveMonsters.Remove(instanceId);
        }
    }
}