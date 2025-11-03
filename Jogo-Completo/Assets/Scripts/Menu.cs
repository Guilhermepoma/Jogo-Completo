using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
}