using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyScript : MonoBehaviour
{
    private Animator anim;
    private EnemyManager enemyManager;
    private CharacterController characterController;
    private EnemyDetector enemyDetector;
    private ComboCharacter comboCharacter;

    [Header("Stats")]
    public int health = 5;
    private float moveSpeed = 1.0f;
    private Vector3 moveDirection;

    [SerializeField] GameObject hitVFX;
    [SerializeField] GameObject dieVFX;
    [SerializeField] float pushBackForce;
    [SerializeField] HealthBar healthBar;
    [SerializeField] float gravity = -9.81f;
    [SerializeField] float groundDistance = 0.4f;
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundMask;


    int currentHealth;
    float _pushBackForce;
    Vector3 velocity;

    [Header("States")]
    [SerializeField] private bool isPreparingAttack;
    [SerializeField] private bool isMoving;
    [SerializeField] private bool isRetreating;
    [SerializeField] private bool isLockedTarget;
    [SerializeField] private bool isStunned;
    [SerializeField] private bool isWaiting = true;

    bool IsGrounded;

    //[Header("Effects")]
    //[SerializeField] private ParticleSystem counterParticle;

    private Coroutine PrepareAttackCoroutine;
    private Coroutine RetreatCoroutine;
    private Coroutine DamageCoroutine;
    private Coroutine MovementCoroutine;

    public UnityEvent<EnemyScript> OnDamage;
    public UnityEvent<EnemyScript> OnStopMoving;
    public UnityEvent<EnemyScript> OnRetreat;

    private void Start()
    {
        currentHealth = health;
        healthBar.UpdateHealthBar(health, currentHealth);

        enemyManager = GetComponentInParent<EnemyManager>();

        anim = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();

        comboCharacter = FindObjectOfType<ComboCharacter>();
        enemyDetector = comboCharacter.GetComponentInChildren<EnemyDetector>();

        comboCharacter.OnTrajectory.AddListener((x) => OnPlayerTrajectory(x));

        MovementCoroutine = StartCoroutine(EnemyMovement());
    }

    IEnumerator EnemyMovement()
    {
        yield return new WaitUntil(() => isWaiting == true);

        int randomChance = Random.Range(0, 2);

        if(randomChance == 1)
        {
            int randomDir = Random.Range(0, 2);
            moveDirection = randomDir == 1 ? Vector3.right : Vector3.left;
            isMoving = true;
        }
        else
        {
            StopMoving();
        }

        yield return new WaitForSeconds(1);

        MovementCoroutine = StartCoroutine(EnemyMovement());
    }

    private void Update()
    {
/*        IsGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (IsGrounded && velocity.y < 0)
            velocity.y = -2f;


        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);*/

        transform.LookAt(new Vector3(
                comboCharacter.transform.position.x, 
                transform.position.y, comboCharacter.transform.position.z));

        MoveEnemy(moveDirection);
    }

    public void SetAttack()
    {
        isWaiting = false;

        PrepareAttackCoroutine = StartCoroutine(PrepAttack());

        IEnumerator PrepAttack()
        {
            PrepareAttack(true);
            yield return new WaitForSeconds(.2f);
            moveDirection = Vector3.forward;
            isMoving = true;
        }
    }


    void PrepareAttack(bool active)
    {
        isPreparingAttack = active;

        if (active)
        {
            //counterParticle.Play();
        }
        else
        {
            StopMoving();
            //counterParticle.Clear();
            //counterParticle.Stop();
        }
    }

    public void SetRetreat()
    {
        StopEnemyCoroutines();

        RetreatCoroutine = StartCoroutine(PrepRetreat());

        IEnumerator PrepRetreat()
        {
            yield return new WaitForSeconds(1.4f);
            OnRetreat.Invoke(this);
            isRetreating = true;
            moveDirection = -Vector3.forward;
            isMoving = true;
            yield return new WaitUntil(() => Vector3.Distance(transform.position, comboCharacter.transform.position) > 4);
            isRetreating = false;
            StopMoving();

            //Free 
            isWaiting = true;
            MovementCoroutine = StartCoroutine(EnemyMovement());
        }
    }

    void OnPlayerTrajectory(EnemyScript target)
    {
        if(target == this)
        {
            StopEnemyCoroutines();
            isLockedTarget = true;
            PrepareAttack(false);
            StopMoving();
        }
    }

    void MoveEnemy(Vector3 direction)
    {
        moveSpeed = 1;

        if (direction == Vector3.forward)
            moveSpeed = 5;
        if (direction == -Vector3.forward)
            moveSpeed = 2;

        anim.SetFloat("InputMagnitude", (characterController.velocity.normalized.magnitude * direction.z) / (5 / moveSpeed), .2f, Time.deltaTime);
        anim.SetBool("SideMove", (direction == Vector3.right || direction == Vector3.left));

        if (!isMoving)
            return;

        Vector3 dir = (comboCharacter.transform.position - transform.position).normalized;
        Vector3 pDir = Quaternion.AngleAxis(90, Vector3.up) * dir;
        Vector3 moveDir = Vector3.zero;

        Vector3 finalDirection = Vector3.zero;

        if (direction == Vector3.forward)
            finalDirection = dir;
        if (direction == Vector3.right || direction == Vector3.left)
            finalDirection = (pDir * direction.normalized.x);
        if (direction == -Vector3.forward)
            finalDirection = -transform.forward;

        if (direction == Vector3.right || direction == Vector3.left)
            moveSpeed /= 1.5f;

        moveDir += finalDirection * moveSpeed * Time.deltaTime;

        characterController.Move(moveDir);

        if (!isPreparingAttack)
            return;

        if(Vector3.Distance(transform.position, comboCharacter.transform.position) < 2)
        {
            StopMoving();
            Attack();

/*            if (comboCharacter.meleeStateMachine.CurrentState.GetType() == typeof(IdleCombatState))
                Attack();
            else
                PrepareAttack(false);*/
        }

    }

    private void Attack()
    {
        //transform.DOMove(transform.position + (transform.forward / 1), .5f);
        anim.SetTrigger("Attack");
    }

    public void HitEvent()
    {
        comboCharacter.OnHit(this);
        PrepareAttack(false);
    }    

    public void StopMoving()
    {
        isMoving = false;
        moveDirection = Vector3.zero;
        if (characterController.enabled)
            characterController.Move(moveDirection);
    }

    void StopEnemyCoroutines()
    {
        PrepareAttack(false);

        if (isRetreating)
        {
            if (RetreatCoroutine != null)
            {
                StopCoroutine(RetreatCoroutine);
                isRetreating = false;
            }
        }

        if (PrepareAttackCoroutine != null)
            StopCoroutine(PrepareAttackCoroutine);

        if (DamageCoroutine != null)
            StopCoroutine(DamageCoroutine);

        if (MovementCoroutine != null)
            StopCoroutine(MovementCoroutine);
    }

    public void TakeDamage(Vector3 hitPoint, Vector3 hitNormal)
    {
        StopEnemyCoroutines();

        enemyDetector.SetCurrentTarget(null);
        isLockedTarget = false;

        _pushBackForce = pushBackForce;

        currentHealth -= 1;
        healthBar.UpdateHealthBar(health, currentHealth);

        if (currentHealth <= 0)
        {
            Die();
            return;
        }
        else
        {
            anim.SetTrigger("Hit");

            Instantiate(hitVFX,
                hitPoint,
                Quaternion.FromToRotation(hitVFX.transform.forward, hitNormal));

            StartCoroutine(PushBack());

        }
    }

    IEnumerator PushBack()
    {
        while (_pushBackForce >= 3f)
        {
            characterController.Move(-transform.forward * _pushBackForce * Time.deltaTime);
            _pushBackForce -= _pushBackForce * 3f * Time.deltaTime;
            yield return null;
        }
    }
    
    void Die()
    {
        StopEnemyCoroutines();

        Instantiate(dieVFX, transform.position, Quaternion.identity);
        comboCharacter.AddToAbilityBar();
        this.enabled = false;
        characterController.enabled = false;
        enemyManager.SetEnemyAvailiability(this, false);
        this.gameObject.SetActive(false);
    }

    public bool isAttackable()
    {
        return health > 0;
    }

    public bool IsPreparingAttack()
    {
        return isPreparingAttack;
    }

    public bool IsRetreating()
    {
        return isRetreating;
    }

    public bool IsLockedTarget()
    {
        return isLockedTarget;
    }

    public void SetLockedTarget(bool b)
    {
        isLockedTarget = b; 
    }

    public bool IsStunned()
    {
        return isStunned;
    }
}
