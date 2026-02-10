using UnityEngine;
using System.Collections;

public class JefeArakocraPro : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float velocidadVuelo = 3f;
    public float velocidadPicado = 12f;
    public float rangoDeteccion = 10f;
    public float rangoAtaque = 3.5f;

    [Header("Zona de Patrulla")]
    public float limiteMovimientoX = 7f; // Cuánto se aleja de su punto inicial
    private float xInicial;
    private float yInicial;

    [Header("Tiempos de Ataque")]
    public float esperaEntreAtaques = 3f;
    private float tiempoProximoAtaque = 0f;

    [Header("Ajustes de Sprite")]
    public float escalaBase = 6f; // La escala que usas en el Inspector

    private Transform jugador;
    private Animator anim;
    private bool atacando = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) jugador = p.transform;

        // Guardamos su posición de origen para que sepa a dónde volver
        xInicial = transform.position.x;
        yInicial = transform.position.y;
    }

    void Update()
    {
        if (jugador == null || atacando) return;

        float dist = Vector2.Distance(transform.position, jugador.position);

        // ¿El jugador está dentro de su territorio?
        bool enZona = Mathf.Abs(jugador.position.x - xInicial) < limiteMovimientoX;

        if (dist < rangoDeteccion && enZona)
        {
            SeguirJugador();

            // Si está cerca y ya pasó el tiempo de espera, ataca
            if (dist <= rangoAtaque && Time.time >= tiempoProximoAtaque)
            {
                StartCoroutine(SecuenciaAtaque());
            }
        }
        else
        {
            RegresarAlSitio();
        }
    }

    void SeguirJugador()
    {
        // Se mueve en X hacia Silente pero mantiene su altura de vuelo
        Vector3 objetivo = new Vector3(jugador.position.x, yInicial, 0);
        transform.position = Vector3.MoveTowards(transform.position, objetivo, velocidadVuelo * Time.deltaTime);
        GirarHacia(jugador.position.x);
    }

    void RegresarAlSitio()
    {
        Vector3 origen = new Vector3(xInicial, yInicial, 0);
        transform.position = Vector3.MoveTowards(transform.position, origen, velocidadVuelo * Time.deltaTime);
        GirarHacia(xInicial);
    }

    IEnumerator SecuenciaAtaque()
    {
        atacando = true;

        // 1. Elegir ataque al azar (1 o 2)
        int numAtk = Random.Range(1, 3);
        anim.SetTrigger("Attack" + numAtk);

        // 2. Bajada (Picado) hacia Silente
        Vector3 puntoGolpe = new Vector3(jugador.position.x, jugador.position.y + 0.5f, 0);
        while (Vector3.Distance(transform.position, puntoGolpe) > 0.5f)
        {
            transform.position = Vector3.MoveTowards(transform.position, puntoGolpe, velocidadPicado * Time.deltaTime);
            yield return null;
        }

        yield return new WaitForSeconds(1f); // Tiempo para que se vea el golpe

        // 3. Subida (Regreso al aire)
        Vector3 puntoCielo = new Vector3(transform.position.x, yInicial, 0);
        while (Vector3.Distance(transform.position, puntoCielo) > 0.5f)
        {
            transform.position = Vector3.MoveTowards(transform.position, puntoCielo, velocidadVuelo * Time.deltaTime);
            yield return null;
        }

        tiempoProximoAtaque = Time.time + esperaEntreAtaques;
        atacando = false;
    }

    void GirarHacia(float xObjetivo)
    {
        if (xObjetivo > transform.position.x)
            transform.localScale = new Vector3(-escalaBase, 5, 1);
        else
            transform.localScale = new Vector3(escalaBase, 5, 1);
    }
}