using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    // Variáveis de Configuração no Inspector
    // -------------------------------------
    public Transform target;        // O player (alvo a ser seguido)
    public float smoothSpeed = 0.15f; // Velocidade da suavização (quanto menor, mais suave/lenta)
    public Vector3 offset;          // Distância da câmera em relação ao player (ex: (0, 0, -10))

    [Header("Limites de Câmera")] // Ajuda a organizar no Inspector
    public bool limitCamera;        // Ativa ou desativa a aplicação dos limites
    public Vector2 minLimit;        // Limite mínimo (canto inferior esquerdo) para X e Y
    public Vector2 maxLimit;        // Limite máximo (canto superior direito) para X e Y

    void LateUpdate()
    {
        // Garante que a câmera só siga se houver um alvo (target)
        if (target == null) return;

        // 1. Calcula a posição ideal que a câmera *deveria* estar
        Vector3 desiredPosition = target.position + offset;

        // 2. Suaviza o movimento da posição atual para a posição desejada
        // O Lerp cria um movimento interpolado (suave)
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // 3. Mantém o eixo Z da câmera fixo (essencial para 2D)
        // Isso impede que a câmera se mova na profundidade Z.
        smoothedPosition.z = transform.position.z;

        // 4. Aplica os limites de X e Y se a opção estiver ativada
        if (limitCamera)
        {
            // O Mathf.Clamp garante que o valor esteja sempre entre o mínimo e o máximo.

            // Limita a posição X: não pode ser menor que minLimit.x nem maior que maxLimit.x
            smoothedPosition.x = Mathf.Clamp(smoothedPosition.x, minLimit.x, maxLimit.x);

            // Limita a posição Y: não pode ser menor que minLimit.y nem maior que maxLimit.y
            smoothedPosition.y = Mathf.Clamp(smoothedPosition.y, minLimit.y, maxLimit.y);
        }

        // 5. Aplica a posição final à câmera
        transform.position = smoothedPosition;
    }
}