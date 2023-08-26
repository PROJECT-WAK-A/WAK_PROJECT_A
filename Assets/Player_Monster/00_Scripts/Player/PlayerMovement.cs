using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    bool isSprinting => canSprint && playerInput.isSprint;

    public Transform orientation;
    PlayerInput playerInput;


    [HideInInspector]
    public Transform playerTransform;
    private CharacterController characterController;
    Vector3 moveDirection;

    [SerializeField] Transform playerCamera;

    [Header("Stats")]
    [SerializeField, Range(1, 10)] float walkSpeed = 5f;
    [SerializeField, Range(1, 15)] float sprintSpeed = 8f;
    [SerializeField, Range(1, 200)] float mass = 10f;
    [SerializeField] bool canMove = true;
    [SerializeField] bool canSprint = true;
    [SerializeField] bool canShaking = true;


    [Header("Jump")]
    [SerializeField] float jumpForce = 8f;
    [SerializeField] bool canJump = true;
    [SerializeField] float groundOffset = -.14f;
    [SerializeField] float groundRadius = .5f;
    [SerializeField, Range(0, 5)] float drag = 0.9f;
    // 종단 속도
    [SerializeField] float terminalVelocity = 55f;
    public float jumpTimeout = .1f;
    public float fallTimeout = .15f;
    public LayerMask groundLayer;
    float thisSpeed;
    bool isGrounded = true;
    float verticalVelocity;

    // timeout deltatime
    private float jumpTimeoutDelta;
    private float fallTimeoutDelta;



    [Header("Camera shake while moving")]
    [SerializeField, Range(1, 10)] float shakingSpeed = 14f;
    [SerializeField, Range(0f, 1f)] float yShakingAmount = .5f;
    [SerializeField, Range(0f, 1f)] float xShakingAmount = .5f;
    Vector3 defaultCamPos = Vector3.zero;
    bool isShaking = false;
    float timer = 0f;



    void Start()
    {
        TryGetComponent(out playerInput);
        TryGetComponent(out characterController);

        defaultCamPos = playerCamera.localPosition;

        playerTransform = transform;

        jumpTimeoutDelta = jumpTimeout;
        fallTimeoutDelta = fallTimeout;
    }

    private void Update()
    {
        // 플레이어 입력 처리
        playerInput.TickInput();

        // 플레이어 상태에 따른 처리
        PlayerState();
        PlayerGroundCheck();

        if (canMove)
        {
            // 플레이어 이동량을 계산
            MovementInput();

            if (canShaking)
            {
                CameraShaking();
            }

            if (canJump)
                MovementPlayerJump();

            // 중력
            MovementPlayerGravity();
            // 플레이어 이동 처리
            MovementPlayer();
        }
    }

    #region Movement

    void PlayerState()
    {
        thisSpeed = isSprinting ? sprintSpeed : walkSpeed;
    }

    void PlayerGroundCheck()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - groundOffset, transform.position.z);
        isGrounded = Physics.CheckSphere(spherePosition, groundRadius, groundLayer, QueryTriggerInteraction.Ignore);
    }
    void MovementInput()
    {
        // move direction 계산
        moveDirection = orientation.forward * playerInput.vertical + orientation.right * playerInput.horizontal;

        if (moveDirection.magnitude > 1f) { moveDirection.Normalize(); }

        moveDirection *= thisSpeed;

        moveDirection.y = verticalVelocity;
    }
    void MovementPlayerGravity()
    {
        if (verticalVelocity < terminalVelocity)
        {
            verticalVelocity += EnvironmentData.Instance.Gravity * mass * drag * Time.deltaTime;
        }

    }
    void MovementPlayer()
    {
        characterController.Move(moveDirection * Time.deltaTime);
    }

    void MovementPlayerJump()
    {
        if (isGrounded)
        {
            fallTimeoutDelta = fallTimeout;

            if (verticalVelocity < 0f)
            {
                verticalVelocity = -2f;
            }

            if (playerInput.isJump && jumpTimeoutDelta <= 0f)
            {
                verticalVelocity = Mathf.Sqrt(jumpForce * -2f * EnvironmentData.Instance.Gravity);
            }

            if (jumpTimeoutDelta >= 0f)
            {
                jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            jumpTimeoutDelta = jumpTimeout;

            if (fallTimeoutDelta >= 0f)
            {
                fallTimeoutDelta -= Time.deltaTime;
            }
            playerInput.isJump = false;
        }

    }
    #endregion

    #region Camera
    void CameraShaking()
    {
        if (!isGrounded) { return; }

        // 이동량이 있을때만
        if (playerInput.horizontal > .2f || playerInput.vertical > .2f)
        {
            isShaking = true;
            timer += Time.deltaTime * shakingSpeed * thisSpeed;

            playerCamera.localPosition
            = new Vector3(defaultCamPos.x + Mathf.Sin(timer) * xShakingAmount,
                          defaultCamPos.y + -Mathf.Abs(Mathf.Sin(timer)) * yShakingAmount,
                          playerCamera.localPosition.z);
        }
        // .2f보다 아래면 입력을 안한것으로 간주
        else
        {
            if (isShaking == false) { return; }

            playerCamera.localPosition = Vector3.Lerp(playerCamera.localPosition, defaultCamPos, .33f);
            timer = 0f;
        }

        if (playerInput.horizontal == 0f && playerInput.vertical == 0f)
        {
            playerCamera.localPosition = defaultCamPos;
            isShaking = false;
        }

    }
    #endregion
}
