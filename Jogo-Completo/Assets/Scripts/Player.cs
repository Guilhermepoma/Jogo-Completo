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

    // ATAQUE NORMAL
    [Header("Ataque Normal")]
    public float attackRange = 1f;
    public float attackCooldown = 0.3f;
    public LayerMask enemyLayer;
    public float pushForce = 5f;
    public float attackInvincibleTime = 0.3f;

    private bool canAttack = true;
    private bool isInvincibleAttack = false;
    private bool isAttacking = false;

    // ATAQUE ESPECIAL (Boneco Simples)
    [Header("Ataque para Boneco Simples")]
    public Transform attackOrigin;
    public float radiusAttack = 1.2f;
    public LayerMask enemieLayer;

    // NOVO ATAQUE ESPECIAL (Malgo)
    [Header("Novo Ataque Especial")]
    public Transform attackOriginMalgo;
    public float radiusAttackMalgo = 1.2f;
    public LayerMask enemyLayerMalgo;
    public float pushForceMalgo = 5f;

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

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        posicaoInicial = transform.position;
    }

    void Update()
    {
        if (isDead || isKnockback) return;

        CheckGround();
        HandleRunInput();
        HandleJumpInput();
        Move();

        if (!isAttacking)
            UpdateAnimations();

        HandleAttackInput();
    }

    // === INPUT DE ATAQUE ===
    void HandleAttackInput()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            // ataque especial Boneco Simples
            AtaqueBonecoSimples();
            NovoAtaqueEspecialMalgo();

            // ataque normal
            if (canAttack)
                StartCoroutine(NovoAtaque());
        }
    }

    // === ATAQUE ESPECIAL BONECO SIMPLES (intocado) ===
    void AtaqueBonecoSimples()
    {
        int facingDir = transform.localScale.x > 0 ? 1 : -1;
        Vector3 boxPos = attackOrigin.position + new Vector3(facingDir * (radiusAttack / 2), 0, 0);

        Collider2D hit = Physics2D.OverlapBox(
            boxPos,
            new Vector2(radiusAttack, radiusAttack / 2),
            0f,
            enemieLayer
        );

        if (hit != null)
        {
            Boneco_Simples bs = hit.GetComponent<Boneco_Simples>();
            if (bs != null)
                Destroy(hit.gameObject);
        }
    }

    // === NOVO ATAQUE ESPECIAL (Malgo) ===
    void NovoAtaqueEspecialMalgo()
    {
        int facingDir = transform.localScale.x > 0 ? 1 : -1;
        Vector3 boxPos = attackOriginMalgo.position + new Vector3(facingDir * (radiusAttackMalgo / 2), 0, 0);

        Collider2D hit = Physics2D.OverlapBox(
            boxPos,
            new Vector2(radiusAttackMalgo, radiusAttackMalgo / 2),
            0f,
            enemyLayerMalgo
        );

        if (hit != null)
        {
            IDano dano = hit.GetComponent<IDano>();
            if (dano != null)
            {
                dano.TakeHit();
                Debug.Log("Novo ataque especial acertou: " + hit.name);

                Rigidbody2D rb = hit.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    Vector2 push = (hit.transform.position - transform.position).normalized;
                    rb.AddForce(push * pushForceMalgo, ForceMode2D.Impulse);
                }
            }
        }
    }

    // === ATAQUE NORMAL ===
    IEnumerator NovoAtaque()
    {
        canAttack = false;
        isAttacking = true;

        anim.SetInteger("transitions", ANIM_ATAQUE);
        rig.linearVelocity = new Vector2(0, rig.linearVelocity.y);

        if (somAtaque != null)
            audioSource.PlayOneShot(somAtaque);

        isInvincibleAttack = true;

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayer);

        foreach (Collider2D inimigo in hitEnemies)
        {
            var dano = inimigo.GetComponent<IDano>();
            if (dano != null)
            {
                dano.TakeHit();

                Rigidbody2D rb = inimigo.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    Vector2 push = (inimigo.transform.position - transform.position).normalized;
                    rb.AddForce(push * pushForce, ForceMode2D.Impulse);
                }
            }
        }

        yield return new WaitForSeconds(attackInvincibleTime);
        isInvincibleAttack = false;

        yield return new WaitForSeconds(0.25f);
        isAttacking = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    // === MOVIMENTO ===
    void CheckGround()
    {
        isGround = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (isGround) canDoubleJump = true;
    }

    void HandleRunInput()
    {
        float h = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.A))
        {
            if (Time.time - lastTapTime < doubleTapTime)
                isRunning = true;

            lastTapTime = Time.time;
        }

        if (h == 0) isRunning = false;

        currentSpeed = isRunning ? speed * runMultiplier : speed;
    }

    void Move()
    {
        float h = Input.GetAxisRaw("Horizontal");
        rig.linearVelocity = new Vector2(h * currentSpeed, rig.linearVelocity.y);

        if (h > 0.1f) transform.localScale = new Vector3(1, 1, 1);
        else if (h < -0.1f) transform.localScale = new Vector3(-1, 1, 1);
    }

    void HandleJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (isGround || canDoubleJump)
            {
                rig.linearVelocity = new Vector2(rig.linearVelocity.x, 0);
                rig.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

                if (!isGround) canDoubleJump = false;

                if (somPulo) audioSource.PlayOneShot(somPulo);
            }
        }
    }

    void UpdateAnimations()
    {
        float h = Mathf.Abs(Input.GetAxisRaw("Horizontal"));
        float y = rig.linearVelocity.y;

        if (y < -0.1f) anim.SetInteger("transitions", ANIM_CAINDO);
        else if (y > 0.1f) anim.SetInteger("transitions", ANIM_PULANDO);
        else if (isGround)
        {
            if (h > 0.1f)
                anim.SetInteger("transitions", isRunning ? ANIM_CORRENDO : ANIM_ANDANDO);
            else
                anim.SetInteger("transitions", ANIM_PARADO);
        }
    }

    // === DANO ===
    public void TomarDano(int quantidade)
    {
        if (isInvincible || isInvincibleAttack || isKnockback || isDead) return;

        if (somDano) audioSource.PlayOneShot(somDano);

        if (gameManager != null)
            gameManager.PerderVidas(quantidade);

        if (gameManager.vidas <= 0)
        {
            Morrer();
            return;
        }

        StartCoroutine(Invencibilidade());
        StartCoroutine(Knockback());
    }

    private void Morrer()
    {
        isDead = true;
        rig.linearVelocity = Vector2.zero;
        rig.gravityScale = 0;
        GetComponent<Collider2D>().enabled = false;

        anim.SetInteger("transitions", ANIM_MORTE);
        StartCoroutine(VoltarMenu());
    }

    IEnumerator VoltarMenu()
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("Menu");
    }

    IEnumerator Knockback()
    {
        isKnockback = true;

        float dir = transform.localScale.x > 0 ? -1 : 1;
        rig.linearVelocity = new Vector2(dir * knockbackForce, rig.linearVelocity.y);

        yield return new WaitForSeconds(knockbackDuration);
        isKnockback = false;
    }

    IEnumerator Invencibilidade()
    {
        isInvincible = true;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        float t = 0;
        while (t < invincibleTime)
        {
            sr.enabled = !sr.enabled;
            yield return new WaitForSeconds(blinkSpeed);
            t += blinkSpeed;
        }

        sr.enabled = true;
        isInvincible = false;
    }

    private void OnDrawGizmos()
    {
        if (attackOrigin == null) return;

        Gizmos.color = Color.blue;
        int facingDir = transform.localScale.x > 0 ? 1 : -1;
        Vector3 boxPos = attackOrigin.position + new Vector3(facingDir * (radiusAttack / 2), 0, 0);
        Gizmos.DrawWireCube(boxPos, new Vector3(radiusAttack, radiusAttack / 2, 0.1f));

        if (attackOriginMalgo != null)
        {
            Vector3 boxPosMalgo = attackOriginMalgo.position + new Vector3(facingDir * (radiusAttackMalgo / 2), 0, 0);
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(boxPosMalgo, new Vector3(radiusAttackMalgo, radiusAttackMalgo / 2, 0.1f));
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Inimigo"))
            TomarDano(1);
    }
}