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

        [SerializeField] private Gun currentGun;                    // 현재 장착된 총
        [SerializeField] private GameObject objGun;                 // 총 프리팹
        [SerializeField] private GameObject bulletPrefab;           // 총알 프리팹
        [SerializeField] private Transform bulletTrans;             // 총알 발사 위치
        [SerializeField] private PlayerController playerController; // 플레이어 컨트롤러

        private float currentFireRate;       // 현재 연사 속도
        private float shootDelay = 0.1f;    // 총알 발사 딜레이
        private bool isReload = false;      // 재장전 중인지 확인하는 변수
        private bool isFineSight = false;   // 정조준 중인지 확인하는 변수
        private Vector3 originPos;          // 정조준 해제 시 위치 복구를 위한 변수

        private Vector3 screenCenter;       // 화면 중앙
        private Vector3 crosshairPosition;  // 크로스헤어의 위치

        /// <summary>
        /// 총알 개수를 표시할 UI
        /// </summary>
        private IUIUpdateable uiUpdateable;

        private void Start()
        {
            uiUpdateable = FindObjectOfType<UIMgr>();
            originPos = objGun.transform.localPosition;

            // 화면 중앙을 기준으로 크로스헤어의 위치 계산
            screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
            crosshairPosition = Camera.main.ScreenToWorldPoint(screenCenter);
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

            if (Input.GetMouseButtonDown(1))
            {
                TryFineSight();
            }
        }

        /// <summary>
        /// 총기 모드 변경 시도
        /// </summary>
        private void TrySetFireMode()
        {
            // 나중에 애니메이션 등 연출에 따라 사용
            if (objGun != null)
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
            if (!isReload)
            {
                // // 연사 속도가 0이면 총알 발사
                if (currentFireRate <= 0 && currentGun.firingMode != Gun.GunModeState.Empty && currentGun.currentBulletCount > 0)
                {
                    Shoot();
                    uiUpdateable.UpdateBulletCount(--currentGun.currentBulletCount, currentGun.carryBulletCount);                // 총알 개수 UI 갱신
                }
                else
                {
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
            GameObject objBullet = Instantiate(bulletPrefab, bulletTrans.transform.position, bulletTrans.transform.rotation);
            Bullet bullet = objBullet.GetComponent<Bullet>();    // 총알 스크립트 가져오기
            bullet.SetBulletSetting(currentGun.bulletSpeed, currentGun.range, CalculateBulletDirection()); // 총알 설정

            currentGun.fireFlash.Play();    // 총 발사 시 총구 화염 효과 재생

            StopAllCoroutines();                // 코루틴 모두 정지
            StartCoroutine(RecoilCoroutine());  // 반동 코루틴 실행
        }

        private Vector3 CalculateBulletDirection()
        {
            // 총알의 이동 방향 계산
            Vector3 bulletDirection = (crosshairPosition - bulletTrans.transform.position).normalized;

            return bulletDirection;
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
            }
            else
            {
                // 총알이 없으면 재장전을 하지 않음
                Debug.Log("총알이 없습니다.");
            }

            uiUpdateable.UpdateBulletCount(currentGun.currentBulletCount, currentGun.carryBulletCount);      // 총알 개수 UI 갱신
        }

        /// <summary>
        /// 정조준 시도 - 정조준 상태가 아니면 정조준
        /// </summary>
        private void TryFineSight()
        {
            FineSight();
        }

        /// <summary>
        /// 정조준 상태 변경 처리 과정
        /// </summary>
        private void FineSight()
        {
            isFineSight = !isFineSight;

            if (isFineSight)
            {
                StopAllCoroutines();                            // 코루틴 모두 정지
                StartCoroutine(FineSightEnabledCoroutine());   // 정조준 코루틴 실행
            }
            else
            {
                StopAllCoroutines();
                StartCoroutine(FineSightDisabledCoroutine()); // 정조준 해제 코루틴 실행
            }
        }

        /// <summary>
        /// 정조준 코루틴
        /// </summary>
        IEnumerator FineSightEnabledCoroutine()
        {
            // 정조준 시 총의 위치를 정조준 위치로 이동
            while (currentGun.transform.localPosition != currentGun.fineSightOriginPos)
            {
                // 정조준 위치로 이동
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, currentGun.fineSightOriginPos, 0.2f);
                yield return null;
            }
        }

        /// <summary>
        /// 정조준 해제 코루틴
        /// </summary>
        IEnumerator FineSightDisabledCoroutine()
        {
            while (currentGun.transform.localPosition != originPos)
            {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, originPos, 0.2f);
                yield return null;
            }

        }

        IEnumerator RecoilCoroutine()
        {
            // 반동 세기 만큼 반동 후퇴
            Vector3 recoilBack = new Vector3(originPos.x, originPos.y, -currentGun.recoilForce);
            Vector3 recoilFineSightBack = new Vector3(currentGun.fineSightOriginPos.x, currentGun.fineSightOriginPos.y, -currentGun.recoilFineSightForce);

            // 정조준 상태가 아닐 때 반동
            if (!isFineSight)
            {
                // 총기 위치 초기화
                currentGun.transform.localPosition = originPos;

                // 반동 액션 실행
                while (currentGun.transform.localPosition.z >= recoilBack.z + 0.02f)
                {
                    currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, recoilBack, 0.4f);
                    yield return null;
                }

                // 원위치 이동
                while (currentGun.transform.localPosition != originPos)
                {
                    currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, originPos, 0.1f);
                    yield return null;
                }
            }
            else
            {  // 정조준 시 
                // 총기 위치 초기화
                currentGun.transform.localPosition = currentGun.fineSightOriginPos;

                // 반동 액션 실행
                while (currentGun.transform.localPosition.z >= recoilFineSightBack.z + 0.02f)
                {
                    currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, recoilFineSightBack, 0.4f);
                    yield return null;
                }

                // 원위치
                while (currentGun.transform.localPosition != currentGun.fineSightOriginPos)
                {
                    currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, currentGun.fineSightOriginPos, 0.1f);
                    yield return null;
                }
            }
        }
    }

}

