using UnityEngine;
using UnityEngine.UI;

public class VidaArakocra : MonoBehaviour
{
    [Header("Estadísticas")]
    public float vidaMaxima = 100f;
    private float vidaActual;

    [Header("UI y Visibilidad")]
    public GameObject objetoBarraVida; // Arrastra el objeto 'Slider' aquí
    public Slider sliderComponente;     // Arrastra el mismo 'Slider' aquí
    public float distanciaParaMostrar = 12f; // Rango para que aparezca la barra

    private Transform jugador;

    void Start()
    {
        vidaActual = vidaMaxima;
        jugador = GameObject.FindGameObjectWithTag("Player").transform;

        if (sliderComponente != null)
        {
            sliderComponente.maxValue = vidaMaxima;
            sliderComponente.value = vidaActual;
        }

        // Empezamos con la barra oculta
        if (objetoBarraVida != null) objetoBarraVida.SetActive(false);
    }

    void Update()
    {
        if (jugador == null || objetoBarraVida == null) return;

        // Calculamos la distancia entre Silente y el Arakocra
        float distancia = Vector2.Distance(transform.position, jugador.position);

        // Si está cerca, mostramos la barra; si se aleja, la ocultamos
        if (distancia <= distanciaParaMostrar)
        {
            objetoBarraVida.SetActive(true);
        }
        else
        {
            objetoBarraVida.SetActive(false);
        }
    }

    public void RecibirDanio(float cantidad)
    {
        vidaActual -= cantidad;
        if (sliderComponente != null) sliderComponente.value = vidaActual;

        if (vidaActual <= 0)
        {
            Morir();
        }
    }

    void Morir()
    {
        // Al morir, ocultamos la barra definitivamente
        if (objetoBarraVida != null) objetoBarraVida.SetActive(false);
        Destroy(gameObject);
    }
}