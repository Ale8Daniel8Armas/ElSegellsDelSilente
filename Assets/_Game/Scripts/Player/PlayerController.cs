using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Configuración")]
    public float velocidadMovimiento = 15f;
    public float fuerzaSalto = 10f;

    [Header("Referencias")]
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private bool estaDefendiendo = false;

    private float movimientoHorizontal;

    private bool estaEnSuelo = true;

    private bool estaAtacando = false;

    [Header("Combate y Vida")]
    public int vidaMaxima = 5;
    public float fuerzaEmpuje = 5f; 
    public float tiempoInvulnerabilidad = 1f; 

    private int vidaActual;
    private bool esInvulnerable = false;

    [Header("Combate Ofensivo")]
    public Transform puntoAtaque; 
    public float rangoLatigo = 2.5f; 
    public LayerMask capaEnemigos;   
    public int dañoLatigo = 1;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        vidaActual = vidaMaxima;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // 1. INPUT (Teclado)
        movimientoHorizontal = Input.GetAxisRaw("Horizontal");

        // 2. ANIMACIÓN de saltar
        if (Input.GetButtonDown("Jump") && estaEnSuelo && !estaDefendiendo)
        {
            rb.AddForce(new Vector2(0f, fuerzaSalto), ForceMode2D.Impulse);
            estaEnSuelo = false; 
        }

        // 3. ANIMACIÓNES
        animator.SetBool("defensa", estaDefendiendo);
        animator.SetFloat("Speed", Mathf.Abs(movimientoHorizontal));
        animator.SetBool("enSuelo", estaEnSuelo);
        animator.SetFloat("velocidadY", rb.linearVelocity.y);

        // 4. GIRAR PERSONAJE (Flip) Y AJUSTAR HITBOX
        if (movimientoHorizontal > 0)
        {
            spriteRenderer.flipX = false;
            Vector3 posicionPunto = puntoAtaque.localPosition;
            posicionPunto.x = Mathf.Abs(posicionPunto.x); 
            puntoAtaque.localPosition = posicionPunto;
        }
        else if (movimientoHorizontal < 0)
        {
            spriteRenderer.flipX = true;
            Vector3 posicionPunto = puntoAtaque.localPosition;
            posicionPunto.x = -Mathf.Abs(posicionPunto.x);
            puntoAtaque.localPosition = posicionPunto;
        }

        // 5. DEFENSA

        if (Input.GetKey(KeyCode.E)) 
        {
            estaDefendiendo = true;
            animator.SetBool("isDefending", true);
            movimientoHorizontal = 0; 
        }
        else
        {
            estaDefendiendo = false;
            movimientoHorizontal = Input.GetAxisRaw("Horizontal");
            animator.SetBool("isDefending", false);
        }

        if (estaDefendiendo) return;

        // 6. ATAQUE
        if (estaAtacando) 
        {
            // Forzamos que se quede quieto mientras ataca
            movimientoHorizontal = 0; 
            return; 
        }

        // --- INPUT DE ATAQUE ---
        if (Input.GetButtonDown("Fire1") && !estaDefendiendo && estaEnSuelo)
        {
            StartCoroutine(RealizarAtaque());
            return; 
        }
        movimientoHorizontal = Input.GetAxisRaw("Horizontal");
    }

    void FixedUpdate()
    {
        // 4. FÍSICAS (Mover el cuerpo)
        rb.linearVelocity = new Vector2(movimientoHorizontal * velocidadMovimiento, rb.linearVelocity.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Suelo")) 
        {
            estaEnSuelo = true;
        }
    }

    System.Collections.IEnumerator RealizarAtaque()
    {
        estaAtacando = true; 
    
        movimientoHorizontal = 0f; 
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

        animator.SetTrigger("atacar"); 
        yield return new WaitForSeconds(0.4f); 

        Collider2D[] enemigosGolpeados = Physics2D.OverlapCircleAll(puntoAtaque.position, rangoLatigo, capaEnemigos);

        foreach (Collider2D enemigo in enemigosGolpeados)
        {
            Debug.Log("¡Golpeaste a " + enemigo.name + "!");
            
            EnemyRat scriptRata = enemigo.GetComponent<EnemyRat>();
            if (scriptRata != null)
            {
                scriptRata.RecibirDaño(dañoLatigo);
            }
        }
                yield return new WaitForSeconds(0.3f);

        estaAtacando = false; 
    }

    // Función pública para que los enemigos la llamen
    public void RecibirDaño(int daño, Vector2 posicionEnemigo)
    {

        if (estaDefendiendo)
        {
            float empujeDefensa = fuerzaEmpuje / 2;
            Vector2 direccion = (transform.position - (Vector3)posicionEnemigo).normalized;
            rb.AddForce(new Vector2(direccion.x * empujeDefensa, 0), ForceMode2D.Impulse);
            
            Debug.Log("¡Ataque Bloqueado!");
            return; 
        }

        if (esInvulnerable) return;

        vidaActual -= daño;
        Debug.Log("Vida restante: " + vidaActual);

        if (vidaActual <= 0)
        {
            Morir(); 
        }
        else
        {
            StartCoroutine(EfectoDaño(posicionEnemigo));
        }
    }

    System.Collections.IEnumerator EfectoDaño(Vector2 posicionEnemigo)
    {
        esInvulnerable = true;

        // EMPUJE (Knockback)
        Vector2 direccionEmpuje = (transform.position - (Vector3)posicionEnemigo).normalized;
        rb.linearVelocity = Vector2.zero; 
        rb.AddForce(new Vector2(direccionEmpuje.x * fuerzaEmpuje, fuerzaEmpuje / 2), ForceMode2D.Impulse);

        // FLASH ROJO (Visual)
        for (int i = 0; i < 3; i++) 
        {
            spriteRenderer.color = Color.red; 
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = Color.white; 
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(tiempoInvulnerabilidad - 0.6f);
        
        esInvulnerable = false;
    }

    void Morir()
    {
        Debug.Log("¡HAS MUERTO!");
        this.enabled = false; 
        GetComponent<Collider2D>().enabled = false; 
        rb.linearVelocity = Vector2.zero;
        // animator.SetTrigger("Muerte"); // Si tienes animación de muerte, descomenta esto
    }
}