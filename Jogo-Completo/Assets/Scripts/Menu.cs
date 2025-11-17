using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void ActiveConfig(GameObject go)
    {
        go.SetActive(true);
    }

    public void DisableConfig(GameObject go)
    {
        go.SetActive(false);
    }

    public void ActivePause(GameObject go)
    {
        Time.timeScale = 0;
        go.SetActive(true);
    }

    public void DisablePause(GameObject go)
    {
        Time.timeScale = 1;
        go.SetActive(false);
    }

    public void MenuPause()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    // Função para sair do jogo
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Fecha o jogo no editor
#else
        Application.Quit(); // Fecha o jogo na build
#endif
    }
}