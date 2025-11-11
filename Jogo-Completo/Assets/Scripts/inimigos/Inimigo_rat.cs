using UnityEngine;

public class Inimigo_rat : MonoBehaviour
{
    public GameManager gameManager;
    public Transform player;
    public float speed = 2f;
    public float chaseDistance = 4f;

    [Header("Vida do Inimigo")]
    public int vida = 3; // Quantidade de vida do rato
    private bool morto = false;
    private bool podeTomarDano = true;

    private Animator anim;
    private Rigidbody2D rig;

    void Start()
    {
        anim = GetComponent<Animator>();
        rig = GetComponent<Rigidbody2D>();

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (morto) return;

        float distance = Vector2.Distance(transform.position, player.position);

        // Movimento de perseguição
        if (distance < chaseDistance)
        {
            anim.SetInteger("transitions", 1); // Correndo
            Vector2 direction = (player.position - transform.position).normalized;
            rig.linearVelocity = new Vector2(direction.x * speed, rig.linearVelocity.y);

            // Faz o inimigo olhar para o player
            transform.localScale = new Vector3(Mathf.Sign(direction.x), 1, 1);
        }
        else
        {
            anim.SetInteger("transitions", 0); // Idle
            rig.linearVelocity = new Vector2(0, rig.linearVelocity.y);
        }
    }

    // Quando encosta no player → o rato causa dano no jogador
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            gameManager.PerderVidas(1);
        }
    }

    // --- AQUI é onde o player dá dano no rato ---
    public void TomarDano(int dano)
    {
        if (!podeTomarDano || morto) return;

        vida -= dano;
        podeTomarDano = false;
        anim.SetTrigger("hit"); // ativa animação de "tomando dano" (se tiver)
        Invoke(nameof(PodeTomarDanoDeNovo), 0.3f);

        if (vida <= 0)
        {
            Morrer();
        }
    }

    void PodeTomarDanoDeNovo()
    {
        podeTomarDano = true;
    }

    void Morrer()
    {
        morto = true;
        anim.SetTrigger("morreu"); // animação de morte (se tiver)
        rig.linearVelocity = Vector2.zero;
        GetComponent<Collider2D>().enabled = false;
        Destroy(gameObject, 0.6f); // destrói o inimigo depois da animação
    }
}