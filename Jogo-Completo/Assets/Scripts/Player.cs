using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rig;

    [Header("Movimento")]
    public float speed = 5f;
    public float runMultiplier = 1.8f;
    private float currentSpeed;
    private bool isRunning;
    private float lastTapTime;
    public float doubleTapTime = 0.25f;

    [Header("Pulo")]
    public float jumpForce = 10f;
    private bool isGrounded;
    private bool canDoubleJump;

    [Header("Combate")]
    private bool isAttacking = false;
    public float attackDuration = 0.5f;
    private float attackTimer;

    // --- HITBOX ---
    public Transform attackOrigin;
    public float radiusAttack = 1f;
    public LayerMask enemieLayer;
    // --- FIM HITBOX ---

    [Header("Geral")]
    public Vector2 posicaoInicial;

    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        posicaoInicial = transform.position;
        currentSpeed = speed;
    }

    void Update()
    {
        if (isAttacking)
        {
            HandleAttackTimer();
            return;
        }

        HandleAttackInput();
        HandleRunInput();
        Move();
        Jump();
        UpdateAnimations();
    }

    // ------------------ ATAQUE ------------------

    void HandleAttackInput()
    {
        if (Input.GetKeyDown(KeyCode.J) && !isAttacking)
        {
            isAttacking = true;
            attackTimer = attackDuration;

            anim.SetInteger("transitions", 4);
            rig.linearVelocity = new Vector2(0, rig.linearVelocity.y);

            AttackHitbox(); // chama a hitbox na hora que aperta J
        }
    }

    void HandleAttackTimer()
    {
        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0)
        {
            isAttacking = false;
            UpdateAnimations();
        }
    }

    // --- Sistema de Hitbox ---
    void AttackHitbox()
    {
        Collider2D hit = Physics2D.OverlapBox(
            attackOrigin.position + transform.right * (radiusAttack / 2),
            new Vector2(radiusAttack, radiusAttack / 2),
            0,
            enemieLayer
        );

        if (hit)
        {
            Destroy(hit.gameObject); // mata o inimigo
        }
    }
    // --- FIM HITBOX ---

    // ------------------

    void HandleRunInput()
    {
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow) ||
            Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (Time.time - lastTapTime < doubleTapTime)
                isRunning = true;
            lastTapTime = Time.time;
        }

        if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.RightArrow) ||
            Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.LeftArrow))
        {
            isRunning = false;
        }

        currentSpeed = isRunning ? speed * runMultiplier : speed;
    }

    void Move()
    {
        if (isAttacking) return;

        float h = Input.GetAxisRaw("Horizontal");
        rig.linearVelocity = new Vector2(h * currentSpeed, rig.linearVelocity.y);

        if (h > 0.01f) transform.localScale = new Vector3(1, 1, 1);
        else if (h < -0.01f) transform.localScale = new Vector3(-1, 1, 1);
    }

    void Jump()
    {
        if (isAttacking) return;

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
                rig.linearVelocity = new Vector2(rig.linearVelocity.x, 0f);
                rig.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                isGrounded = false;
                canDoubleJump = true;

                anim.SetInteger("transitions", 2);
            }
            else if (canDoubleJump)
            {
                rig.linearVelocity = new Vector2(rig.linearVelocity.x, 0f);
                rig.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                canDoubleJump = false;

                anim.SetInteger("transitions", 2);
            }
        }
    }

    void UpdateAnimations()
    {
        if (isAttacking) return;

        float h = Mathf.Abs(Input.GetAxisRaw("Horizontal"));
        float velY = rig.linearVelocity.y;

        if (!isGrounded)
        {
            if (velY > 0.1f)
                anim.SetInteger("transitions", 2);
            else if (velY < -0.1f)
                anim.SetInteger("transitions", 6);
        }
        else
        {
            if (h > 0.1f)
                anim.SetInteger("transitions", isRunning ? 3 : 1);
            else
                anim.SetInteger("transitions", 0);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("tagGround"))
        {
            isGrounded = true;
            canDoubleJump = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("morreu"))
            ReiniciarPosicao();

        if (collision.CompareTag("prox_f2"))
            SceneManager.LoadScene("Fase_2");
    }

    public void ReiniciarPosicao()
    {
        transform.position = posicaoInicial;
        rig.linearVelocity = Vector2.zero;
        isRunning = false;
        isGrounded = true;
        canDoubleJump = false;
        isAttacking = false;
        anim.SetInteger("transitions", 0);
    }

    // Gizmo para visualizar hitbox na cena
    private void OnDrawGizmosSelected()
    {
        if (attackOrigin != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(
                attackOrigin.position + Vector3.right * (radiusAttack / 2f),
                new Vector3(radiusAttack, radiusAttack / 2f, 1)
            );
        }
    }
}