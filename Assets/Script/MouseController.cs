using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        Cursor.lockState = CursorLockMode.Locked;   // 마우스 커서 고정
    }
}
