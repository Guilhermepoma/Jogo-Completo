using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerRefatorado : MonoBehaviour
{
    // Componentes
    private Animator anim;
    private Rigidbody2D rig;
    private GameManager gameManager;
    private AudioSource audioSource;

    // Constantes de Animação
    private const int ANIM_PARADO = 0;
    private const int ANIM_ANDANDO = 1;
    private const int ANIM_PULANDO = 2;
    private const int ANIM_CORRENDO = 3;
    private const int ANIM_ATAQUE = 4;
    private const int ANIM_CAINDO = 5;
    private const int ANIM_MORTE = 6;

    [Header("Movimento")]
    public float speed = 5f;
    public float runMultiplier = 1.8f;
    public float doubleTapTime = 0.25f;
    private float currentSpeed;
    private bool isRunning;
    private float lastTapTime;

    [Header("Pulo")]
    public float jumpForce = 10f;
    private bool isGround;
    private bool canDoubleJump;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Combate")]
    public float attackDuration = 0.5f;
    public Transform attackOrigin;
    public float radiusAttack = 1f;
    public LayerMask enemieLayer;
    private bool isAttacking = false;
    private float attackTimer;

    [Header("Estado")]
    public bool isInvincible = false;
    public float invincibleTime = 1f;
    public float blinkSpeed = 0.1f;
    public float knockbackForce = 10f;
    public float knockbackDuration = 0.15f;
    private bool isKnockback = false;
    private bool isDead = false;

    [Header("Geral")]
    public Vector2 posicaoInicial;

    [Header("Sons")]
    public AudioClip somPulo;
    public AudioClip somAtaque;
    public AudioClip somDano;

    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        gameManager = FindAnyObjectByType<GameManager>();

        // Pega ou adiciona AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        posicaoInicial = transform.position;

        if (groundLayer.value == 0)
            Debug.LogWarning("Ground Layer não definida. Use um LayerMask ou tag 'tagGround'.");
    }

    void Update()
    {
        if (isDead || isKnockback) return;

        CheckGround();

        HandleAttackInput();

        if (isAttacking)
            HandleAttackTimer();

        HandleRunInput();
        HandleJumpInput();

        Move();
        UpdateAnimations();
    }

    void CheckGround()
    {
        if (groundLayer.value != 0)
            isGround = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        else
        {
            isGround = false;
            Collider2D[] hits = Physics2D.OverlapCircleAll(groundCheck.position, groundCheckRadius);
            foreach (Collider2D hit in hits)
                if (hit.CompareTag("tagGround")) { isGround = true; break; }
        }

        if (isGround) canDoubleJump = true;
    }

    void HandleRunInput()
    {
        float h = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow) ||
            Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (Time.time - lastTapTime < doubleTapTime) isRunning = true;
            lastTapTime = Time.time;
        }

        if (h == 0)
            isRunning = false;
        else if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.RightArrow) ||
                 Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.LeftArrow))
        {
            if (!(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) ||
                  Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)))
                isRunning = false;
        }

        currentSpeed = isRunning ? speed * runMultiplier : speed;
    }

    void Move()
    {
        float h = Input.GetAxisRaw("Horizontal");
        rig.linearVelocity = new Vector2(h * currentSpeed, rig.linearVelocity.y);

        if (h > 0.01f) transform.localScale = new Vector3(1, 1, 1);
        else if (h < -0.01f) transform.localScale = new Vector3(-1, 1, 1);

        if (attackOrigin != null)
            attackOrigin.localScale = new Vector3(transform.localScale.x, 1, 1);
    }

    void HandleJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space))
        {
            if (isGround || canDoubleJump)
            {
                PerformJump();
                if (!isGround) canDoubleJump = false;
                anim.SetInteger("transitions", ANIM_PULANDO);

                // Som de pulo
                if (somPulo != null)
                    audioSource.PlayOneShot(somPulo);
            }
        }
    }

    void PerformJump()
    {
        rig.linearVelocity = new Vector2(rig.linearVelocity.x, 0f);
        rig.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        isGround = false;
    }

    void UpdateAnimations()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float velY = rig.linearVelocity.y;

        if (velY < -0.1f) anim.SetInteger("transitions", ANIM_CAINDO);
        else if (velY > 0.1f) anim.SetInteger("transitions", ANIM_PULANDO);
        else if (isGround)
        {
            if (Mathf.Abs(h) > 0.01f)
                anim.SetInteger("transitions", isRunning ? ANIM_CORRENDO : ANIM_ANDANDO);
            else
                anim.SetInteger("transitions", ANIM_PARADO);
        }
    }

    void HandleAttackInput()
    {
        if (Input.GetKeyDown(KeyCode.J) && !isAttacking)
        {
            isAttacking = true;
            attackTimer = attackDuration;

            anim.SetInteger("transitions", ANIM_ATAQUE);
            rig.linearVelocity = new Vector2(0, rig.linearVelocity.y);

            AttackHitbox();

            // Som de ataque
            if (somAtaque != null)
                audioSource.PlayOneShot(somAtaque);

            Debug.Log("Ataque iniciado!");
        }
    }

    void HandleAttackTimer()
    {
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0) isAttacking = false;
    }

    void AttackHitbox()
    {
        if (attackOrigin == null) return;

        Collider2D hit = Physics2D.OverlapBox(
            attackOrigin.position + transform.right * (radiusAttack / 2),
            new Vector2(radiusAttack, radiusAttack / 2),
            0,
            enemieLayer
        );

        if (hit) Destroy(hit.gameObject);
    }

    // ---------- DANO ----------
    public void TomarDano(int quantidade)
    {
        if (isInvincible || isKnockback || isDead) return;

        anim.SetTrigger("hit");

        // Som de dano
        if (somDano != null)
            audioSource.PlayOneShot(somDano);

        if (gameManager != null)
            gameManager.PerderVidas(quantidade);

        if (gameManager.vidas <= 0) { Morrer(); return; }

        StartCoroutine(Invencibilidade());
        StartCoroutine(Knockback());
    }

    private void Morrer()
    {
        Debug.Log("Player morreu!");
        isDead = true;

        rig.linearVelocity = Vector2.zero;
        rig.gravityScale = 0f;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        anim.SetInteger("transitions", ANIM_MORTE);

        isAttacking = true;
        isKnockback = true;
        isInvincible = true;

        StartCoroutine(VoltarMenu());
    }

    private IEnumerator VoltarMenu()
    {
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene("Menu");
    }

    private IEnumerator Knockback()
    {
        if (isDead) yield break;

        isKnockback = true;
        float direction = transform.localScale.x > 0 ? -1 : 1;
        rig.linearVelocity = new Vector2(direction * knockbackForce, rig.linearVelocity.y);

        yield return new WaitForSeconds(knockbackDuration);
        isKnockback = false;
    }

    private IEnumerator Invencibilidade()
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

    private void OnDrawGizmosSelected()
    {
        if (attackOrigin != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(
                attackOrigin.position + transform.right * (radiusAttack / 2f),
                new Vector3(radiusAttack, radiusAttack / 2f, 1)
            );
        }

        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Inimigo"))
        {
            TomarDano(1);
            Debug.Log("Player tomou dano do inimigo!");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("morreu")) TomarDano(1);
        if (collision.CompareTag("prox_f2")) SceneManager.LoadScene("Fase_2");
    }
}