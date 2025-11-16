using UnityEngine;

public class HitboxDano : MonoBehaviour
{
    public int dano = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Dá dano no player, usando a classe PlayerRefatorado
            other.GetComponent<PlayerRefatorado>().TomarDano(dano);
        }
    }
}