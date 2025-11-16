using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rig;
    private GameManager gameManager;

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

    [Header("HITBOX")]
    public Transform attackOrigin;
    public float radiusAttack = 1f;
    public LayerMask enemieLayer;

    [Header("Dano / Invencibilidade")]
    public bool isInvincible = false;
    public float invincibleTime = 1f;
    public float blinkSpeed = 0.1f;

    public float knockbackForce = 10f;
    public float knockbackDuration = 0.15f;
    private bool isKnockback = false;

    [Header("Geral")]
    public Vector2 posicaoInicial;

    private bool isDead = false;

    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        gameManager = FindAnyObjectByType<GameManager>();

        posicaoInicial = transform.position;
        currentSpeed = speed;
    }

    void Update()
    {
        if (isDead) return;
        if (isKnockback) return;

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

    // ---------- SISTEMA DE DANO ----------
    public void TomarDano(int quantidade)
    {
        if (isInvincible || isKnockback || isDead)
            return;

        anim.SetTrigger("hit");

        if (gameManager != null)
            gameManager.PerderVidas(quantidade);

        // --- MORREU ---
        if (gameManager.vidas <= 0)
        {
            Morrer();
            return;
        }

        StartCoroutine(Invencibilidade());
        StartCoroutine(Knockback());
    }
    // -------------------------------------

    private void Morrer()
    {
        Debug.Log("Player morreu!");

        isDead = true;

        // Travar movimentação
        rig.linearVelocity = Vector2.zero;
        rig.gravityScale = 0f;

        // Desativar colisão para não cair do mapa
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        // Anim de morte
        anim.SetInteger("transitions", 6);

        // Impedir qualquer ação
        isAttacking = true;
        isKnockback = true;
        isInvincible = true;

        // Trocar de cena depois de 5s
        StartCoroutine(VoltarMenu());
    }

    // ---------- VOLTAR PARA O MENU ----------
    private System.Collections.IEnumerator VoltarMenu()
    {
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene("Menu");
    }
    // ----------------------------------------

    // --------------- ATAQUE ---------------
    void HandleAttackInput()
    {
        if (isDead) return;

        if (Input.GetKeyDown(KeyCode.J) && !isAttacking)
        {
            isAttacking = true;
            attackTimer = attackDuration;

            anim.SetInteger("transitions", 4);
            rig.linearVelocity = new Vector2(0, rig.linearVelocity.y);

            AttackHitbox();
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
            Destroy(hit.gameObject);
        }
    }
    // --------------------------------------

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
        if (isAttacking || isDead) return;

        float h = Input.GetAxisRaw("Horizontal");
        rig.linearVelocity = new Vector2(h * currentSpeed, rig.linearVelocity.y);

        if (h > 0.01f)
            transform.localScale = new Vector3(1, 1, 1);
        else if (h < -0.01f)
            transform.localScale = new Vector3(-1, 1, 1);

        if (attackOrigin != null)
            attackOrigin.localScale = new Vector3(transform.localScale.x, 1, 1);
    }

    void Jump()
    {
        if (isAttacking || isDead) return;

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
        if (isAttacking || isDead) return;

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

        if (collision.gameObject.CompareTag("Inimigo"))
        {
            TomarDano(1);
            Debug.Log("Player tomou dano do inimigo!");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("morreu"))
            TomarDano(1);

        if (collision.CompareTag("prox_f2"))
            SceneManager.LoadScene("Fase_2");
    }

    // ---------- KNOCKBACK ----------
    private System.Collections.IEnumerator Knockback()
    {
        if (isDead) yield break;

        isKnockback = true;

        float direction = transform.localScale.x > 0 ? -1 : 1;

        rig.linearVelocity = new Vector2(direction * knockbackForce, rig.linearVelocity.y);

        yield return new WaitForSeconds(knockbackDuration);

        isKnockback = false;
    }

    // ---------- INVENCIBILIDADE ----------
    private System.Collections.IEnumerator Invencibilidade()
    {
        if (isDead) yield break;

        isInvincible = true;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        float timer = 0f;

        while (timer < invincibleTime)
        {
            sr.enabled = !sr.enabled;
            timer += blinkSpeed;
            yield return new WaitForSeconds(blinkSpeed);
        }

        sr.enabled = true;
        isInvincible = false;
    }

    // Gizmo do ataque
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