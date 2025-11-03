using UnityEngine;

public class Player : MonoBehaviour
{
    public Animator anim;
    private Rigidbody2D rigd;
    public float speed;

    public Vector2 posicaoInicial;
    public GameManager gameManager;

    // Pulo
    public float jumpForce;
    public bool isGround;
    private bool canDoubleJump;

    void Start()
    {
        posicaoInicial = transform.position;
        anim = GetComponent<Animator>();
        rigd = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Move();
        Jump();
    }

    void Move()
    {
        float teclas = Input.GetAxis("Horizontal");
        rigd.linearVelocity = new Vector2(teclas * speed, rigd.linearVelocityY);

        // virar o personagem sempre (no chão ou no ar)
        if (teclas > 0)
        {
            transform.eulerAngles = new Vector2(0, 0);
        }
        else if (teclas < 0)
        {
            transform.eulerAngles = new Vector2(0, 180);
        }

        // animações
        if (isGround)
        {
            if (Mathf.Abs(teclas) > 0)
            {
                anim.SetInteger("transitions", 1); // andando
            }
            else
            {
                anim.SetInteger("transitions", 0); // parado
            }
        }
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            // Primeiro pulo
            if (isGround)
            {
                rigd.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                anim.SetInteger("transitions", 2);
                isGround = false;
                canDoubleJump = true;
            }
            // Segundo pulo (no ar)
            else if (canDoubleJump)
            {
                rigd.linearVelocity = new Vector2(rigd.linearVelocity.x, 0f);
                rigd.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                anim.SetInteger("transitions", 2);
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
    }
}
