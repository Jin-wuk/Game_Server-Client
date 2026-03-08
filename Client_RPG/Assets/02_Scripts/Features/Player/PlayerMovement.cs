using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 1.5f;
    [HideInInspector] public bool canMove = true;

    private Rigidbody2D rigid;
    private Animator anim;
    private SpriteRenderer spriteRenderer;

    private Vector2 moveInput;      // 키보드 입력 값
    private Vector2? targetPosition; // 클릭 이동 목적지 (null 허용)

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigid.gravityScale = 0f;
    }

    private void Update()
    {
        if (!canMove) { StopMovement(); return; }

        // 1. 키보드 입력 확인
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        moveInput = new Vector2(h, v).normalized;

        //  키보드 입력이 있으면 클릭 이동 목적지를 취소합니다. (우선순위)
        if (moveInput.magnitude > 0.01f)
        {
            targetPosition = null;
        }

        // 2. 애니메이션 및 방향 처리
        HandleVisuals();
    }

    private void FixedUpdate()
    {
        if (!canMove) return;

        if (targetPosition.HasValue) // 클릭 이동 중이라면
        {
            float dist = Vector2.Distance(transform.position, targetPosition.Value);
            if (dist > 0.1f)
            {
                Vector2 dir = (targetPosition.Value - (Vector2)transform.position).normalized;
                rigid.velocity = dir * moveSpeed;
            }
            else
            {
                StopMovement(); // 도착 시 정지
            }
        }
        else // 키보드 이동 중이라면
        {
            rigid.velocity = moveInput * moveSpeed;
        }
    }

    //  외부(Combat)에서 클릭 이동을 시킬 때 호출
    public void SetClickTarget(Vector2 pos)
    {
        if (!canMove) return;
        targetPosition = pos;
    }

    public void StopMovement()
    {
        targetPosition = null;
        moveInput = Vector2.zero;
        rigid.velocity = Vector2.zero;
        if (anim != null) anim.SetBool("isWalking", false);
    }

    private void HandleVisuals()
    {
        Vector2 currentVelocity = rigid.velocity;
        bool isWalking = currentVelocity.magnitude > 0.1f;

        if (anim != null) anim.SetBool("isWalking", isWalking);

        if (currentVelocity.x < -0.1f) spriteRenderer.flipX = true;
        else if (currentVelocity.x > 0.1f) spriteRenderer.flipX = false;
    }
}