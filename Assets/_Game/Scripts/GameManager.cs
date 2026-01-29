using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para recargar la escena

public class GameManager : MonoBehaviour
{
    // Arrastraremos aquí nuestro panel "PantallaGameOver" desde el Inspector
    public GameObject pantallaGameOver;

    // Esta función se llamará cuando el jugador toque la Zona de Muerte
    public void MostrarGameOver()
    {
        pantallaGameOver.SetActive(true); // Activa el panel
        Time.timeScale = 0f; // Pausa el juego
    }

    // Función para el botón REINTENTAR
    public void ReintentarNivel()
    {
        Time.timeScale = 1f; // Reanuda el tiempo antes de recargar
        // Recarga la escena que está activa actualmente
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Función para el botón SALIR
    public void SalirDelJuego()
    {
        Debug.Log("Saliendo del juego..."); // Mensaje para la consola del editor
        Application.Quit(); // Cierra la aplicación (funciona en el juego compilado)
    }
}
