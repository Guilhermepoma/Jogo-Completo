using UnityEngine;

public class Inimigo_malgo : MonoBehaviour, IDano
{
    [Header("Vida do Inimigo")]
    public int vida = 10;

    [Header("Ataque do Player (somente visual do gizmo)")]
    public Transform attackOrigin;
    public float radiusAttack = 1.2f;

    [Header("Movimento")]
    public float speed = 3f;
    public float hoverHeight = 3f;
    public float changeDirTime = 2f;
    public float diveSpeed = 7f;
    public float diveCooldown = 5f;
    public float ascendSpeed = 5f; // velocidade de subida após ataque
    public float ascendHeight = 4f; // altura que sobe após atacar

    private Rigidbody2D rig;
    private Transform target; // agora é referência direta do player
    private bool diving = false;
    private bool ascending = false;
    private float direction = 1f;
    private float dirTimer = 0f;
    private float diveTimer = 0f;

    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            target = playerObj.transform;
    }

    void Update()
    {
        if (vida <= 0 || target == null) return;

        dirTimer += Time.deltaTime;
        diveTimer += Time.deltaTime;

        if (!diving && !ascending)
        {
            // patrulha horizontal
            rig.linearVelocity = new Vector2(speed * direction, 0f);

            // muda direção periodicamente
            if (dirTimer >= changeDirTime)
            {
                direction *= -1f;
                dirTimer = 0f;
            }

            // mantém altura
            Vector3 pos = transform.position;
            pos.y = hoverHeight;
            transform.position = pos;

            // verifica se pode mergulhar
            if (diveTimer >= diveCooldown)
            {
                float distanceX = Mathf.Abs(target.position.x - transform.position.x);
                if (distanceX < 5f)
                {
                    diving = true;
                    diveTimer = 0f;
                }
            }
        }
        else if (diving)
        {
            // mergulho em direção ao player
            Vector2 dir = (target.position - transform.position).normalized;
            rig.linearVelocity = dir * diveSpeed;

            if (transform.position.y <= target.position.y)
            {
                diving = false;
                ascending = true;
            }
        }
        else if (ascending)
        {
            // sobe após atacar
            rig.linearVelocity = new Vector2(0f, ascendSpeed);

            if (transform.position.y >= ascendHeight)
            {
                ascending = false;
                rig.linearVelocity = Vector2.zero;
            }
        }
    }

    public void TakeHit()
    {
        vida--;
        Debug.Log("Malgo recebeu dano! Vida: " + vida);

        if (vida <= 0)
        {
            Destroy(gameObject);
            Debug.Log("Malgo morreu!");
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackOrigin == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(
            attackOrigin.position + transform.right * (radiusAttack / 2),
            new Vector3(radiusAttack, radiusAttack / 2, 0.1f)
        );
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            var playerScript = collision.gameObject.GetComponent<PlayerRefatorado>();
            if (playerScript != null)
            {
                playerScript.TomarDano(1);
            }
        }
    }

    // Função para ser chamada quando lançar a bomba
    public void LancarBomba()
    {
        // lógica da bomba aqui (instancia prefab, etc.)

        // após lançar, sobe automaticamente
        ascending = true;
        diving = false;
    }
}