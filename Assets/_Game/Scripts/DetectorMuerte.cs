using UnityEngine;

public class DetectorMuerte : MonoBehaviour
{
    // Esta funci�n se ejecuta autom�ticamente cuando algo entra en el trigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Comprobamos si el objeto que entr� es el jugador.
        // Aseg�rate de que tu jugador tenga el Tag "Player".
        if (collision.CompareTag("Player"))
        {
            // Busca el objeto GameManager_Obj y llama a su funci�n MostrarGameOver
            FindObjectOfType<GameManager>().MostrarGameOver();
        }
    }
}