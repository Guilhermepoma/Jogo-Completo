using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public Animator anim;
    private Rigidbody2D rigd;

    [Header("Movimento")]
    public float speed = 5f;            // Velocidade de caminhada normal
    public float runSpeedMultiplier = 1.8f; // Multiplicador para a velocidade de corrida (ex: 5 * 1.8 = 9)
    private float currentSpeed;         // A velocidade que está sendo aplicada (speed ou speed * runSpeedMultiplier)

    // Variáveis para o Clique Duplo
    private float lastTapTime;          // Tempo em que o último clique foi registrado
    public float doubleTapTimeThreshold = 0.2f; // Tempo máximo entre os cliques para ser considerado duplo
    private bool isRunning = false;     // Indica se o player está atualmente correndo

    [Header("Configurações Gerais")]
    public Vector2 posicaoInicial;
    public GameManager gameManager;

    // Pulo
    public float jumpForce = 10f;
    public bool isGround;
    private bool canDoubleJump;

    void Start()
    {
        posicaoInicial = transform.position;
        anim = GetComponent<Animator>();
        rigd = GetComponent<Rigidbody2D>();
        currentSpeed = speed; // Inicia com a velocidade normal
    }

    void Update()
    {
        HandleDoubleTap(); // Novo: Trata o clique duplo
        Move();
        Jump();
    }

    // --- NOVO: Lógica de Clique Duplo (Double Tap) ---
    void HandleDoubleTap()
    {
        // 1. Verifica se a tecla D (ou RightArrow) foi pressionada
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            // Se o tempo entre o último clique e o clique atual for menor que o limite
            if (Time.time - lastTapTime < doubleTapTimeThreshold)
            {
                // É um clique duplo! Ativa a corrida.
                isRunning = true;
                currentSpeed = speed * runSpeedMultiplier;
            }
            lastTapTime = Time.time; // Registra o tempo do clique atual
        }

        // 2. Verifica se a tecla A (ou LeftArrow) foi pressionada
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (Time.time - lastTapTime < doubleTapTimeThreshold)
            {
                // É um clique duplo! Ativa a corrida.
                isRunning = true;
                currentSpeed = speed * runSpeedMultiplier;
            }
            lastTapTime = Time.time; // Registra o tempo do clique atual
        }

        // 3. Desativa a corrida se o player parar de se mover
        float teclas = Input.GetAxisRaw("Horizontal"); // Usamos Raw para saber se a tecla está segurada
        if (Mathf.Approximately(teclas, 0f) || !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            isRunning = false;
            currentSpeed = speed; // Volta para a velocidade normal
        }
    }
    // ----------------------------------------------------

    void Move()
    {
        float teclas = Input.GetAxis("Horizontal");

        // Usa a currentSpeed (que pode ser speed ou speed * runSpeedMultiplier)
        rigd.linearVelocity = new Vector2(teclas * currentSpeed, rigd.linearVelocity.y);

        // Virar o personagem (usando a propriedade .x do scale para flip)
        if (teclas > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (teclas < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        // Animações (só no chão)
        if (isGround)
        {
            if (Mathf.Abs(teclas) > 0)
            {
                // Se está correndo, usa a animação de corrida (transitions = 3)
                if (isRunning)
                {
                    anim.SetInteger("transitions", 5); // Player_walk (Corrida)
                }
                // Se está apenas andando (velocidade normal)
                else
                {
                    anim.SetInteger("transitions", 1); // Andando normal
                }
            }
            else
            {
                anim.SetInteger("transitions", 0); // Parado
            }
        }

        // Se a velocidade for alterada para correr, é importante que você ajuste o Animator
        // para que a transição de "Andando" para "Corrida" seja suave.
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            // Primeiro pulo
            if (isGround)
            {
                rigd.linearVelocity = new Vector2(rigd.linearVelocity.x, 0f); // Reseta a velocidade Y antes de pular
                rigd.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                anim.SetInteger("transitions", 2); // Pulo
                isGround = false;
                canDoubleJump = true;
            }
            // Segundo pulo (no ar)
            else if (canDoubleJump)
            {
                rigd.linearVelocity = new Vector2(rigd.linearVelocity.x, 0f); // Zera a velocidade vertical para um pulo consistente
                rigd.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                anim.SetInteger("transitions", 2); // Pulo
                canDoubleJump = false;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("tagGround"))
        {
            isGround = true;
            canDoubleJump = false;
        }
    }

    public void ReiniciarPosicao()
    {
        transform.position = posicaoInicial;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("morreu"))
        {
            transform.position = posicaoInicial;
        }

        if (collision.CompareTag("prox_f2"))
        {
            SceneManager.LoadScene("Fase_2");
        }
    }
}