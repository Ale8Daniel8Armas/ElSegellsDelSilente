using UnityEngine;

public class ActivadorDialogo : MonoBehaviour
{
    [Header("Contenido")]
    [TextArea(3, 10)]
    public string[] lineasDeTexto;

    private bool jugadorCerca;
    private ManagerDialogos manager;

    void Start()
    {
        // Buscamos el manager una sola vez al inicio para ahorrar recursos
        manager = FindObjectOfType<ManagerDialogos>();
    }

    void Update()
    {
        // EL SEGURO: Solo inicia si el panel NO está activo
        if (jugadorCerca && Input.GetKeyDown(KeyCode.E))
        {
            if (!manager.panelDialogo.activeSelf)
            {
                manager.IniciarDialogo(lineasDeTexto);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = false;
        }
    }
}