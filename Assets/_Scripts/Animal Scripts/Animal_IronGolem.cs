using System;
using System.Collections;
using UnityEditor.Analytics;
using UnityEngine;
using UnityEngine.AI;

[SelectionBase]
public class Animal_IronGolem : MonoBehaviour, I_Animal
{
    [Header("Stats")]
    public int _hp;
    public int _ap;
    public int _mp;

    public int hp
    {
        get { return _hp; }
        set { _hp = value; }
    }

    public int ap
    {
        get { return _ap; }
        set { _ap = value; }
    }

    public int mp
    {
        get { return _mp; }
        set { _mp = value; }
    }

    [Header("Movement Settings")]
    public float speed = 1f;
    public float alertDuration = 5f;

    [Header("Attack Settings")]
    public float attackCooldown = 2f;
    public float smashDamageDelay = 1.4f;
    public float smashDamageDuration = 0.5f;
    public IronGolem_SmashDetector smashDetector;
    public float smashNudgeForce = 100f;

    [Header("Detection Settings")]
    [SerializeField]
    float sightHeight = 1f;

    [SerializeField]
    float sightDistance = 15f;

    [SerializeField]
    float forgetDistance = 20f;
    LayerMask playerLayerMask;
    RaycastHit playerRaycastHit;

    [HideInInspector]
    public EnemyMode mode;

    [HideInInspector]
    public EnemyMode _currentMode;

    [HideInInspector]
    public bool inTransit;

    [HideInInspector]
    public bool running;

    [HideInInspector]
    public bool attacking;

    [HideInInspector]
    public Vector3 direction;

    private bool returningFromInterrupt;

    private bool alertBehaviorActive;

    [HideInInspector]
    public Rigidbody rB;

    [Header("References")]
    private WaypointSystem waypointSystem;
    private Animator animator;
    public AnimatorStateInfo stateInfo;
    private NavMeshAgent navMeshAgent;
    private Coroutine movementMotorCoroutine;
    private Coroutine initPlayerDetected;
    private Coroutine alertStartBehavior;
    private Coroutine attackBehavior;

    void Awake()
    {
        waypointSystem = GetComponentInChildren<WaypointSystem>();
        mode = EnemyMode.Patrol;
        rB = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        playerLayerMask = LayerMask.GetMask("Player");
        navMeshAgent = GetComponent<NavMeshAgent>();
        smashDetector = GetComponentInChildren<IronGolem_SmashDetector>(true);
        smashDetector.golem = this;
    }

    public void Patrol()
    {
        if (_currentMode != EnemyMode.Patrol)
        {
            Debug.Log(transform.name + " is patrolling.");
            _currentMode = EnemyMode.Patrol;
            navMeshAgent.isStopped = true;
            navMeshAgent.updateRotation = false;
        }

        if (!inTransit)
        {
            inTransit = true;
            waypointSystem.speed = speed;

            if (movementMotorCoroutine == null)
            {
                if (returningFromInterrupt)
                {
                    movementMotorCoroutine = StartCoroutine(
                        PatrolWaypointMotor(transform.position)
                    );
                    returningFromInterrupt = false;
                }
                else
                    movementMotorCoroutine = StartCoroutine(PatrolWaypointMotor());
                initPlayerDetected = null;
            }
        }

        if (direction.sqrMagnitude > 0)
            transform.rotation = Quaternion.LookRotation(-direction);

        if (PlayerDetected())
        {
            Debug.Log(transform.name + " detected the player!");
            mode = EnemyMode.Alert;
            StopCoroutine(movementMotorCoroutine);
            inTransit = false;
            running = false;
            if (initPlayerDetected == null)
            {
                movementMotorCoroutine = null;
                if (initPlayerDetected == null)
                {
                    initPlayerDetected = StartCoroutine(InitPlayerDetected());
                }
            }
        }
    }

