using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    Vector3 originalPos;

    private void Awake() => Instance = this;

    private void OnShake(float duration, float strength)
    {
        transform.DOComplete();
        transform.DOShakePosition(duration, strength);
        //transform.DOShakeRotation(duration, strength);
    }

    public static void Shake(float duration, float strength) => Instance.OnShake(duration, strength);
}
