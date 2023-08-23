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
    float thisSpeed;


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
    }

    private void Update()
    {
        // 플레이어 입력 처리
        playerInput.TickInput();

        // 플레이어 상태에 따른 처리
        PlayerState();

        if (canMove)
        {
            // 플레이어 이동량을 계산
            MovementInput();

            if (canShaking)
            {
                CameraShaking();
            }


            // 플레이어 이동 처리
            MovementPlayer();
        }
    }

    #region Movement

    void PlayerState()
    {
        thisSpeed = isSprinting ? sprintSpeed : walkSpeed;
    }
    void MovementInput()
    {
        // move direction 계산
        moveDirection = orientation.forward * playerInput.vertical + orientation.right * playerInput.horizontal;

        if (moveDirection.magnitude > 1f) { moveDirection.Normalize(); }

        moveDirection *= Time.deltaTime * thisSpeed;
    }

    void MovementPlayer()
    {
        if (!characterController.isGrounded) { moveDirection.y -= EnvironmentData.Instance.Gravity * mass * Time.deltaTime; }

        characterController.Move(moveDirection);
    }
    #endregion

    #region Camera
    void CameraShaking()
    {
        if (!characterController.isGrounded) { return; }

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
        // .2f보다 떨어지면 입력을 안한것으로 간주
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
