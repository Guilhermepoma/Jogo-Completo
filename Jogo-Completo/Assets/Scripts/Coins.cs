using UnityEngine;

public class Coins : MonoBehaviour
{
    public GameManager gameManager;
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            gameManager.AddPontos(1);
            Destroy(gameObject);
        }
    }
}
