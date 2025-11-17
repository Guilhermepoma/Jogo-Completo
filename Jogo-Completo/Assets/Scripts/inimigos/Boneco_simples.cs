using UnityEngine;

public class Boneco_Simples : MonoBehaviour
{
    public Transform player;
    public float speed = 2f;
    public float chaseDistance = 4f;

    [Header("Vida do Inimigo")]
    public int vida = 2;
    private bool morto = false;
    private bool podeTomarDano = true;

    [Header("Dano no Player")]
    public int danoNoPlayer = 1;
    public float tempoEntreDanos = 0.5f;
    private bool podeDarDano = true;

    private Animator anim;
    private Rigidbody2D rig;

    private float randomMoveTime = 0f;
    private float currentDirection = 0f;

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
            PerseguirPlayer();
        else
            MovimentoAleatorio();
    }

    void PerseguirPlayer()
    {
        anim.SetInteger("transitions", 1);

        Vector2 direction = (player.position - transform.position).normalized;
        rig.linearVelocity = new Vector2(direction.x * speed, rig.linearVelocity.y);

        transform.localScale = new Vector3(Mathf.Sign(direction.x), 1, 1);
    }

    void MovimentoAleatorio()
    {
        anim.SetInteger("transitions", 0);

        randomMoveTime -= Time.deltaTime;

        if (randomMoveTime <= 0)
        {
            randomMoveTime = Random.Range(1f, 3f);

            int escolha = Random.Range(0, 3);
            if (escolha == 0) currentDirection = 0;
            else if (escolha == 1) currentDirection = 1;
            else currentDirection = -1;
        }

        rig.linearVelocity = new Vector2(currentDirection * (speed * 0.5f), rig.linearVelocity.y);

        if (currentDirection != 0)
            transform.localScale = new Vector3(currentDirection, 1, 1);
    }

    public void TomarDano(int dano)
    {
        if (!podeTomarDano || morto) return;

        vida -= dano;
        podeTomarDano = false;

        anim.SetInteger("transitions", 2);

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
        anim.SetInteger("transitions", 3);

        rig.linearVelocity = Vector2.zero;
        GetComponent<Collider2D>().enabled = false;

        Destroy(gameObject, 0.6f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && podeDarDano)
        {
            collision.gameObject.GetComponent<PlayerRefatorado>().TomarDano(danoNoPlayer);

            podeDarDano = false;
            Invoke(nameof(PodeBaterDeNovo), tempoEntreDanos);
        }
    }

    void PodeBaterDeNovo()
    {
        podeDarDano = true;
    }
}