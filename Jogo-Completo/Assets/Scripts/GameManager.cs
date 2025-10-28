using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class GameManager : MonoBehaviour
{
    public int pontos = 0;
    public int vidas = 3;

    public TextMeshProUGUI textPontos;

    public void AddPontos(int qtd)
    {
        pontos += qtd;
        Debug.Log("pontos: " + pontos);
        if (pontos < 0) { pontos = 0; }

        textPontos.text = "Pontos: " + pontos;
    }

    public void PerderVidas(int vida)
    {
        Debug.Log("Vida: " + vidas);
        GameObject player = GameObject.FindWithTag("Player");
        player.GetComponent<Player>().ReiniciarPosicao();
        vidas -= vida;
        if (vidas <= 0)
        {
            Time.timeScale = 0;
            Debug.Log("Perdeu ó hahahahah");
        }
    }

}