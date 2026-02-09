using UnityEngine;

public class ObjetoRecogible : MonoBehaviour
{
    [Header("Configuración del Objeto")]
    public Sprite iconoParaInventario; // Arrastra aquí el sprite del objeto
    public string nombreObjeto;        // Escribe el nombre (ej: "RunaAncestral")

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verificamos que sea el jugador quien toca el objeto
        if (other.CompareTag("Player"))
        {
            // Busca el inventario en el GameManager
            Inventario inv = FindObjectOfType<Inventario>();

            if (inv != null)
            {
                // Ahora enviamos el ICONO y el NOMBRE al inventario
                bool exito = inv.AgregarItem(iconoParaInventario, nombreObjeto);

                if (exito)
                {
                    Debug.Log("Objeto '" + nombreObjeto + "' guardado en el inventario.");
                    Destroy(gameObject); // El objeto desaparece de la escena
                }
            }
        }
    }
}