using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Configuración")]
    public float velocidadMovimiento = 15f;
    public float fuerzaSalto = 12f;

    [Header("Referencias")]
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private bool estaDefendiendo = false;

    private float movimientoHorizontal;

    private bool estaEnSuelo = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
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

        // 4. GIRAR PERSONAJE (Flip)
        if (movimientoHorizontal > 0) spriteRenderer.flipX = false;
        else if (movimientoHorizontal < 0) spriteRenderer.flipX = true;

        // 5. POSE DE DEFENSA

        if (Input.GetKey(KeyCode.E)) 
        {
            estaDefendiendo = true;
            movimientoHorizontal = 0; 
        }
        else
        {
            estaDefendiendo = false;
            movimientoHorizontal = Input.GetAxisRaw("Horizontal");
        }

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
}