
using System.ComponentModel;
using UnityEngine;

public class MonsterController : MonoBehaviour
{
    [Header("몬스터 이동 설정")]
    public float moveSpeed = 2f;
    public float rangeRadius = 5f;
    public float idleTime = 1.5f;

    private Vector2 spawnPosition;
    private Vector2 targetPosition;
    private bool isMoving = false;

    private Rigidbody2D rigid;
    private Animator anim;
    private SpriteRenderer spriteRenderer;

    public void Init(Vector2 origin, float radius)
    {
        spawnPosition = origin;
        rangeRadius = radius;
        SetNewTarget();
    }

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigid.gravityScale = 0f; 
    }

    private void FixedUpdate()
    {
        if (isMoving)
        {
            MoveToTarget();
        }
        else
        {
            rigid.velocity = Vector2.zero;
            if (anim != null) anim.SetBool("isWalking", false);
        }
    }

    private void MoveToTarget()
    {
        Vector2 currentPos = rigid.position;
        float distance = Vector2.Distance(currentPos, targetPosition);

        // 1. 아직 목표에 도달하지 않았을 때만 이동 로직 수행
        if (distance > 0.15f) 
        {
            Vector2 direction = (targetPosition - currentPos).normalized;
            rigid.velocity = direction * moveSpeed;

            //  이동 중일 때만 방향 전환 (좌우 반전)
            if (direction.x < -0.01f) spriteRenderer.flipX = true;
            else if (direction.x > 0.01f) spriteRenderer.flipX = false;

            if (anim != null) anim.SetBool("isWalking", true);
        }
        // 2. 목표에 거의 다 왔을 때
        else
        {
            isMoving = false; // 이동 상태 종료 -> FixedUpdate의 else문이 실행됨
            rigid.velocity = Vector2.zero;

            // idleTime(예: 2초) 후에 새로운 목표 설정
            Invoke(nameof(SetNewTarget), idleTime);
        }
    }

    private void SetNewTarget()
    {
        Vector2 randomOffset = Random.insideUnitCircle * rangeRadius;
        targetPosition = spawnPosition + randomOffset;
        isMoving = true;
    }
}
