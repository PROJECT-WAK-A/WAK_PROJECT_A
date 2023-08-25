using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GunMgr : MonoBehaviour
{

    public GameObject gun;              // 총 프리팹
    public GameObject bullet;           // 총알 프리팹
    private float shootTime = 0.0f;     // 총알 발사 시간
    private float shootDelay = 0.1f;    // 총알 발사 딜레이
    
    private enum GunState { 
        Empty,
        Auto,
        SemiAuto
    };

    private GunState gunState = GunState.Empty; // 총 상태

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        shootTime += Time.deltaTime; 
        
        // 총 상태 변경
        if(Input.GetKeyDown(KeyCode.T))
        {
            // T 키를 누르면 총 상태 변경
            gunState = (GunState)(((int)gunState + 1) % Enum.GetNames(typeof(GunState)).Length);
            ChangedGunState(gunState);
            Debug.Log(gunState);
        }

        if (Input.GetMouseButton(0)){
            if (shootTime > shootDelay && gunState != GunState.Empty){          // 총이 비거나 총알 발사 시간이 지났을 때만 총알 발사
                TryShoot(bullet);
                shootTime = 0.0f;
            }
        }
    }

    // 총 상태 변경
    private void ChangedGunState(GunState state)
    {
        switch (state)
        {
        case GunState.Empty:
            // 비어있는 상태에서는 아무 작업도 하지 않음
            break;

        case GunState.Auto:
            // Auto 상태에서는 계속 발사
            shootDelay = 0.1f;
            break;

        case GunState.SemiAuto:
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
}
