using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public string nextSpawnPointID;
    public GameObject pantallaGameOver;

    void Awake()
    {
        // Patrón Singleton básico: asegurar que solo haya un GameManager
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Esta funci�n se llamar� cuando el jugador toque la Zona de Muerte
    public void MostrarGameOver()
    {
        pantallaGameOver.SetActive(true); // Activa el panel
        Time.timeScale = 0f; // Pausa el juego
    }

    // Funci�n para el bot�n REINTENTAR
    public void ReintentarNivel()
    {
        Time.timeScale = 1f; // Reanuda el tiempo antes de recargar
        // Recarga la escena que est� activa actualmente
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Funci�n para el bot�n SALIR
    public void SalirDelJuego()
    {
        Debug.Log("Saliendo del juego..."); // Mensaje para la consola del editor
        Application.Quit(); // Cierra la aplicaci�n (funciona en el juego compilado)
    }
}