using UnityEngine;
using System.Collections;

public class CantunaBossIA : MonoBehaviour
{
    [Header("--- Configuración General ---")]
    public Transform jugador; 
    public float vidaBoss = 500f;
    public bool peleaIniciada = false;

    [Header("--- Combate Ofensivo ---")]
    public Transform puntoAtaque;  
    public float radioGolpe = 2.0f; 
    public LayerMask capaJugador;   
    public int danoBoss = 1;        
    public float tiempoImpacto = 0.5f;

    [Header("--- Movimiento ---")]
    public float velocidad = 4f;
    public float fuerzaSalto = 10f;
    public float distanciaAtaque = 3.5f; 

    // Referencias
    private Animator anim;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Collider2D miCollider;
    
    // Estados internos
    private bool estaMuerto = false;
    private bool estaAtacando = false; 
    private bool enCooldown = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>(); 
        miCollider = GetComponent<Collider2D>();
        
        StartCoroutine(RutinaEntrada());
    }

    void Update()
    {
        if (estaMuerto || !peleaIniciada) return;

        MirarAlJugador();

        float distancia = Vector2.Distance(transform.position, jugador.position);

        if (estaAtacando || enCooldown) 
        {
            rb.linearVelocity = Vector2.zero; 
            anim.SetBool("Caminando", false);
            return; 
        }

        // --- LÓGICA DE PERSECUCIÓN ---
        if (distancia > distanciaAtaque)
        {
            anim.SetBool("Caminando", true);
            MoverseHaciaJugador();
        }
        else
        {
            anim.SetBool("Caminando", false); 
            StartCoroutine(LanzarAtaque());
        }
    }

    void SetIntangible(bool activo)
    {
        // 1. Desactivamos/Activamos las colisiones físicas
        if (miCollider != null) miCollider.enabled = !activo;

        // 2. Cambio Visual (Semitransparente)
        if (sr != null)
        {
            Color color = sr.color;
            color.a = activo ? 0.5f : 1.0f; 
            sr.color = color;
        }
    }
    IEnumerator RutinaEntrada()
    {
        yield return new WaitForSeconds(3.0f); 
        peleaIniciada = true;
        anim.SetBool("EmpezarPelea", true);
    }

    void MoverseHaciaJugador()
    {
        Vector2 target = new Vector2(jugador.position.x, rb.position.y);
        transform.position = Vector2.MoveTowards(transform.position, target, velocidad * Time.deltaTime);
    }

    void MirarAlJugador()
    {
        if (transform.position.x < jugador.position.x)
            transform.localScale = new Vector3(-1, 1, 1); 
        else
            transform.localScale = new Vector3(1, 1, 1);  
    }

    IEnumerator LanzarAtaque()
    {
        estaAtacando = true;
        enCooldown = true;

        ResetearTriggers();

        // 1. ELEGIR ATAQUE
        int dado = Random.Range(0, 100);
        
        if (dado < 35) 
        {
            anim.SetTrigger("Ataque_Golpe");
            yield return StartCoroutine(ProcesarGolpeNormal()); 
        }
        else if (dado < 50) 
        {
            anim.SetTrigger("Ataque_Fuego");
            yield return StartCoroutine(ProcesarGolpeNormal());
        }
        else if (dado < 85) // Latigazo
        {
            SetIntangible(true); 
            
            anim.SetTrigger("Latigazo");
            yield return new WaitForSeconds(1.0f); 
            
            SetIntangible(false); 
        }
        else // Salto
        {
            SetIntangible(true);
            
            anim.SetTrigger("Salto");
            rb.AddForce(Vector2.up * fuerzaSalto, ForceMode2D.Impulse);
            
            float direccionX = (jugador.position.x - transform.position.x) > 0 ? 1 : -1;
            rb.linearVelocity = new Vector2(direccionX * (velocidad * 1.5f), rb.linearVelocity.y);

            yield return new WaitForSeconds(1.2f);

            GolpearJugadorEnArea();

            SetIntangible(false); 
            rb.linearVelocity = Vector2.zero; 
        }

        estaAtacando = false; 
        yield return new WaitForSeconds(0.5f); 
        enCooldown = false;
    }

    IEnumerator ProcesarGolpeNormal()
    {
        yield return new WaitForSeconds(tiempoImpacto); 
        GolpearJugadorEnArea();
        yield return new WaitForSeconds(1.2f - tiempoImpacto); 
    }

    void GolpearJugadorEnArea()
    {
        if (puntoAtaque != null)
        {
            Collider2D golpe = Physics2D.OverlapCircle(puntoAtaque.position, radioGolpe, capaJugador);
            if (golpe != null)
            {
                PlayerController playerScript = golpe.GetComponent<PlayerController>();
                if (playerScript != null)
                {
                    playerScript.RecibirDaño(danoBoss, transform.position);
                }
            }
        }
    }

    void ResetearTriggers()
    {
        anim.ResetTrigger("Ataque_Golpe");
        anim.ResetTrigger("Ataque_Fuego");
        anim.ResetTrigger("Latigazo");
        anim.ResetTrigger("Salto");
        anim.ResetTrigger("Herido");
    }

    public void RecibirDaño(float daño)
    {
        if (estaMuerto) return;
        
        if (miCollider != null && !miCollider.enabled) return; 

        vidaBoss -= daño;
        Debug.Log("Cantuña Vida: " + vidaBoss);

        if (vidaBoss > 0 && Random.value > 0.9f) 
        {
            anim.SetTrigger("Herido");
            StopAllCoroutines(); 
            SetIntangible(false); 
            estaAtacando = false;
            enCooldown = false;
        }

        if (vidaBoss <= 0) Morir();
    }

    void Morir()
    {
        estaMuerto = true;
        SetIntangible(false); 
        anim.SetTrigger("Muerto");
        rb.linearVelocity = Vector2.zero;
        rb.isKinematic = true; 
        GetComponent<Collider2D>().enabled = false;
        
        StartCoroutine(DestruirDespuesDeAnimacion());
    }

    IEnumerator DestruirDespuesDeAnimacion()
    {
        yield return new WaitForSeconds(2.0f); 
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        if (puntoAtaque != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(puntoAtaque.position, radioGolpe);
        }
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, distanciaAtaque);
    }
}