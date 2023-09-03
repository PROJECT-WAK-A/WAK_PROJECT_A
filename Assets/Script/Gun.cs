using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Items
{
    /// <summary>
    /// 총의 정보를 담고 있는 클래스
    /// 모든 총기 류들이 갖고 있을 공통적인 정보를 담고 있음
    /// </summary>
    public class Gun : MonoBehaviour
    {
        public string gunName;      // 총의 이름
        public float range;         // 사정거리
        public float accuracy;      // 정확도
        public float fireRate;      // 연사속도
        public float reloadTime;    // 재장전 속도
        public float damage = 0;    // 총의 데미지
        public float bulletSpeed;   // 총알 속도


        public int reloadBulletCount;   // 총알 재장전 개수
        public int currentBulletCount;  // 현재 탄알집에 남아있는 총알 개수
        public int maxBulletCount;      // 최대 소유 가능 총알 개수
        public int carryBulletCount;    // 현재 소유하고 있는 총알 개수

        public float recoilForce;              // 반동 세기
        public float recoilFineSightForce;     // 정조준시 반동 세기
        public Vector3 fineSightOriginPos;          // 정조준시 위치

        // 총기 모드 상태
        public enum GunModeState
        {
            Empty,
            Auto,
            SemiAuto
        };

        public GunModeState firingMode;             // 현재 총기 모드 상태
        public ParticleSystem fireFlash;           // 총알 발사 시 파티클

    }

}
