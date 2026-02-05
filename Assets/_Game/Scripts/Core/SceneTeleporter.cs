using UnityEngine;
using UnityEngine.SceneManagement; 

public class SceneTeleporter : MonoBehaviour
{
    [Header("Configuración del Viaje")]
    [Tooltip("El nombre EXACTO de la escena a la que vamos")]
    public string sceneToLoad;

    [Tooltip("El nombre del Punto de Aparición en la nueva escena donde debemos aparecer")]
    public string spawnPointID;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("🚪 ¡Portal tocado! Cambiando destino a: " + spawnPointID);
            GameManager.instance.nextSpawnPointID = spawnPointID;
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}