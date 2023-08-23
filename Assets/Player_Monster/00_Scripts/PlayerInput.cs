using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public float horizontal;
    public float vertical;
    public float mouseX;
    public float mouseY;

    public bool isSprint = false;

    [Header("Controls")]
    [SerializeField] KeyCode sprintKey = KeyCode.LeftShift;

    public void TickInput()
    {
        MoveInput();
    }

    private void MoveInput()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        mouseX = Input.GetAxisRaw("Mouse X");
        mouseY = Input.GetAxisRaw("Mouse Y");

        isSprint = Input.GetKey(sprintKey);
    }
}
