using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Tooltip("A referência para o Transform do jogador que a câmera deve seguir.")]
    public Transform playerTransform;

    [Tooltip("A velocidade com que a câmera segue o jogador. Use um valor baixo (ex: 0.1) para um movimento mais suave ou 1 para um acompanhamento instantâneo.")]
    [Range(0.01f, 1.0f)]
    public float smoothSpeed = 0.125f;

    [Tooltip("O deslocamento da câmera em relação ao jogador. Para um jogo 2D, o Z deve ser negativo (ex: -10) para que a câmera veja o cenário.")]
    public Vector3 offset = new Vector3(0, 0, -10);

    // É chamado em todos os frames, mas depois que todos os Updates foram concluídos.
    // Ideal para câmeras, pois garante que o jogador já se moveu naquele frame.
    void LateUpdate()
    {
        // Se a referência do jogador não foi definida, não faz nada.
        if (playerTransform == null)
        {
            Debug.LogWarning("Referência do Player Transform não definida no CameraController.");
            return;
        }

        // Posição desejada para a câmera (posição do jogador + deslocamento)
        Vector3 desiredPosition = playerTransform.position + offset;
        
        // Interpola suavemente da posição atual da câmera para a posição desejada
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        
        // Atualiza a posição da câmera
        transform.position = smoothedPosition;
    }
}