using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float speedZ = 10.0f;    // 총알 속도
    private float limitZ = 10.0f;    // 총알 생존 거리
    private float damage = 20.0f;   // 총알 데미지

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward*speedZ*Time.deltaTime);
        if(transform.position.z > limitZ)
        {
            Destroy(gameObject);
        }
    }
}
