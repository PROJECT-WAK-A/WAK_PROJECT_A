using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    public float sensX = 400f;
    public float sensY = 400f;

    public Transform orientation;
    public Transform cameraTransform;

    float xRotation;
    float yRotation;

    PlayerInput playerInput;

    private void Awake()
    {
        TryGetComponent(out playerInput);
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void LateUpdate()
    {
        float mouseX = playerInput.mouseX * sensX * Time.deltaTime;
        float mouseY = playerInput.mouseY * sensY * Time.deltaTime;
        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0, 0);
    }
}

