using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunMgr : MonoBehaviour
{

    public GameObject gun;              // 총 프리팹
    public GameObject bullet;           // 총알 프리팹
    private float shootTime = 0.0f;     // 총알 발사 시간
    private float shootDelay = 0.1f;    // 총알 발사 딜레이

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        shootTime += Time.deltaTime; 

        if(Input.GetMouseButton(0))
        {
            if(shootTime > shootDelay){
                Instantiate(bullet, gun.transform.position, gun.transform.rotation);
                shootTime = 0.0f;

            }
        }
    }
}
