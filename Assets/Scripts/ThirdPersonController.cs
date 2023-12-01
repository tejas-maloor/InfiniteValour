using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class ThirdPersonController : MonoBehaviour
{
    [SerializeField] CharacterController controller;
    [SerializeField] Transform cam;
    [SerializeField] Transform groundCheck;

    [SerializeField] float speed = 6f;
    [SerializeField] float turnSmoothTime = 0.1f;
    [SerializeField] float gravity = -9.81f;
    [SerializeField] float groundDistance = 0.4f;
    [SerializeField] float jumpHeight = 3f;
    [SerializeField] float slideSpeed;
    [SerializeField] LayerMask groundMask;

    float turnSmoothVelocity;
    Vector3 velocity;
    Vector3 slideDir;
    float horizontal;
    float vertical;

    ComboCharacter comboChar;


    [HideInInspector]
    public float Speed;
    [HideInInspector]
    public bool IsGrounded;
    [HideInInspector]
    public bool Roll;
    [HideInInspector]
    public bool canMove = true;

    private State state;
    public GameObject transitionImage;

    private enum State
    {
        Normal,
        DodgeRollSliding
    }

    private void Start()
    {
        cam = Camera.main.transform;
        state = State.Normal;
        comboChar = GetComponent<ComboCharacter>();
    }

    void Update()
    {
        IsGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (IsGrounded && velocity.y < 0)
            velocity.y = -2f;

        switch(state)
        {
            case State.Normal:
                HandleMovement();
                HandleGravity();
                HandleRoll();
                break;
            case State.DodgeRollSliding:
                HandleDodgeRollSliding();
                break;

        }
    }

    void HandleMovement()
    {
        if (!canMove)
        {
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, cam.eulerAngles.y, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
        }
        else
        {
            horizontal = Input.GetAxisRaw("Horizontal");
            vertical = Input.GetAxisRaw("Vertical");
            Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
            Speed = Mathf.Clamp01(direction.sqrMagnitude);

            if (direction.magnitude >= 0.1f)
            {
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                controller.Move(moveDir.normalized * speed * Time.deltaTime);
            }
        }
    }

    void HandleGravity()
    {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleRoll()
    {
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            Roll = true;
            comboChar.ResetCombo();
            state = State.DodgeRollSliding;

            Vector3 inputDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
            if (inputDir == Vector3.zero) inputDir = new Vector3(0f, 0f, -1f);

            float targetAngle = Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);

            slideDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            slideSpeed = 20f;
        }
    }

    void HandleDodgeRollSliding()
    {
        controller.Move(slideDir.normalized * slideSpeed * Time.deltaTime);

        slideSpeed -= slideSpeed * 3f * Time.deltaTime;

        if (slideSpeed < 5f)
        {
            Roll = false;
            state = State.Normal;
        }
    }

    public void AttackMove(float amount)
    {
        controller.Move(transform.forward * amount * Time.deltaTime);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if(hit.collider.CompareTag("Portal"))
        {
            transitionImage.GetComponent<Animator>().Play("FadeInOut");
        }
        
        if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            transform.DOKill();
        }
    }

    #region Public Variables

    public Vector2 GetInput()
    {
        return new Vector2(horizontal, vertical);
    }

    #endregion
}
