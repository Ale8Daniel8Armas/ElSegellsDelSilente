using UnityEngine;
using System.Collections;

public class EnemyRat : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float velocidadPatrulla = 2f;
    public float velocidadPersecucion = 4.5f;
    public float rangoVision = 6f;       
    public float rangoAtaque = 3.3f;    
    public float distanciaFrenado = 0.5f; 

    [Header("Combate")]
    public int vida = 2;
    public Transform puntoA;
    public Transform puntoB;

    // ESTADOS
    private enum Estado { Patrullando, Persiguiendo, Atacando, Muerto, Herido }
    private Estado estadoActual;

    // REFERENCIAS INTERNAS
    private Transform objetivoActual;
    private Transform jugador;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    
    // VARIABLES DE CONTROL
    private bool puedeAtacar = true;
    private float velocidadActual = 0f; 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) jugador = playerObj.transform;

        estadoActual = Estado.Patrullando;
        objetivoActual = puntoA;
    }

    void Update() 
    {
        if (estadoActual == Estado.Muerto || jugador == null) return;

        switch (estadoActual)
        {
            case Estado.Patrullando:
                LogicaPatrulla();
                break;
            case Estado.Persiguiendo:
                LogicaPersecucion();
                break;
        }
    }

    void FixedUpdate()
    {
        if (estadoActual == Estado.Muerto) return;

        if (estadoActual == Estado.Atacando || estadoActual == Estado.Herido)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        rb.linearVelocity = new Vector2(velocidadActual, rb.linearVelocity.y);

        animator.SetFloat("Speed", Mathf.Abs(velocidadActual));
    }

    // --- LÓGICA DEL CEREBRO ---

    void LogicaPatrulla()
    {
        float distanciaPunto = Vector2.Distance(transform.position, objetivoActual.position);

        if (distanciaPunto < distanciaFrenado)
        {
            objetivoActual = (objetivoActual == puntoA) ? puntoB : puntoA;
        }

        MoverseHacia(objetivoActual.position, velocidadPatrulla);

        float distanciaJugador = Vector2.Distance(transform.position, jugador.position);
        if (distanciaJugador < rangoVision)
        {
            estadoActual = Estado.Persiguiendo;
        }
    }

    void LogicaPersecucion()
    {
        float distanciaJugador = Vector2.Distance(transform.position, jugador.position);

        if (distanciaJugador > rangoVision * 1.5f)
        {
            estadoActual = Estado.Patrullando;
            objetivoActual = puntoA; 
            return;
        }

        if (distanciaJugador < rangoAtaque && puedeAtacar)
        {
            StartCoroutine(RealizarAtaque());
        }
        else
        {
            MoverseHacia(jugador.position, velocidadPersecucion);
        }
    }

    void MoverseHacia(Vector3 destino, float velocidadDeseada)
    {
        // Calculamos dirección (-1 o +1)
        float direccion = (destino.x > transform.position.x) ? 1 : -1;
        
        velocidadActual = direccion * velocidadDeseada;

        if (Mathf.Abs(velocidadActual) > 0.1f)
        {
            spriteRenderer.flipX = (velocidadActual < 0);
        }
    }

    // --- COMBATE ---

    IEnumerator RealizarAtaque()
    {
        estadoActual = Estado.Atacando;
        velocidadActual = 0; 
        puedeAtacar = false;
        
        animator.SetTrigger("Attack");

        yield return new WaitForSeconds(0.6f);

        estadoActual = Estado.Persiguiendo;
        
        yield return new WaitForSeconds(1.5f); 
        puedeAtacar = true;
    }

    public void RecibirDaño(int daño)
    {
        if (estadoActual == Estado.Muerto) return;

        vida -= daño;
        if (vida <= 0) Morir();
        else StartCoroutine(HurtRoutine());
    }

    IEnumerator HurtRoutine()
    {
        Estado estadoAnterior = estadoActual;
        estadoActual = Estado.Herido;
        velocidadActual = 0;
        animator.SetTrigger("Hurt");

        yield return new WaitForSeconds(0.3f); 

        if (estadoActual != Estado.Muerto) 
            estadoActual = Estado.Persiguiendo;
    }

    void Morir()
    {
        estadoActual = Estado.Muerto;
        velocidadActual = 0;
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static; 
        GetComponent<Collider2D>().enabled = false; 
        animator.SetTrigger("Die");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangoAtaque);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangoVision);

        if (objetivoActual != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, objetivoActual.position);
        }
    }
}