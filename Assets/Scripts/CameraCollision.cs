using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCollision : MonoBehaviour
{
    [SerializeField] float minDistance = 1.0f;
    [SerializeField] float maxDistance = 4.0f;
    [SerializeField] float smooth = 10.0f;
    [SerializeField] Vector3 dollyDirAdjusted;
    [SerializeField] float distance;
    [SerializeField] LayerMask environmentMask;

    Vector3 dollyDir;


    private void Awake()
    {
        dollyDir = transform.localPosition.normalized;
        distance = transform.localPosition.magnitude;
    }

    void Update()
    {
        Vector3 desiredCamPosition = transform.parent.TransformPoint(dollyDir * maxDistance);
        
        RaycastHit hit;
        if(Physics.Linecast(transform.parent.position, desiredCamPosition, out hit, environmentMask))
        {
            distance = Mathf.Clamp((hit.distance * 0.9f), minDistance, maxDistance);
        }
        else
        {
            distance = maxDistance;
        }

        transform.localPosition = Vector3.Lerp(transform.localPosition, dollyDir * distance, Time.deltaTime * smooth);
    }
}
