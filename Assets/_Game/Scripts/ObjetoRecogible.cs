using UnityEngine;

public class ObjetoRecogible : MonoBehaviour
{
    public Sprite iconoParaInventario; // Arrastra aquí el sprite de la poción

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Busca el inventario en el GameManager y trata de agregar el item
            Inventario inv = FindObjectOfType<Inventario>();

            if (inv != null)
            {
                bool exito = inv.AgregarItem(iconoParaInventario);
                if (exito)
                {
                    Debug.Log("Item guardado en inventario");
                    Destroy(gameObject);
                }
            }
        }
    }
}