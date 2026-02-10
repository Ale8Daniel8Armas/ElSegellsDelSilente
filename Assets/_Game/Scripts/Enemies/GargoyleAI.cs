using UnityEngine;
using System.Collections;

public class GargoyleAI : MonoBehaviour
{
    // Usamos la misma estructura de Estados de la Rata
    public enum GargoyleState { Idle, Patrol, Chase, Attack, Dead, Hurt }
    
    [Header("--- Estado Actual ---")]
    public GargoyleState currentState;

    [Header("--- Referencias de Combate ---")]
    public Transform puntoAtaque; // ARRASTRA AQUÍ EL OBJETO "PUÑO"
    public float radioGolpe = 0.8f; // Tamaño de la bola de daño
    public LayerMask capaJugador;   // SELECCIONA LA LAYER "PLAYER" AQUÍ
    public int dañoGolpe = 1;
    
    public float tiempoAntesDeImpacto = 0.3f; // Tiempo de "windup"
    public float attackCooldown = 1.5f;

    [Header("--- Configuración General ---")]
    public Transform targetPlayer; 
    public float visionRange = 5f; 
    public float attackRange = 1.6f; // Distancia para empezar a atacar
    public float maxHealth = 3f;

    [Header("--- Configuración de Patrulla ---")]
    public float patrolSpeed = 2f;
    public Transform[] patrolPoints; 
    public float waitTimeAtPoint = 2f; 
    private int currentPatrolIndex = 0;

    [Header("--- Configuración de Persecución ---")]
    public float chaseSpeed = 4f;

    [Header("--- Configuración General ---")]
    public float tiempoDesaparecer = 2.0f; 

    // Variables internas
    private float currentHealth;
    private float waitTimer;
    private bool canAttack = true;
    private float velocidadActual = 0f; // Controla la física en FixedUpdate

    // Referencias componentes
    private Animator anim;
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;

