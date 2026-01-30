using UnityEngine;

public class CajaLoot : MonoBehaviour
{
    public int vida = 3; // Golpes para romperla
    public GameObject[] posiblesPremios; // Aquí pondremos tus 3 pociones

    // Esta función detecta cuando el ataque del jugador la toca
    private void OnTriggerEnter2D(Collider2D otro)
    {
        // El objeto de ataque del jugador debe tener el Tag "Ataque"
        if (otro.CompareTag("Ataque"))
        {
            RecibirDano();
        }
    }

    void RecibirDano()
    {
        vida--;
        Debug.Log("Caja golpeada! Vida: " + vida);

        if (vida <= 0)
        {
            SoltarPremio();
        }
    }

    void SoltarPremio()
    {
        // Elige un premio al azar de la lista
        int indice = Random.Range(0, posiblesPremios.Length);

        // Crea la poción en la misma posición de la caja
        Instantiate(posiblesPremios[indice], transform.position, Quaternion.identity);

        // Destruye la caja
        Destroy(gameObject);
    }
}