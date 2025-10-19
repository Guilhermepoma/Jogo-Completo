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
        rigd.linearVelocity = new Vector2(teclas * speed, rigd.linearVelocity.y); // corrigido: velocity

        if (teclas > 0 && isGround)
        {
            transform.eulerAngles = new Vector3(0, 0, 0); // corrigido: Vector3
            anim.SetInteger("transition", 1);
        }
        else if (teclas < 0 && isGround)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
            anim.SetInteger("transition", 1);
        }
        else if (teclas == 0 && isGround)
        {
            anim.SetInteger("transition", 0);
        }
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.W) && isGround)
        {
            rigd.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            anim.SetInteger("transition", 2);
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