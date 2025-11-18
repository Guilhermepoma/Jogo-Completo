using UnityEngine;
using UnityEngine.SceneManagement;

public class Prox_nivel : MonoBehaviour
{
    [Header("Delay antes de mudar a fase (opcional)")]
    public float delay = 0f; // tempo em segundos antes de trocar a cena

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Chama a função para mudar a fase após o delay
            Invoke("LoadNextLevel", delay);
        }
    }

    private void LoadNextLevel()
    {
        // Pega o índice da cena atual
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        // Carrega a próxima cena
        int nextSceneIndex = currentSceneIndex + 1;

        // Verifica se existe próxima cena
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("Você chegou à última fase!");
        }
    }
}