    bool PlayerDetected()
    {
        bool playerSighted = Physics.Raycast(
            transform.position + new Vector3(0, sightHeight, 0),
            transform.TransformDirection(Vector3.forward),
            out playerRaycastHit,
            sightDistance,
            playerLayerMask
        );
        return playerSighted;
    }

    IEnumerator InitPlayerDetected()
    {
        yield return null;
        initPlayerDetected = null;
    }

    public void Alert()
    {
        if (_currentMode != EnemyMode.Alert)
        {
            Debug.Log(transform.name + " was alerted by " + PLAYERSingleton.i.name);
            _currentMode = EnemyMode.Alert;
            if (alertStartBehavior == null)
            {
                alertStartBehavior = StartCoroutine(AlertCoroutine());
            }
            alertBehaviorActive = true;
            navMeshAgent.updateRotation = false;
        }

        Vector3 targetPos = PLAYERSingleton.i.transform.position;
        targetPos.y = transform.position.y;
        transform.LookAt(targetPos);

        if (Vector3.Distance(transform.position, targetPos) > forgetDistance)
        {
            mode = EnemyMode.Patrol;
            navMeshAgent.ResetPath();
            return;
        }

        if (alertBehaviorActive == false)
        {
            alertStartBehavior = null;
            mode = EnemyMode.Pursue;
        }
    }

    IEnumerator AlertCoroutine()
    {
        Debug.Log("Alert Coroutine started");
        yield return new WaitForSeconds(alertDuration);
        alertBehaviorActive = false;
        Debug.Log("Alert Coroutine ending");
    }

    void TargetRangeBehavior(Vector3 target)
    {
        bool targetTooFar = Vector3.Distance(transform.position, target) > forgetDistance;
        bool targetInAttackRange =
            Vector3.Distance(transform.position, target) <= navMeshAgent.stoppingDistance
            && !attacking;

        if (attacking)
            return;

        if (targetTooFar)
        {
            mode = EnemyMode.Patrol;
            returningFromInterrupt = true;
            navMeshAgent.ResetPath();
        }
        else if (targetInAttackRange)
        {
            if (mode != EnemyMode.Attack)
                mode = EnemyMode.Attack;
        }
        else if (mode != EnemyMode.Pursue)
            mode = EnemyMode.Pursue;
    }

    IEnumerator InitReturnToPatrol()
    {
        yield return null;
    }

    void Pursue()
    {
        if (_currentMode != EnemyMode.Pursue)
        {
            Debug.Log(transform.name + " has started pursuing " + PLAYERSingleton.i.name + "!!");
            _currentMode = EnemyMode.Pursue;
            navMeshAgent.isStopped = false;
            navMeshAgent.updateRotation = false;
            attacking = false;
        }

        Vector3 targetPos = PLAYERSingleton.i.transform.position;
        targetPos.y = transform.position.y;
        direction = targetPos - transform.position;

        transform.LookAt(targetPos);
        navMeshAgent.SetDestination(PLAYERSingleton.i.rB.position);

        TargetRangeBehavior(PLAYERSingleton.i.rB.position);
    }

    void Attack()
    {
        if (_currentMode != EnemyMode.Attack)
        {
            _currentMode = EnemyMode.Attack;
            running = false;
            navMeshAgent.isStopped = true;
            navMeshAgent.updateRotation = false;
        }

        if (attackBehavior == null && !attacking)
        {
            attackBehavior = StartCoroutine(AttackCoroutine());
        }

        TargetRangeBehavior(PLAYERSingleton.i.rB.position);
    }

