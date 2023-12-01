using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetector : MonoBehaviour
{
    [SerializeField] private ThirdPersonController controller;

    public LayerMask layerMask;

    [SerializeField] Vector3 inputDirection;
    [SerializeField] private EnemyScript currentTarget;

    Transform cam;

    private void Start()
    {
        controller = GetComponentInParent<ThirdPersonController>();
        cam = Camera.main.transform;
    }

    private void Update()
    {
        var forward = cam.forward;
        var right = cam.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        inputDirection = forward * controller.GetInput().y + right * controller.GetInput().x;
        inputDirection = inputDirection.normalized;

        RaycastHit info;

        if(Physics.SphereCast(transform.position, 3f, inputDirection, out info, 1f, layerMask)) 
        {
            if (info.collider.transform.GetComponent<EnemyScript>().isAttackable())
            {
                if(currentTarget != null)
                {
                    if (currentTarget != info.collider.transform.GetComponent<EnemyScript>())
                    {
                        currentTarget.SetLockedTarget(false);
                        currentTarget = info.collider.transform.GetComponent<EnemyScript>();
                        currentTarget.SetLockedTarget(true);
                    }
                }
                else
                {
                    currentTarget = info.collider.transform.GetComponent<EnemyScript>();
                    currentTarget.SetLockedTarget(true);
                }
            }
        }

       if(currentTarget != null)
       {
            if(Vector3.Distance(transform.position, currentTarget.transform.position) > 5f)
            {
                currentTarget.SetLockedTarget(false);
                SetCurrentTarget(null);
            }
       }
    }
    
    public EnemyScript CurrentTarget()
    {
        return currentTarget;
    }

    public void SetCurrentTarget(EnemyScript target)
    {
        currentTarget = target;
    }

    public float InputMagnitude()
    {
        return inputDirection.magnitude;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawRay(transform.position, inputDirection);
        Gizmos.DrawWireSphere(transform.position, 1);
        if (CurrentTarget() != null)
            Gizmos.DrawSphere(CurrentTarget().transform.position, .5f);
    }
}
