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

        [SerializeField]
        private Transform bulletTrans;    // 총알 위치

        private float currentFireRate;       // 현재 연사 속도

        private float shootDelay = 0.1f;    // 총알 발사 딜레이
        private bool isReload = false;      // 재장전 중인지 확인하는 변수

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
            if (Input.GetKeyDown(KeyCode.T))
            {
                TrySetFireMode();
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                TryReload();
            }

            if (Input.GetMouseButton(0))
            {
                TryShoot();
            }
        }

        /// <summary>
        /// 총기 모드 변경 시도
        /// </summary>
        private void TrySetFireMode()
        {
            // 나중에 애니메이션 등 연출에 따라 사용
            if (gun != null)
            {
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
        private void TryShoot()
        {   
            // 재장전 중이면 발사하지 않음
            if(!isReload){
                // // 연사 속도가 0이면 총알 발사
                if (currentFireRate <= 0 && currentGun.firingMode != Gun.GunModeState.Empty && currentGun.currentBulletCount > 0)
                {
                    Shoot();
                    uiUpdateable.UpdateBulletCount(--currentGun.currentBulletCount, currentGun.carryBulletCount);                // 총알 개수 UI 갱신
                }else{
                    // 재장전을 계속 진행
                }
            }

        }

        /// <summary>
        /// 총알 발사 처리 과정
        /// </summary>
        private void Shoot()
        {
            currentFireRate = currentGun.fireRate + shootDelay;

            // 총알 생성 및 발사
            Instantiate(bullet, bulletTrans.transform.position, bulletTrans.transform.rotation);
        }

        /// <summary>
        /// 연사 속도 계산
        /// </summary>
        private void GunFireRateCalculate()
        {
            if (currentFireRate > 0)
            {
                currentFireRate -= Time.deltaTime;
            }
        }

        /// <summary>
        /// 재장전 시도 - 재장전 중이 아니고, 탄알집에 남아이 있는 총알이 가득 차 있지 않으면 재장전
        /// </summary>
        private void TryReload()
        {
            if (!isReload && currentGun.currentBulletCount < currentGun.reloadBulletCount)
            {
                ReloadCoroutine();
            }
        }

        /// <summary>
        /// 재장전 처리 과정
        /// todo: 재장전 애니메이션 추가, 재장전 소리 추가
        /// </summary>
        private void ReloadCoroutine()
        {
            // 현재 남아있는 총알의 개수가 0보다 크다면 재장전
            if (currentGun.carryBulletCount > 0)
            {
                isReload = true;

                // 기존 총알을 소유하고 있는 총알 개수에 더하고 재장전
                currentGun.carryBulletCount += currentGun.currentBulletCount;   
                currentGun.currentBulletCount = 0;                              

                if (currentGun.carryBulletCount >= currentGun.reloadBulletCount)
                {
                    currentGun.currentBulletCount = currentGun.reloadBulletCount;  
                    currentGun.carryBulletCount -= currentGun.reloadBulletCount;   
                }
                else
                {
                    // 현재 소유하고 있는 총알 개수가 재장전 가능한 최대 개수보다 작다면
                    // 현재 소유하고 있는 총알 개수만큼만 재장전
                    currentGun.currentBulletCount = currentGun.carryBulletCount;   
                    currentGun.carryBulletCount = 0;                               
                }

                isReload = false;
            }else{
                // 총알이 없으면 재장전을 하지 않음
                Debug.Log("총알이 없습니다.");
            }

            uiUpdateable.UpdateBulletCount(currentGun.currentBulletCount, currentGun.carryBulletCount);      // 총알 개수 UI 갱신
        }
    }

}

