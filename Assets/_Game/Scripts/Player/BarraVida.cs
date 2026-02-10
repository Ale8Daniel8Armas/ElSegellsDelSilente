using UnityEngine;
using UnityEngine.UI;

public class BarraVida : MonoBehaviour
{
    public Image rellenoBarraVida;
    public PlayerController jugador; // referencia directa

    void Update()
    {
        if (jugador == null) return;

        float porcentaje = (float)jugador.vidaActual / jugador.vidaMaxima;
        rellenoBarraVida.fillAmount = porcentaje;
    }
}