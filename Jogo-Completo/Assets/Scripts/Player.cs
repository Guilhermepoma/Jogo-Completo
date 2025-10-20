using UnityEngine;

public class Player : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rigd;
    public float speed;

    private Vector2 posicaoInicial;
    public GameManager gameManager;

    // Pulo
    public float jumpForce;
    public bool isGround;

    void Start()
    {
        posicaoInicial = transform.position;
        anim = GetComponentInChildren<Animator>(); // pega o Animator mesmo se estiver num filho
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
        rigd.linearVelocity = new Vector2(teclas * speed, rigd.linearVelocity.y);

        // Flip independente de estar no chão
        if (teclas > 0)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else if (teclas < 0)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }

        // Animações
        if (isGround)
        {
            if (Mathf.Abs(teclas) > 0)
                anim.SetInteger("transitions", 1); // andando
            else
                anim.SetInteger("transitions", 0); // parado
        }
        else
        {
            anim.SetInteger("transitions", 2); // pulando
        }
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.W) && isGround)
        {
            rigd.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            anim.SetInteger("transitions", 2);
            isGround = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("tagGround"))
        {
            isGround = true;
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