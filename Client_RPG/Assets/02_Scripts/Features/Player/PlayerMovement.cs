
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("플레이어 이동설정")]
    public float moveSpeed = 5f;

    private Rigidbody2D rigid;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private Vector2 moveInput;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        rigid.gravityScale = 0f;
    }

    private void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        moveInput = new Vector2(moveX, moveY).normalized;

        if (anim != null )
        {
            anim.SetBool("isWalking", moveInput.magnitude > 0.01f);
        }

        if (moveX < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (moveX > 0)
        {
            spriteRenderer.flipX = false;
        }
    }

    private void FixedUpdate()
    {
        rigid.velocity = moveInput * moveSpeed;
    }
}
