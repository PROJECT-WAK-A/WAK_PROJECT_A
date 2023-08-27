using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UI;
using Items;

namespace Controllers
{
    public class GunController : MonoBehaviour
    {
        
        [SerializeField]
        private Gun currentGun;             // 현재 장착된 총
        [SerializeField]
        private GameObject gun;              // 총 프리팹
        [SerializeField]
        private GameObject bullet;           // 총알 프리팹
        
        private float currentFireRate;          // 현재 연사 속도

        private float shootDelay = 0.1f;    // 총알 발사 딜레이
        private int bulletCount = 30;            // 총알 개수


        /// <summary>
        /// 총알 개수를 표시할 UI
        /// </summary>
        private IUIUpdateable uiUpdateable;

        private void Start()
        {
            uiUpdateable = FindObjectOfType<UIMgr>();
        }

        // Update is called once per frame
        void Update()
        {
            GunFireRateCalculate();
            
            // 총 상태 변경
            if(Input.GetKeyDown(KeyCode.T))
            {
                TrySetFireMode();
            }

            if(Input.GetKeyDown(KeyCode.R)){
                // Reload();
            }

            if (Input.GetMouseButton(0)){
                TryShoot();
            }
        }

        /// <summary>
        /// 총기 모드 변경 시도
        /// </summary>
        private void TrySetFireMode(){
            // 나중에 애니메이션 등 연출에 따라 사용
            if(gun != null){
                SetFiringMode();
            }
        }

        // 총 발사 상태 변경 처리 과정
        private void SetFiringMode()
        {
            currentGun.firingMode = (Gun.GunModeState)(((int)currentGun.firingMode + 1) % Enum.GetNames(typeof(Gun.GunModeState)).Length); // 발사 모드 변경
            uiUpdateable.UpdateFireMode(currentGun.firingMode.ToString()); // 발사 모드 UI 갱신

            switch (currentGun.firingMode)
            {
                case Gun.GunModeState.Empty:
                    // 비어있는 상태에서는 아무 작업도 하지 않음
                    break;

                case Gun.GunModeState.Auto:
                    // Auto 상태에서는 연사가 가능하도록 발사 딜레이를 짧게 설정
                    shootDelay = 0.1f;
                    break;

                case Gun.GunModeState.SemiAuto:
                    // SemiAuto 상태에서는 연사가 불가능하도록 발사 딜레이를 길게 설정
                    shootDelay = 0.8f;
                    break;
            }
        }

        /// <summary>
        /// 총알 발사 시도 = 발사 가능 상태인지 확인, 발사 진행 과정
        /// </summary>
        private void TryShoot(){
            // // 연사 속도가 0이면 총알 발사
            if(currentFireRate <= 0 && currentGun.firingMode != Gun.GunModeState.Empty && bulletCount > 0){
                Shoot();
                uiUpdateable.UpdateBulletCount(--bulletCount);                // 총알 개수 UI 갱신
            }
        }

        /// <summary>
        /// 총알 발사 처리 과정
        /// </summary>
        private void Shoot(){
            currentFireRate = currentGun.fireRate + shootDelay;

            // 총알 생성 및 발사
            Instantiate(bullet, gun.transform.position, gun.transform.rotation);
        }

        // private void Reload(){
        //     // R 키를 누르면 재장전
        //     bulletCount = 30;
        //     uiUpdateable.UpdateBulletCount(bulletCount);                // 총알 개수 UI 갱신
        //     gunState = previousState;                                   // 이전 상태로 변경
        // }


        /// <summary>
        /// 연사 속도 계산
        /// </summary>
        private void GunFireRateCalculate(){
            if(currentFireRate > 0){
                currentFireRate -= Time.deltaTime;
            }
        }
    }

}

