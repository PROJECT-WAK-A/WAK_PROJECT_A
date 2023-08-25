using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UI;

namespace Item
{
    public class GunMgr : MonoBehaviour
    {

        public GameObject gun;              // 총 프리팹
        public GameObject bullet;           // 총알 프리팹
        private float shootTime = 0.0f;     // 총알 발사 시간
        private float shootDelay = 0.1f;    // 총알 발사 딜레이
        private int bulletCount = 30;  // 총알 개수
        
        private enum GunModeState { 
            Empty,
            Auto,
            SemiAuto
        };

        private GunModeState gunState = GunModeState.Empty; // 총 상태
        private GunModeState previousState = GunModeState.Empty; // 이전 상태 저장


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
            shootTime += Time.deltaTime; 
            
            // 총 상태 변경
            if(Input.GetKeyDown(KeyCode.T))
            {
                gunState = (GunModeState)(((int)gunState + 1) % Enum.GetNames(typeof(GunModeState)).Length);        // 총 상태 변경
                previousState = gunState;                                                                           // 이전 상태 저장
                uiUpdateable.UpdateFireMode(gunState.ToString());                                                   // 발사 모드 UI 갱신
                SetFiringMode(gunState);
                Debug.Log(gunState);
            }

            if(Input.GetKeyDown(KeyCode.R)){
                Reload();
            }

            if(bulletCount <= 0)
            {
                gunState = GunModeState.Empty;
                SetFiringMode(gunState);
            }

            if (Input.GetMouseButton(0)){
                if (shootTime > shootDelay && gunState != GunModeState.Empty){          // 총이 비거나 총알 발사 시간이 지났을 때만 총알 발사
                    TryShoot(bullet);
                    shootTime = 0.0f;
                    uiUpdateable.UpdateBulletCount(--bulletCount);
                }
            }
        }

        // 총 발사 상태 변경
        private void SetFiringMode(GunModeState state)
        {
            switch (state)
            {
            case GunModeState.Empty:
                // 비어있는 상태에서는 아무 작업도 하지 않음
                break;

            case GunModeState.Auto:
                // Auto 상태에서는 계속 발사
                shootDelay = 0.1f;
                break;

            case GunModeState.SemiAuto:
                // SemiAuto 상태에서는 연사가 불가능하도록 발사 딜레이를 길게 설정
                shootDelay = 0.5f;
                break;
            }
        }

        /// <summary>
        /// 총알 발사
        /// </summary>
        /// <param name="bullet">총알 프리팹</param>
        private void TryShoot(GameObject bullet){
            Instantiate(bullet, gun.transform.position, gun.transform.rotation);
        }

        private void Reload(){
            // R 키를 누르면 재장전
            bulletCount = 30;
            uiUpdateable.UpdateBulletCount(bulletCount);                // 총알 개수 UI 갱신
            gunState = previousState;                                   // 이전 상태로 변경
        }
    }

}

