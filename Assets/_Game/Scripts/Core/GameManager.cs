using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public string nextSpawnPointID;
    public GameObject pantallaGameOver;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            // DontDestroyOnLoad(gameObject); // OJO: Lee la nota de abajo sobre esto
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void MostrarGameOver()
    {
        if (pantallaGameOver != null)
        {
            pantallaGameOver.SetActive(true);
            Time.timeScale = 0f; // Pausa el juego
        }
    }

    public void ReintentarNivel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // --- NUEVA FUNCIÓN PARA TU BOTÓN DE SALIR AL MENÚ ---
    public void IrAlMenu()
    {
        Time.timeScale = 1f; // ¡Vital! Si no, el menú aparecerá congelado
        SceneManager.LoadScene("Menu"); // Asegúrate de que tu escena se llame exactamente "Menu"
    }

    // Esta la puedes dejar para el botón de "Salir" del menú principal
    public void SalirDelJuego()
    {
        Debug.Log("Cerrando aplicación...");
        Application.Quit();
    }
}