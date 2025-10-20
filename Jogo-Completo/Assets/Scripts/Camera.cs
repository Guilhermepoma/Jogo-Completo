using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    public Transform target;        // O player
    public float smoothSpeed = 0.15f; // Velocidade da suavização
    public Vector3 offset;          // Distância da câmera em relação ao player
    public bool limitCamera;        // Ativa ou desativa limites
    public Vector2 minLimit;        // Limite mínimo (x, y)
    public Vector2 maxLimit;        // Limite máximo (x, y)

    void LateUpdate()
    {
        if (target == null) return;

        // Calcula a posição desejada
        Vector3 desiredPosition = target.position + offset;

        // Suaviza o movimento
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Mantém o eixo Z fixo (2D)
        smoothedPosition.z = transform.position.z;

        // Aplica limites se estiver ativado
        if (limitCamera)
        {
            smoothedPosition.x = Mathf.Clamp(smoothedPosition.x, minLimit.x, maxLimit.x);
            smoothedPosition.y = Mathf.Clamp(smoothedPosition.y, minLimit.y, maxLimit.y);
        }

        transform.position = smoothedPosition;
    }
}