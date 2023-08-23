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

    public new Rigidbody rigidbody;
    public GameObject normalCamera;

    [Header("Stats")]
    [SerializeField, Range(1, 10)] float walkSpeed = 5f;
    [SerializeField, Range(1, 15)] float sprintSpeed = 8f;
    [SerializeField, Range(1, 200)] float mass = 10f;
    [SerializeField] bool canSprint = true;
    float thisSpeed;

    Vector3 moveDirection;
    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;

    private CharacterController characterController;

    void Start()
    {
        TryGetComponent(out rigidbody);
        TryGetComponent(out playerInput);
        TryGetComponent(out characterController);

        playerTransform = transform;
    }

    private void Update()
    {
        // 플레이어 입력 처리
        playerInput.TickInput();

        // 플레이어 상태에 따른 처리
        PlayerState();

        // 플레이어 이동량을 계산
        MovementInput();

        // 플레이어 이동 처리
        MovementPlayer();
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
    }

    void MovementPlayer()
    {
        if (!characterController.isGrounded) { moveDirection.y -= EnvironmentData.Instance.Gravity * mass * Time.deltaTime; }

        characterController.Move(Time.deltaTime * thisSpeed * moveDirection);
    }
    #endregion
}
