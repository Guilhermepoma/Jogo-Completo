using UnityEngine;

public class Inimigo_rat : MonoBehaviour
{
    public GameManager gameManager;
    public Transform player;
    public float speed = 2f;
    public float chaseDistance = 4f;

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
        float distance = Vector2.Distance(transform.position, player.position);

        // Define a transição: 0 = Idle, 1 = Correndo
        float transitions = (distance < chaseDistance) ? 1f : 0f;


        //anim.SetFloat("transitions", transitions);

        // Movimento de perseguição
        if (distance < chaseDistance)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rig.linearVelocity = new Vector2(direction.x * speed, rig.linearVelocity.y);

            // Faz o inimigo olhar para o player
            transform.localScale = new Vector3(Mathf.Sign(direction.x), 1, 1);
        }
        else
        {
            rig.linearVelocity = new Vector2(0, rig.linearVelocity.y);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            gameManager.PerderVidas(1);
        }
    }
}