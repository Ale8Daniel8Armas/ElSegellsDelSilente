using UnityEngine;

public class DetectorMuerte : MonoBehaviour
{
    // Esta función se ejecuta automáticamente cuando algo entra en el trigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Comprobamos si el objeto que entró es el jugador.
        // Asegúrate de que tu jugador tenga el Tag "Player".
        if (collision.CompareTag("Player"))
        {
            // Busca el objeto GameManager_Obj y llama a su función MostrarGameOver
            FindObjectOfType<GameManager>().MostrarGameOver();
        }
    }
}