//using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class Enemy : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] Transform player;
    [SerializeField] Rigidbody rb;
    [SerializeField] float pushBackForce;
    [SerializeField] float knockBackForce;
    [SerializeField] GameObject hitVFX;
    [SerializeField] GameObject dieVFX;
    [SerializeField] int health = 100;

    private int currentHealth;

    private enum State
    {
        Idle,
        MoveToPlayer,
        Waiting,
        Attack,
        Die
    }

    State state;

    private void Start()
    {
        state = State.Idle;
        currentHealth = health;
        player = FindObjectOfType<ThirdPersonController>().transform;
    }

    private void Update()
    {
        switch(state)
        {
            case State.Idle:
                break;
            case State.MoveToPlayer:
                break;
            case State.Waiting:
                break;
            case State.Attack:
                break;
            case State.Die:
                break;
        }

        transform.LookAt(player.position);
    }

    public void TakeDamage(bool knockHit, Vector3 hitPoint, Vector3 hitNormal)
    {
        currentHealth -= 25;

        if (currentHealth <= 0) 
        {
            Die(knockHit);
        }
        else
        {
            if (knockHit)
            {
                anim.SetTrigger("KnockHit");
                rb.AddForce(-transform.forward * knockBackForce + transform.up * 8, ForceMode.Impulse);
                Instantiate(hitVFX,
                    hitPoint,
                    Quaternion.FromToRotation(hitVFX.transform.forward, hitNormal));
            }
            else
            {
                anim.SetTrigger("Hit");
                Instantiate(hitVFX,
                    hitPoint,
                    Quaternion.FromToRotation(hitVFX.transform.forward, hitNormal));
                rb.AddForce(-transform.forward * pushBackForce, ForceMode.Impulse);

            }
        }
    }

    IEnumerator Wait(float time)
    {
        yield return new WaitForSeconds(time);
    }

    void Die(bool knockHit)
    {
        int waitTime = knockHit ? 10 : 2;
        StartCoroutine(Wait(waitTime));
        Instantiate(dieVFX,
            transform.position,
            Quaternion.identity);
        Destroy(this.gameObject);
    }
}
