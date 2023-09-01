using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controllers
{
    public class PlayerController : MonoBehaviour
    {

        [SerializeField]
        private float playerSpeed;      // 케릭터 이동속도
        private float playerMoveSpeed;  // 케릭터 현재 이동 속도
        private bool isMove = false;   // 케릭터 이동 체크

        [SerializeField]
        private float mouseSensitivity;      // 마우스 감도

        [SerializeField]
        private float camRotationLimit;      // 카메라 회전 제한
        private float currentCamRotationX;

        [SerializeField]
        private Camera camera;


        private Rigidbody playerRigidbody; // 케릭터 리지드바디


        // Start is called before the first frame update
        void Start()
        {
            playerRigidbody = GetComponent<Rigidbody>();
        }

        // Update is called once per frame
        void Update()
        {
            Move();
            CameraRotation();
            CharaterRotation();
        }


        /// <summary>
        /// 케릭터 이동
        /// </summary>
        private void Move()
        {
            float moveX = Input.GetAxisRaw("Horizontal");
            float moveZ = Input.GetAxisRaw("Vertical");

            Vector3 moveHorizontal = transform.right * moveX;
            Vector3 moveVertical = transform.forward * moveZ;

            // 이동 방향에 따라 이동속도 변경
            Vector3 velocity = (moveHorizontal + moveVertical).normalized * playerSpeed;

            playerRigidbody.MovePosition(transform.position + velocity * Time.deltaTime);
        }


        /// <summary>
        /// 카메라 회전
        /// </summary>
        private void CameraRotation()
        {
            float xRotation = Input.GetAxisRaw("Mouse Y");
            float cameraRotation = xRotation * mouseSensitivity;

            // 카메라 회전
            currentCamRotationX -= cameraRotation;
            currentCamRotationX = Mathf.Clamp(currentCamRotationX, -camRotationLimit, camRotationLimit);

            camera.transform.localEulerAngles = new Vector3(currentCamRotationX, 0f, 0f);   // 카메라 회전
        }

        /// <summary>
        /// 케릭터 회전
        /// </summary>
        private void CharaterRotation()
        {
            float xRotation = Input.GetAxisRaw("Mouse X");
            Vector3 charaterRotationY = new Vector3(0f, xRotation, 0f) * mouseSensitivity;
            playerRigidbody.MoveRotation(playerRigidbody.rotation * Quaternion.Euler(charaterRotationY)); // 케릭터 회전
        }

        // 케릭터의 현재 시선 방향 반환
        public Vector3 GetAimDirection()
        {
            return camera.transform.forward;
        }
    }

}
