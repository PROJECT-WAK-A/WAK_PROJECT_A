using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Items
{
    public class Bullet : MonoBehaviour
    {
        private float speed;            // 총알 속도
        private float maxDistance;      // 총알 최대 사정거리
        private Vector3 moveDirection;

        private Vector3 initPosition;   // 총알 초기 위치

        void Start(){
            initPosition = transform.position;
        }

        // Update is called once per frame
        void Update()
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
            CheckDistance();
        }
             
        // 총알이 최대 사정거리를 넘어가면 사라지도록 설정
        private void CheckDistance()
        {
            float distanceTraveled = Vector3.Distance(initPosition, transform.position);
            if(distanceTraveled  >= maxDistance)
            {
                DestroyBullet();
            }
        }


        /// <summary>
        /// 총알의 속도와 최대 사정거리를 설정
        /// </summary>
        /// <param name="speed"> 총알 속도</param>
        /// <param name="maxDistance">총알 사정거리</param>
        public void SetBulletSetting(float speed, float maxDistance, Vector3 moveDirection)
        {
            this.speed = speed;
            this.maxDistance = maxDistance;
            this.moveDirection = moveDirection.normalized; // 이동 방향을 정규화하여 저장
        }

        // todo: 총알이 사라지는 동시에 파티클이 나오도록 수정
        public void DestroyBullet()
        {
            Destroy(gameObject);
        }

        private void OnTriggerEnter(Collider other) {
            if(other.gameObject.tag == "Enemy")
            {
                DestroyBullet();
            }
        }

    }
}
