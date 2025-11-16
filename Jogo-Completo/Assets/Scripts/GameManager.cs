using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int vidas = 5;

    // Referências dos corações na UI
    public GameObject[] coracoesUI;

    void Start()
    {
        AtualizarUI_Vidas();
    }

    public void PerderVidas(int vida)
    {
        Debug.Log("Vida: " + vidas);

        // REMOVEU A LINHA QUE CAUSAVA ERRO:
        // player.GetComponent<Player>().ReiniciarPosicao();

        vidas -= vida;

        // Atualiza a UI após perder vida
        AtualizarUI_Vidas();

        if (vidas <= 0)
        {
            Debug.Log("Player morreu (GameManager detectou)");
        }
    }

    // Atualiza visual dos corações
    public void AtualizarUI_Vidas()
    {
        for (int i = 0; i < coracoesUI.Length; i++)
        {
            bool estaAtivo = i < vidas;

            if (coracoesUI[i] != null)
            {
                coracoesUI[i].SetActive(estaAtivo);
            }
        }
    }
}