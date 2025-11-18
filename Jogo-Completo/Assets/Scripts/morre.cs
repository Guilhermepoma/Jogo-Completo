using UnityEngine;

public class morre : MonoBehaviour
{
    [Header("Destino do Teleporte")]
    public Transform teleportTarget; // posição para onde o player será teleportado

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica se o objeto que colidiu tem a tag "Player"
        if (other.CompareTag("Player"))
        {
            other.transform.position = teleportTarget.position;
        }
    }

    // Opcional: desenhar gizmo no editor para ver o destino
    private void OnDrawGizmos()
    {
        if (teleportTarget != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(teleportTarget.position, 0.2f);
            Gizmos.DrawLine(transform.position, teleportTarget.position);
        }
    }
}
