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
        HandleRunInput();
        Move();
        Jump();
        UpdateAnimations();
    }

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
        float h = Input.GetAxisRaw("Horizontal");
        rig.linearVelocity = new Vector2(h * currentSpeed, rig.linearVelocity.y);

        if (h > 0.01f) transform.localScale = new Vector3(1, 1, 1);
        else if (h < -0.01f) transform.localScale = new Vector3(-1, 1, 1);
    }

    void Jump()
    {
        // aceita W e Space
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
                rig.linearVelocity = new Vector2(rig.linearVelocity.x, 0f);
                rig.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                isGrounded = false;
                canDoubleJump = true;

                // força animação de pulo
                anim.SetInteger("transitions", 2); // 2 = Jump
            }
            else if (canDoubleJump)
            {
                rig.linearVelocity = new Vector2(rig.linearVelocity.x, 0f);
                rig.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                canDoubleJump = false;

                anim.SetInteger("transitions", 2); // pulo duplo também usa 2
            }
        }
    }

    void UpdateAnimations()
    {
        float h = Mathf.Abs(Input.GetAxisRaw("Horizontal"));
        float velY = rig.linearVelocity.y;

        // atualiza parâmetro vertical (se tiver transições baseadas nisso)
        anim.SetFloat("velocityY", velY);

        if (!isGrounded)
        {
            // Subida / queda: garantir valores corretos
            if (velY > 0.1f)
                anim.SetInteger("transitions", 2); // subindo / jump
            else if (velY < -0.1f)
                anim.SetInteger("transitions", 6); // caindo / fall
        }
        else
        {
            // no chão: idle / walk / run
            if (h > 0.1f)
                anim.SetInteger("transitions", isRunning ? 3 : 1); // 3=run,1=walk
            else
                anim.SetInteger("transitions", 0); // 0 = idle
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

    // Método requerido pelo GameManager e útil para reset manual
    public void ReiniciarPosicao()
    {
        transform.position = posicaoInicial;
        rig.linearVelocity = Vector2.zero;
        isRunning = false;
        isGrounded = true;
        canDoubleJump = false;
        anim.SetInteger("transitions", 0);
    }
}
