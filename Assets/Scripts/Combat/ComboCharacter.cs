using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class ComboCharacter : MonoBehaviour
{
    public int health = 100;
    public int abilityAmount = 3;
    public bool idleCombatState = true;

    public HealthUI healthUI;
    public Animator anim;
    public bool allowCombo = false;
    public static bool knockHit = false;

    public AnimationCurve attack1Move;
    public AnimationCurve attack2Move;
    public AnimationCurve attack3Move;
    public AnimationCurve attack4Move;

    public ThirdPersonController controller;

    public StateMachine meleeStateMachine;
    private EnemyDetector enemyDetector;
    private int currentHealth;
    private int currentAbilityAmt;

    public UnityEvent<EnemyScript> OnTrajectory;

    // Start is called before the first frame update
    void Start()
    {
        meleeStateMachine = GetComponent<StateMachine>();
        controller = GetComponent<ThirdPersonController>();
        enemyDetector = GetComponentInChildren<EnemyDetector>();

        currentHealth = health;
        healthUI.UpdateHealthBar(health, currentHealth);
        healthUI.UpdateAbilityBar(abilityAmount, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0) && idleCombatState)
        {
            meleeStateMachine.SetNextState(new GroundEntryState());
        }
        HandleAbility();
    }

    public void ResetCombo()
    {
        idleCombatState = true;
        meleeStateMachine.SetNextStateToMain();
    }

    void MoveTowardsTarget(EnemyScript target, float duration)
    {
        OnTrajectory.Invoke(target);
        transform.DOLookAt(target.transform.position, .2f);
        transform.DOMove(TargetOffset(target.transform), duration);

    }

    public Vector3 TargetOffset(Transform target)
    {
        Vector3 position = target.position;
        return Vector3.MoveTowards(position, transform.position, .95f);
    }

    float TargetDistance(EnemyScript target)
    {
        return Vector3.Distance(transform.position, target.transform.position);
    }

    public void OnHit(EnemyScript attackingEnemy)
    {
        if(TargetDistance(attackingEnemy) < 3)
        {
            anim.SetTrigger("Hit");
            TakeDamage(10);
            StartCoroutine(DisableMovement());
        }
    }

    void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthUI.UpdateHealthBar(health, currentHealth);
        if (currentHealth <= 0)
            StartCoroutine(Die());
    }

    IEnumerator Die()
    {
        anim.SetBool("Die", true);
        controller.enabled = false;
        this.enabled = false;

        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene(2);
    }

    IEnumerator DisableMovement()
    {
        controller.enabled = false;
        yield return new WaitForSeconds(0.8f);
        controller.enabled = true;
    }

    public bool IsDead()
    {
        return currentHealth <= 0;
    }

    public void AddToAbilityBar()
    {
        currentAbilityAmt++;
        healthUI.UpdateAbilityBar(abilityAmount, currentAbilityAmt);
    }

    public void HandleAbility()
    {
        if(currentAbilityAmt >= abilityAmount && Input.GetKeyDown(KeyCode.Space))
        {
            //Debug.Log("Heal");
            currentAbilityAmt = 0;
            currentHealth += 15;
            healthUI.UpdateAbilityBar(abilityAmount, currentAbilityAmt);
            healthUI.UpdateHealthBar(health, currentHealth);
        }
    }
}
