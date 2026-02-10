using UnityEngine;
using System.Collections;

public class ObreroAI : MonoBehaviour
{
    // Estados posibles del Obrero
    public enum ObreroState { Idle, Patrol, Chase, Attack, Dead, Hurt }

    [Header("--- Estado Actual ---")]
    public ObreroState currentState;

    [Header("--- Referencias de Combate ---")]
    public Transform puntoMartillo; 
    public float radioGolpe = 0.8f; 
    public LayerMask capaJugador;   
    public int dañoGolpe = 1;

    public float tiempoAntesDeImpacto = 0.4f;
    public float attackCooldown = 2.0f;       

    [Header("--- Configuración General ---")]
    public Transform targetPlayer;
    public float visionRange = 6f;
    public float attackRange = 1.8f; 
    public float maxHealth = 4f;     
    public float tiempoDesaparecer = 1.5f;

    [Header("--- Configuración de Patrulla ---")]
    public float patrolSpeed = 2f;
    public Transform[] patrolPoints;
    public float waitTimeAtPoint = 2f;
    private int currentPatrolIndex = 0;

    [Header("--- Configuración de Persecución ---")]
    public float chaseSpeed = 3.5f;

   
    private float currentHealth;
    private float waitTimer;
    private bool canAttack = true;
    private float velocidadActual = 0f;

    // Componentes
    private Animator anim;
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;

        if (targetPlayer == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) targetPlayer = p.transform;
        }

        ChangeState(ObreroState.Idle);
        if (patrolPoints.Length > 0) waitTimer = waitTimeAtPoint;
    }

    void Update()
    {
        if (currentState == ObreroState.Dead) return;

        switch (currentState)
        {
            case ObreroState.Idle:
                LogicaIdle();
                break;
            case ObreroState.Patrol:
                LogicaPatrulla();
                break;
            case ObreroState.Chase:
                LogicaPersecucion();
                break;
        }
    }

    void FixedUpdate()
    {
        if (currentState == ObreroState.Dead) return;

        if (currentState == ObreroState.Attack || currentState == ObreroState.Hurt)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        rb.linearVelocity = new Vector2(velocidadActual, rb.linearVelocity.y);
        
        anim.SetFloat("Speed", Mathf.Abs(velocidadActual));
    }

    // --- LÓGICA DE ESTADOS ---

    void LogicaIdle()
    {
        velocidadActual = 0f;
        waitTimer -= Time.deltaTime;

        if (waitTimer <= 0 && patrolPoints.Length > 0)
        {
            ChangeState(ObreroState.Patrol);
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
            ChangeState(ObreroState.Idle);
        }
        CheckForPlayer();
    }

    void LogicaPersecucion()
    {
        if (targetPlayer == null) return;

        float distanciaJugador = Vector2.Distance(transform.position, targetPlayer.position);

        if (distanciaJugador > visionRange * 1.5f)
        {
            ChangeState(ObreroState.Patrol);
            return;
        }

        if (distanciaJugador <= attackRange && canAttack)
        {
            StartCoroutine(RutinaAtaque());
        }
        else
        {
            MoverseHacia(targetPlayer.position.x, chaseSpeed);
        }
    }

    // --- MOVIMIENTO ---

    void MoverseHacia(float targetX, float speed)
    {
        float direccion = (targetX > transform.position.x) ? 1f : -1f;
        velocidadActual = direccion * speed;

        if (Mathf.Abs(velocidadActual) > 0.1f)
        {
            if (direccion > 0)
                transform.localScale = new Vector3(1, 1, 1); // Derecha
            else
                transform.localScale = new Vector3(-1, 1, 1); // Izquierda
        }
    }

    // --- ATAQUE ---

    IEnumerator RutinaAtaque()
    {
        ChangeState(ObreroState.Attack);
        canAttack = false;

        anim.SetTrigger("Attack"); 

        yield return new WaitForSeconds(tiempoAntesDeImpacto);

        if (puntoMartillo != null)
        {
            Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(puntoMartillo.position, radioGolpe, capaJugador);
            
            foreach (Collider2D playerCollider in hitPlayers)
            {
                PlayerController playerScript = playerCollider.GetComponent<PlayerController>();
                if (playerScript != null)
                {
                    playerScript.RecibirDaño(dañoGolpe, transform.position);
                }
            }
        }

        yield return new WaitForSeconds(0.5f); 
        ChangeState(ObreroState.Chase);
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    // --- DAÑO Y MUERTE ---

    public void TakeDamage(float damage)
    {
        if (currentState == ObreroState.Dead) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            ChangeState(ObreroState.Dead);
        }
        else if (currentState != ObreroState.Attack)
        {
            ChangeState(ObreroState.Chase);
        }
    }

    void ChangeState(ObreroState newState)
    {
        if (currentState == newState) return;
        currentState = newState;

        anim.SetBool("IsPatrolling", currentState == ObreroState.Patrol);
        anim.SetBool("IsChasing", currentState == ObreroState.Chase);

        if (currentState == ObreroState.Dead)
        {
            velocidadActual = 0f;
            anim.SetTrigger("Die");
            
            rb.isKinematic = true;
            rb.linearVelocity = Vector2.zero;
            GetComponent<Collider2D>().enabled = false;

            StartCoroutine(RutinaDestruir());
        }
    }

    IEnumerator RutinaDestruir()
    {
        yield return new WaitForSeconds(tiempoDesaparecer);
        Destroy(gameObject);
    }

    void CheckForPlayer()
    {
        if (targetPlayer != null && Vector2.Distance(transform.position, targetPlayer.position) < visionRange)
        {
            ChangeState(ObreroState.Chase);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        if (puntoMartillo != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(puntoMartillo.position, radioGolpe);
        }
    }
}