    IEnumerator AttackCoroutine()
    {
        attacking = true;

        yield return null;

        float currentAnimLength = animator.GetCurrentAnimatorStateInfo(0).length;

        yield return new WaitForSeconds(smashDamageDelay);

        if (smashDetector != null)
            smashDetector.gameObject.SetActive(true);

        rB.isKinematic = false;
        rB.AddForce(direction * smashNudgeForce, ForceMode.Impulse);

        yield return new WaitForSeconds(smashDamageDuration);
        rB.isKinematic = true;

        if (smashDetector != null)
            smashDetector.gameObject.SetActive(false);

        float remainingTime = currentAnimLength - (smashDamageDelay + smashDamageDuration);
        if (remainingTime > 0)
            yield return new WaitForSeconds(remainingTime);

        attacking = false;

        yield return new WaitForSeconds(attackCooldown);

        attackBehavior = null;
    }

    public void Die()
    {
        Destroy(gameObject);
        animator.SetTrigger("Die");
    }

    public void TakeDamage(int amount)
    {
        hp = hp - amount;
    }

    IEnumerator PatrolWaypointMotor(Vector3? interruptVector = null) // nullable value type
    {
        int activeIndex;

        if (waypointSystem.activeWaypointTarget == null)
        {
            activeIndex = 0;
        }
        else
        {
            activeIndex = waypointSystem.activeWaypointTarget.index;
        }

        for (int i = activeIndex; i < waypointSystem.waypoints.Count; i++)
        {
            Vector3 originVector;
            if (interruptVector.HasValue)
            {
                originVector = interruptVector.GetValueOrDefault(); // nullable value type
                interruptVector = null;
            }
            else
                originVector = waypointSystem.waypoints[i].location;
            waypointSystem.activeWaypointTarget = waypointSystem.waypoints[i];
            running = true;
            direction = (
                originVector - waypointSystem.waypoints[i].neighborNext.location
            ).normalized;
            float distance = Vector3.Distance(
                originVector,
                waypointSystem.waypoints[i].neighborNext.location
            );
            float calculatedSpeed = distance / speed;

            for (float j = 0; j < 1; j += Time.deltaTime / calculatedSpeed)
            {
                rB.MovePosition(
                    Vector3.Lerp(originVector, waypointSystem.waypoints[i].neighborNext.location, j)
                );
                yield return null;
            }
            running = false;
            yield return new WaitForSeconds(1);
        }
        inTransit = false;
        movementMotorCoroutine = null;
        waypointSystem.activeWaypointTarget = null;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerDamage") && PLAYERSingleton.i.playerIsAttacking)
        {
            UISingleton.i.debug.pushMessage(
                other.transform.root + " hit " + transform.name,
                "#ff3355"
            );
            UISingleton.i.debug.pushMessage(transform.name + " took", "#ff3355", false);
            UISingleton.i.debug.pushMessage(" " + 10, "#ff3355", false);
            UISingleton.i.debug.pushMessage(" damage!", "#ff3355");
            Debug.Log(other.name + " hit " + transform.name);
            PLAYERSingleton.i.playerIsAttacking = false;

            TakeDamage(10);
        }
    }

    void Update()
    {
        UpdateMode();
        UpdateAnimation();
    }

    void FixedUpdate()
    {
        UpdateMode();
    }

    private void UpdateMode()
    {
        if (mode == EnemyMode.Idle) { }
        if (mode == EnemyMode.Patrol)
        {
            Patrol();
        }
        if (mode == EnemyMode.Alert)
        {
            Alert();
        }
        if (mode == EnemyMode.Pursue)
        {
            Pursue();
        }
        if (mode == EnemyMode.Attack)
        {
            Attack();
        }
        if (mode == EnemyMode.Retreat) { }
    }

    void UpdateAnimation()
    {
        if (
            (running || (mode == EnemyMode.Pursue && navMeshAgent.velocity.magnitude > 0))
            && !attacking
        )
            animator.SetBool("isRunning", true);
        else if (!running)
        {
            animator.SetBool("isRunning", false);
        }

        if (attacking)
        {
            animator.SetBool("isAttacking", true);
        }
        else if (!attacking)
        {
            animator.SetBool("isAttacking", false);
        }
    }
}
