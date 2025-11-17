using UnityEngine;

public class Inimigo_malgo : MonoBehaviour
{
    public Transform player;        // arrasta o player aqui
    public float speed = 3f;        // velocidade de voo
    public float followDistance = 10f; // distância para começar a seguir

    private Animator anim;
    private Vector3 direction;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);

        // Só segue se estiver dentro do raio
        if (dist <= followDistance)
        {
            // Direção até o player
            direction = (player.position - transform.position).normalized;

            // Movimento suave
            transform.position += direction * speed * Time.deltaTime;

            // Virar para o lado certo sem “rodar”
            if (direction.x > 0)   // indo para direita
                transform.localScale = new Vector3(1, 1, 1);
            else if (direction.x < 0) // indo para esquerda
                transform.localScale = new Vector3(-1, 1, 1);

            // Ativa a única animação
            anim.SetBool("Voando", true);
        }
        else
        {
            // fora do alcance — parar animação
            anim.SetBool("Voando", false);
        }
    }
}
