using UnityEngine;
using System.Collections;

public class EnemyRat : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float velocidadPatrulla = 2f;
    public float velocidadPersecucion = 4.5f;
    public float rangoVision = 6f;       
    public float rangoAtaque = 1.5f; 
    public float distanciaFrenado = 0.5f; 

    [Header("Combate")]
    public Transform puntoA;
    public Transform puntoB;
    
    public Transform puntoMordisco; 

    [Header("Combate y Vida")]
    public int vidaMaxima = 3;
    public float fuerzaEmpuje = 5f;
    public float tiempoInvulnerabilidad = 1f; 
    public int vidaActual;
    public bool esInvulnerable = false;
    public float tiempoDesaparecer = 1.5f;

    private enum Estado { Patrullando, Persiguiendo, Atacando, Muerto, Herido }
    private Estado estadoActual;

    private Transform objetivoActual;
    private Transform jugador;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    
    private bool puedeAtacar = true;
    private float velocidadActual = 0f; 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) jugador = playerObj.transform;

        vidaActual = vidaMaxima;
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

        Vector3 origenAtaque = (puntoMordisco != null) ? puntoMordisco.position : transform.position;
        float distanciaBoca = Vector2.Distance(origenAtaque, jugador.position);

        if (distanciaBoca < rangoAtaque && puedeAtacar)
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
        float direccion = (destino.x > transform.position.x) ? 1 : -1;
        velocidadActual = direccion * velocidadDeseada;

        if (Mathf.Abs(velocidadActual) > 0.1f)
        {
            bool mirarIzquierda = (velocidadActual < 0);
            
            spriteRenderer.flipX = mirarIzquierda;

            if (puntoMordisco != null)
            {
                Vector3 posBoca = puntoMordisco.localPosition;
                
                if (mirarIzquierda)
                    posBoca.x = -Mathf.Abs(posBoca.x); 
                else
                    posBoca.x = Mathf.Abs(posBoca.x); 

                puntoMordisco.localPosition = posBoca;
            }
        }
    }

    public void RecibirDaño(int daño)
    {
        if (estadoActual == Estado.Muerto) return;

        vidaActual -= daño;
        Debug.Log("🐀 Rata Herida! Vida restante: " + vidaActual);

        if (vidaActual <= 0) Morir();
        else StartCoroutine(HurtRoutine());
    }

    IEnumerator HurtRoutine()
    {
        estadoActual = Estado.Herido;
        velocidadActual = 0;
        animator.SetTrigger("Hurt");
        yield return new WaitForSeconds(0.3f); 
        if (estadoActual != Estado.Muerto) estadoActual = Estado.Persiguiendo;
    }

    void Morir()
    {
        estadoActual = Estado.Muerto;
        velocidadActual = 0;
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static; 
        GetComponent<Collider2D>().enabled = false; 
        animator.SetTrigger("Die");
        StartCoroutine(RutinaDestruir());
    }

    IEnumerator RutinaDestruir()
    {
        yield return new WaitForSeconds(tiempoDesaparecer);
        Destroy(gameObject);
    }

    IEnumerator RealizarAtaque()
    {
        estadoActual = Estado.Atacando;
        velocidadActual = 0; 
        puedeAtacar = false;
        
        animator.SetTrigger("Attack"); 
        yield return new WaitForSeconds(0.3f); 
        
        Vector3 origenAtaque = (puntoMordisco != null) ? puntoMordisco.position : transform.position;
        float distanciaReal = Vector2.Distance(origenAtaque, jugador.position);
        
        if (distanciaReal <= rangoAtaque)
        {
            PlayerController scriptJugador = jugador.GetComponent<PlayerController>();
            if (scriptJugador != null)
            {
                scriptJugador.RecibirDaño(1, transform.position);
            }
        }

        yield return new WaitForSeconds(0.5f); 
        estadoActual = Estado.Persiguiendo;
        yield return new WaitForSeconds(1.5f); 
        puedeAtacar = true;
    }

    private void OnDrawGizmos()
    {
        if (puntoMordisco != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(puntoMordisco.position, rangoAtaque);
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangoVision);

        if (objetivoActual != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, objetivoActual.position);
        }
    }
}