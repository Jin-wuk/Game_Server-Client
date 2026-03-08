using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//  클래스 PlayerState와 충돌하지 않도록 enum 이름 유지
public enum PlayerActionState { Idle, Chasing, Attacking }

public class PlayerCombat : MonoBehaviour
{
    [Header("설정")]
    public LayerMask monsterLayer;
    public float attackRange = 1.5f;
    public float centerYOffset = 0.5f;

    [Header("상태")]
    //  변수명을 currentActionState로 통일
    public PlayerActionState currentActionState = PlayerActionState.Idle;
    private MonsterController targetMonster;

    [Header("참조")]
    private Animator anim;
    private PlayerMovement movement;
    private PlayerState playerStats; //  스탯/골드 관리용 클래스 참조

    private void Awake()
    {
        anim = GetComponent<Animator>();
        movement = GetComponent<PlayerMovement>();
        playerStats = GetComponent<PlayerState>();
    }

    private void Update()
    {
        if (currentActionState == PlayerActionState.Attacking) return;

        if (Input.GetMouseButtonDown(0))
        {
            HandleMouseClick();
        }

        if (currentActionState == PlayerActionState.Chasing && targetMonster != null)
        {
            float distance = Vector2.Distance(GetAttackCenter(), targetMonster.transform.position);
            if (distance <= attackRange)
            {
                StartCoroutine(PerformAttack());
            }
            else
            {
                movement.SetClickTarget(targetMonster.transform.position);
            }
        }
    }

    // 플레이어의 중앙 위치를 반환하는 헬퍼 함수
    private Vector3 GetAttackCenter()
    {
        // transform.position(발바닥)에서 centerYOffset만큼 올린 지점이 실제 공격의 중심
        return transform.position + new Vector3(0, centerYOffset, 0);
    }

    private void HandleMouseClick()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, 0f, monsterLayer);

        if (hit.collider != null)
        {
            targetMonster = hit.collider.GetComponent<MonsterController>();
            if (targetMonster != null)
            {
                //  PlayerState(클래스)가 아닌 PlayerActionState(Enum)를 대입
                currentActionState = PlayerActionState.Chasing;
            }
        }
        else
        {
            targetMonster = null;
            currentActionState = PlayerActionState.Idle;
            movement.SetClickTarget(mousePos);
        }
    }

    private IEnumerator PerformAttack()
    {
        currentActionState = PlayerActionState.Attacking;
        movement.canMove = false;
        movement.StopMovement();

        //  공격 방향 전환: 몬스터가 내 왼쪽에 있으면 왼쪽을 보게 함
        if (targetMonster != null)
        {
            float dir = targetMonster.transform.position.x - transform.position.x;
            GetComponent<SpriteRenderer>().flipX = (dir < 0);
        }

        if (anim != null) anim.SetTrigger("doAttack");

        //  중앙 중심 범위 공격 판정
        Collider2D[] hitMonsters = Physics2D.OverlapCircleAll(GetAttackCenter(), attackRange, monsterLayer);

        foreach (Collider2D monster in hitMonsters)
        {
            var monsterCtrl = monster.GetComponent<MonsterController>();
            if (monsterCtrl != null)
            {
                SendAttackRequestToServer(monsterCtrl);
                // break를 지우면 범위 내 모든 몬스터를 동시에 공격합니다!
                break;
            }
        }

        yield return new WaitForSeconds(0.6f);

        movement.canMove = true;
        currentActionState = PlayerActionState.Idle;
        targetMonster = null;
    }

    private void SendAttackRequestToServer(MonsterController target)
    {
        AttackRequestDto request = new AttackRequestDto
        {
            characterID = LoginResponseDto.Instance.characterID,
            instanceID = target.instanceID,
            playerPosX = transform.position.x,
            playerPosY = transform.position.y,
            monsterPosX = target.transform.position.x,
            monsterPosY = target.transform.position.y
        };

        string jsonRequest = JsonUtility.ToJson(request);

        StartCoroutine(NetworkManager.Instance.PostJson<AttackResponseDto>(
            "http://localhost:5043/api/Player/attack",
            jsonRequest,
            onSuccess: (result) => {
                if (result.success) HandleAttackResponse(target.instanceID, result);
            },
            onError: (err) => Debug.LogError($"공격 요청 실패: {err}")
        ));
    }

    private void HandleAttackResponse(int instanceID, AttackResponseDto result)
    {
        if (result.isDead)
        {
            MonsterManager.Instance.RemoveMonster(instanceID);

            // [연동] 서버에서 받아온 골드를 내 스탯창에 반영합니다!
            if (playerStats != null)
            {
                // result.currentGold가 있다면 그걸로 덮어씌워도 좋고, 일단 100골드씩 추가해도 됩니다.
                playerStats.AddGold(100);
                Debug.Log($"골드 획득! 현재 골드: {playerStats.Gold}");
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // 1. 색상을 노란색으로 설정하여 가시성을 높입니다.
        Gizmos.color = Color.yellow;

        // 2. 공격 중심점을 계산합니다.
        Vector3 center = GetAttackCenter();

        // 3. 중앙에 아주 작은 꽉 찬 원을 그려 중심점을 명확히 표시합니다.
        Gizmos.DrawSphere(center, 0.1f);

        // 4. 실제 공격 사거리인 와이어 형태의 원을 그립니다.
        Gizmos.DrawWireSphere(center, attackRange);
    }
}