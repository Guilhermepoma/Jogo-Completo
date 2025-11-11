using UnityEngine;

public class PlayerHitbox : MonoBehaviour
{
    private bool ativa = false;

    public void AtivarHitbox()
    {
        ativa = true;
        gameObject.SetActive(true);
        Invoke(nameof(DesativarHitbox), 0.2f); // hitbox dura 0.2 segundos
    }

    void DesativarHitbox()
    {
        ativa = false;
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!ativa) return;

        if (collision.CompareTag("Inimigo"))
        {
            Debug.Log("Inimigo atingido e destruído!");
            Destroy(collision.gameObject); // 💥 inimigo some na hora
        }
    }
}