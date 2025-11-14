using UnityEngine;

public class HitboxController : MonoBehaviour
{
    public Transform player;
    public float activeTime = 0.2f; // tempo ativa
    private Vector3 originalOffset;

    private bool isActive = false;

    void Start()
    {
        // salva a posição inicial da hitbox em relação ao player
        originalOffset = transform.position - player.position;

        // começa desativado
        gameObject.SetActive(false);
    }

    void LateUpdate()
    {
        if (player == null) return;

        // direção do player (direita = 1, esquerda = -1)
        float direction = player.localScale.x >= 0 ? 1f : -1f;

        // aplica o flip no offset
        Vector3 flippedOffset = new Vector3(originalOffset.x * direction, originalOffset.y, originalOffset.z);

        // segue o player
        transform.position = player.position + flippedOffset;
    }

    void Update()
    {
        // ativa quando apertar J
        if (Input.GetKeyDown(KeyCode.J) && !isActive)
        {
            StartCoroutine(Activate());
        }
    }

    private System.Collections.IEnumerator Activate()
    {
        isActive = true;
        gameObject.SetActive(true);

        yield return new WaitForSeconds(activeTime);

        gameObject.SetActive(false);
        isActive = false;
    }
}
