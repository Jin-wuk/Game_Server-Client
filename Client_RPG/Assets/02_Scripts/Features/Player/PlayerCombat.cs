using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("설정")]
    public LayerMask monsterLayer;
    public float attackRange = 1.5f;
    public float moveSpeed = 5f;

    [Header("참조")]
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        // 'Z' 키나 마우스 왼쪽 버튼으로 공격 시도
        if (Input.GetKeyDown(KeyCode.Z) || Input.GetMouseButtonDown(0))
        {
            Attack();
        }
    }

    private void Attack()
    {
        // 1. 공격 애니메이션 재생 (클라이언트 측 예측: 일단 휘두름)
        if (anim != null) anim.SetTrigger("doAttack");

        // 2. 공격 범위 내의 몬스터 탐색
        // 플레이어 정면 방향으로 원형 범위를 체크합니다.
        Collider2D[] hitMonsters = Physics2D.OverlapCircleAll(transform.position, attackRange, monsterLayer);

        foreach (Collider2D monster in hitMonsters)
        {
            // 3. 몬스터가 가진 MonsterController에서 서버용 InstanceID를 추출합니다.
            var monsterCtrl = monster.GetComponent<MonsterController>();
            if (monsterCtrl != null)
            {
                // 서버로 공격 패킷 전송
                SendAttackRequestToServer(monsterCtrl.instanceID);

                // 일단 한 마리만 때리는 예시 (범위 공격이면 break 삭제)
                break;
            }
        }
    }

    private void SendAttackRequestToServer(int instanceID)
    {
        // 1. 데이터 준비 (LoginResponseDto.Instance 사용)
        AttackRequestDto request = new AttackRequestDto
        {
            characterID = LoginResponseDto.Instance.characterID,
            instanceID = instanceID,
            playerPosX = transform.position.x,
            playerPosY = transform.position.y
        };

        string jsonRequest = JsonUtility.ToJson(request);

        // 2. 이제 <AttackResponseDto> 형식 인수를 넣어도 에러가 나지 않습니다!
        StartCoroutine(NetworkManager.Instance.PostJson<AttackResponseDto>(
            "http://localhost:5043/api/Player/attack",
            jsonRequest,
            onSuccess: (result) => {
                // result가 이미 AttackResponseDto 타입이므로 바로 사용 가능!
                if (result.success)
                {
                    HandleAttackResponse(instanceID, result);
                }
            },
            onError: (err) => Debug.LogError($"공격 요청 실패: {err}")
        ));
    }

    private void HandleAttackResponse(int instanceID, AttackResponseDto result)
    {
        if (result.isDead)
        {
            // 몬스터 사망 시 화면에서 제거 (MonsterManager 활용)
            MonsterManager.Instance.RemoveMonster(instanceID);

            // UI에 획득 골드 표시 등 추가 로직
            Debug.Log($"몬스터 처치! 현재 골드: {result.currentGold}");
        }
        else
        {
            // 아직 살아있다면 몬스터 객체에게 피격 연출 명령
            // (예: 데미지 텍스트 띄우기)
        }
    }

    // 에디터에서 공격 범위를 시각적으로 확인하기 위함
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}