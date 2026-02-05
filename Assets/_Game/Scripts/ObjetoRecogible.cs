using UnityEngine;

public class ObjetoRecogible : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Esto busca que lo que toque la poci�n tenga el Tag "Player"
        if (other.CompareTag("Player"))
        {
            Debug.Log("Objeto recogido por el jugador");

            // Aqu� podr�as avisar al inventario, pero por ahora...
            // �Pum! El objeto desaparece de la escena.
            Destroy(gameObject);
        }
    }
}