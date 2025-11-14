using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public Transform player;

    private Vector3 originalOffset;

    void Start()
    {
        originalOffset = transform.position - player.position;
    }

    void LateUpdate()
    {
        float direction = player.localScale.x >= 0 ? 1f : -1f;
        Vector3 flippedOffset = new Vector3(originalOffset.x * direction, originalOffset.y, originalOffset.z);
        transform.position = player.position + flippedOffset;
    }
}