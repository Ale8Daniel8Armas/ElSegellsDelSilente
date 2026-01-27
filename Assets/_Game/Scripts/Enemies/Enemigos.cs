using UnityEngine;

public class Enemigos : MonoBehaviour
{
    [Header("Puntos patrulla")]
    public Transform pointA;
    public Transform pointB;

    [Header("Detección y ataque")]
    public Transform player;
    public float detectRange = 7f;
    public float attackRange = 1.3f;
    public float attackCooldown = 1.0f;

    [Header("Movimiento suave")]
    public float patrolSpeed = 0.7f;      // tranquilo
    public float chaseSpeed = 1.2f;       // más agresivo al ver
    public float smoothTime = 0.18f;      // suavidad (más alto = más “pesado”)
    public float pauseAtEnds = 0.35f;     // pausa en los extremos (natural)
    public float pauseWhenSee = 0.10f;    // mini reacción al ver (opcional)

    Rigidbody2D rb;
    Animator animator;

    Transform currentPoint;
    Vector2 smoothVel;                   // usado por SmoothDamp
    float nextAttackTime = 0f;
    float pauseTimer = 0f;

    enum State { Patrol, Chase, Attack, React }
    State state = State.Patrol;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        currentPoint = pointB;
    }

    void FixedUpdate()
    {
        if (player == null || pointA == null || pointB == null) return;

        float dist = Vector2.Distance(rb.position, (Vector2)player.position);

        // Si está en pausa (para que se vea natural)
        if (pauseTimer > 0f)
        {
            pauseTimer -= Time.fixedDeltaTime;
            SetWalking(false);
            return;
        }

        // Estados según distancia
        if (dist <= detectRange && state != State.Attack)
        {
            // Apenas lo ve: una mini reacción y luego persigue
            if (state == State.Patrol)
            {
                state = State.React;
                pauseTimer = pauseWhenSee;
                SetWalking(false);
                FaceX(player.position.x - rb.position.x);
                return;
            }

            if (dist <= attackRange)
            {
                state = State.Attack;
            }
            else
            {
                state = State.Chase;
            }
        }
        else if (dist > detectRange && state != State.Patrol)
        {
            state = State.Patrol;
        }

        // Ejecutar comportamiento
        switch (state)
        {
            case State.Patrol:
                Patrol();
                break;

            case State.React:
                // Después de la reacción, pasa a Chase
                state = State.Chase;
                break;

            case State.Chase:
                Chase();
                break;

            case State.Attack:
                DoAttack(dist);
                break;
        }
    }

    void Patrol()
    {
        MoveSmooth(currentPoint.position, patrolSpeed);

        // si llegó al punto, cambia y pausa
        if (Vector2.Distance(rb.position, currentPoint.position) < 0.18f)
        {
            currentPoint = (currentPoint == pointA) ? pointB : pointA;
            pauseTimer = pauseAtEnds;
            SetWalking(false);
        }
    }

    void Chase()
    {
        MoveSmooth(player.position, chaseSpeed);
    }

    void DoAttack(float dist)
    {
        // si se aleja un poquito, vuelve a perseguir
        if (dist > attackRange + 0.2f)
        {
            state = State.Chase;
            return;
        }

        SetWalking(false);
        FaceX(player.position.x - rb.position.x);

        // no spamear ataque
        if (Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + attackCooldown;
            animator.SetTrigger("Attack");
        }
    }

    // Movimiento suave tipo “ease” (sin tirones)
    void MoveSmooth(Vector2 target, float spd)
    {
        Vector2 dir = target - rb.position;

        // si está muy cerca, no vibra
        if (dir.magnitude < 0.02f)
        {
            SetWalking(false);
            return;
        }

        // “posición deseada” con velocidad
        Vector2 desired = Vector2.MoveTowards(rb.position, target, spd * Time.fixedDeltaTime);

        // suavizado real (como el blanco)
        Vector2 smoothed = Vector2.SmoothDamp(rb.position, desired, ref smoothVel, smoothTime);
        rb.MovePosition(smoothed);

        SetWalking(true);
        FaceX(dir.x);
    }

    void FaceX(float xDir)
    {
        if (Mathf.Abs(xDir) < 0.01f) return;
        transform.localScale = new Vector3(xDir > 0 ? 1 : -1, 1, 1);
    }

    void SetWalking(bool walking)
    {
        animator.SetBool("isWalking", walking);
    }
}
