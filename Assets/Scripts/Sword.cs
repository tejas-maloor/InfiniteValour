using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Sword : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] PlayerIK ik;

    Transform originalParent;

    Vector3 hitPoint;
    Vector3 hitNormal;

    private void Start()
    {
        originalParent = transform.parent;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Enemy") && PlayerAnimator.hitWindow)
        {
            CameraShake.Shake(0.35f, 0.25f);
            hitPoint = other.ClosestPoint(transform.position);
            hitNormal = transform.position - hitPoint;
            StartCoroutine(HitStop(other, hitPoint, hitNormal));
        }
    }

    IEnumerator HitStop(Collider other, Vector3 hitPoint, Vector3 hitNormal)
    {
        // yield return new WaitForSeconds(0.02f);
        transform.SetParent(other.transform, true);
        ik.HandIKAmount = 1f;
        ik.ElbowIKAmount = 1f;
        yield return new WaitForSeconds(0.15f);
        transform.SetParent(originalParent);
        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        transform.localScale = Vector3.one;
        ik.HandIKAmount = 0f;
        ik.ElbowIKAmount = 0f;
        other.GetComponent<EnemyScript>().TakeDamage(hitPoint, hitNormal);
    }
}