        // Si no asignaste el targetPlayer manualmente, búscalo por Tag
        if (targetPlayer == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) targetPlayer = p.transform;
        }

        ChangeState(GargoyleState.Idle);
        if (patrolPoints.Length > 0) waitTimer = waitTimeAtPoint;
    }

    void Update()
    {
        if (currentState == GargoyleState.Dead) return;

        // Máquina de estados principal
        switch (currentState)
        {
            case GargoyleState.Idle:
                LogicaIdle();
                break;
            case GargoyleState.Patrol:
                LogicaPatrulla();
                break;
            case GargoyleState.Chase:
                LogicaPersecucion();
                break;
            // Attack y Hurt no necesitan lógica continua en Update,
            // se manejan por Corrutinas o tiempos.
        }
    }

    void FixedUpdate()
    {
        if (currentState == GargoyleState.Dead) return;

        // Si estamos atacando o heridos, nos quedamos quietos
        if (currentState == GargoyleState.Attack || currentState == GargoyleState.Hurt)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        // Aplicar movimiento físico
        rb.linearVelocity = new Vector2(velocidadActual, rb.linearVelocity.y);
        
        // Animación de correr (opcional)
        // anim.SetFloat("Speed", Mathf.Abs(velocidadActual)); 
    }

    // --- LÓGICA POR ESTADOS (Igual que la Rata) ---

    void LogicaIdle()
    {
        velocidadActual = 0f;
        waitTimer -= Time.deltaTime;

        if (waitTimer <= 0 && patrolPoints.Length > 0)
        {
            ChangeState(GargoyleState.Patrol);
        }
        CheckForPlayer();
    }

    void LogicaPatrulla()
    {
        Transform targetPoint = patrolPoints[currentPatrolIndex];
        float distanciaPunto = Vector2.Distance(transform.position, targetPoint.position);

        MoverseHacia(targetPoint.position.x, patrolSpeed);

        if (distanciaPunto < 0.2f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            waitTimer = waitTimeAtPoint;
            ChangeState(GargoyleState.Idle);
        }
        CheckForPlayer();
    }

    void LogicaPersecucion()
    {
        if (targetPlayer == null) return;

        float distanciaJugador = Vector2.Distance(transform.position, targetPlayer.position);

        // Si el jugador se aleja mucho, volver a patrullar
        if (distanciaJugador > visionRange * 1.5f)
        {
            ChangeState(GargoyleState.Patrol); // O Idle
            return;
        }

        // Calculamos distancia real hasta el jugador desde la gárgola
        // (Podríamos usar distancia desde el puño, pero desde el centro es más estable para moverse)
        if (distanciaJugador <= attackRange && canAttack)
        {
            StartCoroutine(RutinaAtaque());
        }
        else
        {
            MoverseHacia(targetPlayer.position.x, chaseSpeed);
        }
    }

    // --- MOVIMIENTO Y GIRO ---

    void MoverseHacia(float targetX, float speed)
    {
        float direccion = (targetX > transform.position.x) ? 1f : -1f;
        velocidadActual = direccion * speed;

        // Girar usando ESCALA (Scale) para que el objeto Puño también gire
        if (Mathf.Abs(velocidadActual) > 0.1f)
        {
            if (direccion > 0)
                transform.localScale = new Vector3(1, 1, 1); // Derecha
            else
                transform.localScale = new Vector3(-1, 1, 1); // Izquierda
        }
    }

    // --- SISTEMA DE ATAQUE (OverlapCircle) ---

    IEnumerator RutinaAtaque()
    {
        ChangeState(GargoyleState.Attack); // Esto pone la velocidad a 0 en FixedUpdate
        canAttack = false;

        anim.SetTrigger("Attack");

        // Esperamos el momento del impacto visual
        yield return new WaitForSeconds(tiempoAntesDeImpacto);

        // --- GOLPE ---
        // Usamos OverlapCircle en la posición del objeto Puño
        if (puntoAtaque != null)
        {
            Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(puntoAtaque.position, radioGolpe, capaJugador);
            
            foreach (Collider2D playerCollider in hitPlayers)
            {
                PlayerController playerScript = playerCollider.GetComponent<PlayerController>();
                if (playerScript != null)
                {
                    // Enviamos daño y la posición de la Gárgola para calcular el empuje
                    playerScript.RecibirDaño(dañoGolpe, transform.position);
                }
            }
        }

        // Esperamos que termine la animación (ajusta este tiempo si es necesario)
        yield return new WaitForSeconds(0.5f);

        // Cooldown
        ChangeState(GargoyleState.Chase);
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    // --- DAÑO RECIBIDO ---

    public void TakeDamage(float damage)
    {
        if (currentState == GargoyleState.Dead) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            ChangeState(GargoyleState.Dead);
        }
        else if (currentState != GargoyleState.Attack)
        {
            ChangeState(GargoyleState.Chase);
        }
    }

    IEnumerator RutinaDestruir()
    {
        yield return new WaitForSeconds(tiempoDesaparecer);
        Destroy(gameObject);
    }

    void ChangeState(GargoyleState newState)
    {
        if (currentState == newState) return;
        currentState = newState;

        // Actualizar parámetros Animator
        anim.SetBool("IsPatrolling", currentState == GargoyleState.Patrol);
        anim.SetBool("IsChasing", currentState == GargoyleState.Chase);

        if (currentState == GargoyleState.Dead)
        {
            velocidadActual = 0f;
            anim.SetTrigger("Die");
            
            // Desactivamos físicas y colisiones para que el cadáver no estorbe
            rb.isKinematic = true;
            rb.linearVelocity = Vector2.zero;
            GetComponent<Collider2D>().enabled = false;

            // Iniciamos la destrucción con retraso
            StartCoroutine(RutinaDestruir());
        }
    }

    void CheckForPlayer()
    {
        if (targetPlayer != null && Vector2.Distance(transform.position, targetPlayer.position) < visionRange)
        {
            ChangeState(GargoyleState.Chase);
        }
    }

    // --- DEBUG VISUAL ---
    private void OnDrawGizmosSelected()
    {
        // Rango Visión
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRange);

        // Rango donde decide atacar
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Área de Daño del Puño
        if (puntoAtaque != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(puntoAtaque.position, radioGolpe);
        }
    }
}