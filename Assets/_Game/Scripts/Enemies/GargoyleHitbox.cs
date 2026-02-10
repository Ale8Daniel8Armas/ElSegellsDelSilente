using UnityEngine;

public class GargoyleHitbox : MonoBehaviour
{
    [Header("Configuración")]
    public int dañoGolpe = 1; 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            
            if (player != null)
            {
                Vector2 origenDelDaño = transform.parent.position;
                
                player.RecibirDaño(dañoGolpe, origenDelDaño);
                
                // Opcional: Desactivar el hitbox inmediatamente para no golpear 2 veces en el mismo frame
                // gameObject.SetActive(false); 
            }
        }
    }
}