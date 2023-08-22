using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Transform cameraObject;
    public Transform orientation;
    PlayerInput playerInput;

    [HideInInspector]
    public Transform playerTransform;

    public new Rigidbody rigidbody;
    public GameObject normalCamera;

    [Header("Stats")]
    [SerializeField]
    float movementSpeed = 5f;
    [SerializeField]
    float groundDrag;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    void Start()
    {
        TryGetComponent(out rigidbody);

        TryGetComponent(out playerInput);

        cameraObject = Camera.main.transform;

        playerTransform = transform;
    }

    private void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * .5f + .2f, whatIsGround);

        playerInput.TickInput();
        SpeedControl();

        if (grounded)
        {
            rigidbody.drag = groundDrag;
        }
        else
        {
            rigidbody.drag = 0;
        }
    }

    #region Movement
    private void FixedUpdate()
    {
        MovePlayer();
    }
    void MovePlayer()
    {
        // move direction 계산
        Vector3 moveDirection = orientation.forward * playerInput.vertical + orientation.right * playerInput.horizontal;

        rigidbody.AddForce(moveDirection.normalized * movementSpeed * 10f, ForceMode.Force);
    }

    void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rigidbody.velocity.x, 0f, rigidbody.velocity.z);

        if (flatVel.magnitude > movementSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * movementSpeed;
            rigidbody.velocity = new Vector3(limitedVel.x, rigidbody.velocity.y, limitedVel.z);
        }
    }
    #endregion
}
