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
    private Dictionary<int, MonsterDto> monsterDataDict = new Dictionary<int, MonsterDto>();

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
        StartCoroutine(FetchMonsterData());
    }

    private IEnumerator FetchMonsterData()
    {
        string url = "http://localhost:5043/api/Monster";

        yield return StartCoroutine(NetworkManager.Instance.GetJson<List<MonsterDto>>(
            url,
            onSuccess: (monsterList) => {
                foreach (var m in monsterList)
                {
                    monsterDataDict[m.monsterID] = m;
                    Debug.Log($"[MonsterManager] 몬스터 데이터 로드: {m.name}");
                }
            },
            onError: (err) => Debug.LogError($"몬스터 로드 실패: {err}")
        ));
    }

    // 몬스터 소환 함수
    public GameObject SpawnMonster(int id, Vector3 position)
    {
        if (monsterPrefabDict.ContainsKey(id))
        {
            GameObject obj = Instantiate(monsterPrefabDict[id], position, Quaternion.identity);

            //  추가: 생성된 몬스터에게 서버 데이터를 주입!
            MonsterState state = obj.GetComponent<MonsterState>();
            if (state != null && monsterDataDict.ContainsKey(id))
            {
                state.Setup(monsterDataDict[id]);
            }

            return obj;
        }
        return null;
    }
}

