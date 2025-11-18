using UnityEngine;
using UnityEngine.SceneManagement;

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
    public float ascendSpeed = 5f;
    public float ascendHeight = 4f;

    private Rigidbody2D rig;
    private Transform target;
    private bool diving = false;
    private bool ascending = false;
    private float direction = 1f;
    private float dirTimer = 0f;
    private float diveTimer = 0f;
    private bool morreu = false;

    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            target = playerObj.transform;
    }

    void Update()
    {
        if ((vida <= 0 && !morreu) || target == null) return;

        dirTimer += Time.deltaTime;
        diveTimer += Time.deltaTime;

        if (!diving && !ascending)
        {
            rig.linearVelocity = new Vector2(speed * direction, 0f);

            if (dirTimer >= changeDirTime)
            {
                direction *= -1f;
                dirTimer = 0f;
            }

            Vector3 pos = transform.position;
            pos.y = hoverHeight;
            transform.position = pos;

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

        if (vida <= 0 && !morreu)
        {
            morreu = true;
            Debug.Log("Malgo morreu!");

            // faz ele sumir imediatamente
            gameObject.SetActive(false);

            // inicia contagem de 2 segundos para voltar ao menu
            Invoke("VoltarMenu", 2f);
        }
    }

    private void VoltarMenu()
    {
        SceneManager.LoadScene("Menu"); // substitua pelo nome exato da cena do menu
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

    public void LancarBomba()
    {
        ascending = true;
        diving = false;
    }
}
