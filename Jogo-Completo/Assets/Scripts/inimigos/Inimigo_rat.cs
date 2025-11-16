using UnityEngine;

public class Inimigo_rat : MonoBehaviour
{
    public Transform player;
    public float speed = 2f;
    public float chaseDistance = 4f;

    [Header("Vida do Inimigo")]
    public int vida = 3;
    private bool morto = false;
    private bool podeTomarDano = true;

    private Animator anim;
    private Rigidbody2D rig;

    // Movimento aleatório
    private float randomMoveTime = 0f;
    private float currentDirection = 0f; // -1 esquerda, 1 direita, 0 parado

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

        if (distance < chaseDistance)
        {
            PerseguirPlayer();
        }
        else
        {
            MovimentoAleatorio();
        }
    }

    void PerseguirPlayer()
    {
        anim.SetInteger("transitions", 1);

        Vector2 direction = (player.position - transform.position).normalized;
        rig.linearVelocity = new Vector2(direction.x * speed, rig.linearVelocity.y);

        transform.localScale = new Vector3(Mathf.Sign(direction.x), 1, 1);
    }

    // --- Movimento aleatório ---
    void MovimentoAleatorio()
    {
        anim.SetInteger("transitions", 0);

        randomMoveTime -= Time.deltaTime;

        // Troca direção a cada 1–3 segundos
        if (randomMoveTime <= 0)
        {
            randomMoveTime = Random.Range(1f, 3f);

            int escolha = Random.Range(0, 3);
            // 0 = parado, 1 = direita, 2 = esquerda
            if (escolha == 0) currentDirection = 0;
            else if (escolha == 1) currentDirection = 1;
            else currentDirection = -1;
        }

        // Movimento suave
        rig.linearVelocity = new Vector2(currentDirection * (speed * 0.5f), rig.linearVelocity.y);

        // Virar pro lado certo
        if (currentDirection != 0)
            transform.localScale = new Vector3(currentDirection, 1, 1);
    }

    // --- Colisão com o Player ---
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Aqui você coloca o que quiser (dano no player, empurrão, etc)
        }
    }

    // --- Receber dano ---
    public void TomarDano(int dano)
    {
        if (!podeTomarDano || morto) return;

        vida -= dano;
        podeTomarDano = false;
        anim.SetTrigger("hit");

        Invoke(nameof(PodeTomarDanoDeNovo), 0.3f);

        if (vida <= 0)
            Morrer();
    }

    void PodeTomarDanoDeNovo()
    {
        podeTomarDano = true;
    }

    void Morrer()
    {
        morto = true;
        anim.SetTrigger("morreu");
        rig.linearVelocity = Vector2.zero;
        GetComponent<Collider2D>().enabled = false;
        Destroy(gameObject, 0.6f);
    }
}
