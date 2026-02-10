using UnityEngine;

public class ControladorInventario : MonoBehaviour
{
    [Header("Arrastra aquí el PanelInventario")]
    public GameObject panelInventario;
    private bool estaAbierto = false;

    void Update()
    {
        // Si el jugador presiona la tecla 'I'
        if (Input.GetKeyDown(KeyCode.I))
        {
            AlternarInventario();
        }
    }

    void AlternarInventario()
    {
        // Cambia el estado de abierto a cerrado y viceversa
        estaAbierto = !estaAbierto;

        // Activa o desactiva el panel en el juego
        panelInventario.SetActive(estaAbierto);

        // Opcional: Pausar el juego y liberar el mouse
        if (estaAbierto)
        {
            Time.timeScale = 0f; // Pausa el tiempo
            Cursor.lockState = CursorLockMode.None; // Desbloquea el cursor
            Cursor.visible = true; // Muestra el cursor
        }
        else
        {
            Time.timeScale = 1f; // Reanuda el tiempo
            Cursor.lockState = CursorLockMode.Locked; // Bloquea el cursor (ajústalo a tu juego)
            Cursor.visible = false; // Oculta el cursor (ajústalo a tu juego)
        }
    }
}