using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI; // Adicione este namespace para trabalhar com elementos UI

public class GameManager : MonoBehaviour
{
    public int vidas = 5;

    public TextMeshProUGUI textPontos;

    // 1. Adicione um array para segurar as referências aos corações da UI
    // Você arrastará os GameObjects dos corações para este array no Inspector do Unity
    public GameObject[] coracoesUI;

    // Opcional: Adicionar um método Start para garantir que a UI comece corretamente
    void Start()
    {
        AtualizarUI_Vidas();
    }

    public void PerderVidas(int vida)
    {
        Debug.Log("Vida: " + vidas);
        GameObject player = GameObject.FindWithTag("Player");
        player.GetComponent<Player>().ReiniciarPosicao();

        vidas -= vida;

        // 2. Chame o método de atualização da UI após mudar as vidas
        AtualizarUI_Vidas();

        if (vidas <= 0)
        {
            Time.timeScale = 0;
            Debug.Log("Perdeu ó hahahahah");
        }
    }

    // 3. Método para atualizar a UI dos corações
    public void AtualizarUI_Vidas()
    {
        // Certifica-se de que o número de vidas e coraçõesUI é consistente
        // Se as vidas forem 5, o loop deve ir de 0 a 4.

        // Este loop habilita/desabilita cada coração.
        for (int i = 0; i < coracoesUI.Length; i++)
        {
            // Se o índice for menor que o número atual de vidas, o coração deve estar ativo.
            // Ex: Se vidas = 3, os corações nos índices 0, 1 e 2 estarão ativos.
            // coracoesUI[i].SetActive(i < vidas);

            // Alternativamente, se quiser desabilitar os corações de trás para frente (o último coração some primeiro):
            // O coração no índice 'i' deve estar ativo se 'i' for menor que 'vidas'.

            bool estaAtivo = i < vidas;

            // Desabilita/habilita o GameObject do coração.
            if (coracoesUI[i] != null)
            {
                coracoesUI[i].SetActive(estaAtivo);
            }
        }
    }
}