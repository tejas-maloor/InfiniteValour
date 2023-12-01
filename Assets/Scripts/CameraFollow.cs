using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] float cameraMoveSpeed = 120f;
    [SerializeField] GameObject cameraFollowObj;
    [SerializeField] float clampAngle = 80f;
    [SerializeField] float inputSensitivty = 150f;

    float mouseX;
    float mouseY;
    float rotY = 0.0f;
    float rotX = 0.0f;
    Vector2 followPos;

    void Start()
    {
        Vector3 rot = transform.localRotation.eulerAngles;
        rotY = rot.y;
        rotX = rot.x;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");

        rotY += mouseX * inputSensitivty * Time.deltaTime;
        rotX -= mouseY * inputSensitivty * Time.deltaTime;

        rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);

        Quaternion localRotation = Quaternion.Euler(rotX, rotY, 0.0f);
        transform.rotation = localRotation;
    }

    private void LateUpdate()
    {
        CameraUpdater();
    }

    void CameraUpdater()
    {
        Transform target = cameraFollowObj.transform;

        float step = cameraMoveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target.position, step);
    }
}
