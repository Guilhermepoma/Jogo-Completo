using System.Collections;
using UnityEngine;

public class Boneco_50 : MonoBehaviour, IDano
{
    [Header("Referências")]
    public Transform player;
    private Rigidbody2D rig;
    private Animator anim;
    private SpriteRenderer sr;

    [Header("Movimentação")]
    public float speed = 2f;
    public float chaseDistance = 4f;

    [Header("Ataque")]
    public float attackDistance = 1f;
    public float attackCooldown = 1.2f;
    private bool podeAtacar = true;
    public GameObject hitboxAtaque;

    [Header("Vida")]
    public int vida = 3;
    private bool morto = false;
    private bool podeTomarDano = true;

    // Movimento aleatório
    private float randomMoveTime = 0f;
    private float currentDirection = 0f;


    // --------------------------------------
    // START
    // --------------------------------------
    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;
    }


    // --------------------------------------
    // UPDATE
    // --------------------------------------
    void Update()
    {
        if (morto) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= attackDistance)
        {
            Atacar();
        }
        else if (distance < chaseDistance)
        {
            PerseguirPlayer();
        }
        else
        {
            MovimentoAleatorio();
        }
    }


    // --------------------------------------
    // PERSEGUIR PLAYER
    // --------------------------------------
    void PerseguirPlayer()
    {
        anim.SetInteger("transitions", 1);

        Vector2 direction = (player.position - transform.position).normalized;
        rig.linearVelocity = new Vector2(direction.x * speed, rig.linearVelocity.y);

        transform.localScale = new Vector3(Mathf.Sign(direction.x), 1, 1);
    }


    // --------------------------------------
    // ATAQUE
    // --------------------------------------
    void Atacar()
    {
        rig.linearVelocity = Vector2.zero;

        if (podeAtacar)
        {
            podeAtacar = false;
            anim.SetInteger("transitions", 2);

            StartCoroutine(AtivarHitbox());

            Invoke(nameof(ResetarAtaque), attackCooldown);
        }
    }

    IEnumerator AtivarHitbox()
    {
        yield return new WaitForSeconds(0.15f);

        hitboxAtaque.SetActive(true);

        yield return new WaitForSeconds(0.2f);
        hitboxAtaque.SetActive(false);
    }

    void ResetarAtaque()
    {
        anim.SetInteger("transitions", 0);
        podeAtacar = true;
    }


    // --------------------------------------
    // MOVIMENTO ALEATÓRIO
    // --------------------------------------
    void MovimentoAleatorio()
    {
        anim.SetInteger("transitions", 0);

        randomMoveTime -= Time.deltaTime;

        if (randomMoveTime <= 0)
        {
            randomMoveTime = Random.Range(1f, 3f);
            int escolha = Random.Range(0, 3);

            currentDirection = escolha == 0 ? 0 : (escolha == 1 ? 1 : -1);
        }

        rig.linearVelocity = new Vector2(currentDirection * (speed * 0.5f), rig.linearVelocity.y);

        if (currentDirection != 0)
            transform.localScale = new Vector3(currentDirection, 1, 1);
    }


    // -------------------------------------------------------------------
    // === O PLAYER CHAMA ESTE MÉTODO! (TakeHit) ===
    // -------------------------------------------------------------------
    public void TakeHit()
    {
        if (!podeTomarDano || morto) return;

        vida--;
        StartCoroutine(Piscar());

        podeTomarDano = false;
        Invoke(nameof(ResetarDano), 0.25f);

        if (vida <= 0)
            Morrer();
    }

    IEnumerator Piscar()
    {
        sr.color = new Color(1, 1, 1, 0.3f);
        yield return new WaitForSeconds(0.1f);
        sr.color = Color.white;
    }

    void ResetarDano()
    {
        podeTomarDano = true;
    }


    // --------------------------------------
    // MORRER
    // --------------------------------------
    void Morrer()
    {
        morto = true;
        rig.linearVelocity = Vector2.zero;

        GetComponent<Collider2D>().enabled = false;

        Destroy(gameObject, 0.6f);
    }
}