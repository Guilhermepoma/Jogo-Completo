using System.Runtime.ConstrainedExecution;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86;
using System.Collections;

public class Inimigo_esqueleto : MonoBehaviour
{
    public Transform player;
    public float speed = 2f;
    public float chaseDistance = 4f;

    [Header("Ataque")]
    public float attackDistance = 1f;
    public float attackCooldown = 1.2f;
    public int danoCausado = 1;
    private bool podeAtacar = true;

    [Header("Vida do Inimigo")]
    public int vida = 3;
    private bool morto = false;
    private bool podeTomarDano = true;

    private Animator anim;
    private Rigidbody2D rig;

    // Movimento aleatório
    private float randomMoveTime = 0f;
    private float currentDirection = 0f;

    public GameObject hitboxAtaque;


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

    void PerseguirPlayer()
    {
        anim.SetInteger("transitions", 1); // ANDANDO

        Vector2 direction = (player.position - transform.position).normalized;
        rig.linearVelocity = new Vector2(direction.x * speed, rig.linearVelocity.y);

        transform.localScale = new Vector3(Mathf.Sign(direction.x), 1, 1);
    }

    // === ATAQUE ===
    void Atacar()
    {
        rig.linearVelocity = Vector2.zero;

        if (podeAtacar)
        {
            podeAtacar = false;

            anim.SetInteger("transitions", 2); // ATAQUE

            StartCoroutine(AtivarHitbox());

            Invoke(nameof(ResetarAtaque), attackCooldown);
        }
    }

    IEnumerator AtivarHitbox()
    {
        yield return new WaitForSeconds(0.15f); // tempo até o golpe acertar (ajustável)

        hitboxAtaque.SetActive(true);

        yield return new WaitForSeconds(0.2f); // tempo ativo da hitbox
        hitboxAtaque.SetActive(false);
    }


    void ResetarAtaque()
    {
        anim.SetInteger("transitions", 0); // volta pro idle após atacar
        podeAtacar = true;
    }

    // === Movimento aleatório ===
    void MovimentoAleatorio()
    {
        anim.SetInteger("transitions", 0); // IDLE

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

    // === Receber dano ===
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