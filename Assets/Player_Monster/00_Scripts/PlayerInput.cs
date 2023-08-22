using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public float horizontal;
    public float vertical;
    public float moveAmount;
    public float mouseX;
    public float mouseY;

    PlayerController inputActions;

    Vector2 movementInput;

    private void OnEnable()
    {
        if (inputActions == null)
        {
            inputActions = new PlayerController();

            // 할당
            inputActions.PlayerMovement.Movement.performed += inputActions => movementInput = inputActions.ReadValue<Vector2>();
        }

        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    public void TickInput()
    {
        MoveInput();
    }

    private void MoveInput()
    {
        horizontal = movementInput.x;
        vertical = movementInput.y;

        // Clamp01 = 최소 최대값 fix 반환값 0 ~ 1 float
        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));
        mouseX = Input.GetAxisRaw("Mouse X");
        mouseY = Input.GetAxisRaw("Mouse Y");
    }
}
