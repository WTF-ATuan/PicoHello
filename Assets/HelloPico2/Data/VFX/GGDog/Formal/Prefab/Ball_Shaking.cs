using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball_Shaking : MonoBehaviour
{
    [Range(0.25f, 0)]
    public float Min_Scale = 0.01f;

    void Awake()
    {
        OriScale = transform.localScale * Random.Range(0.5f, 1.25f);
    }

    Vector3 OriScale;
    private float _RandomTrans_timer;

    Vector3 RandomTrans;

    private void OnEnable()
    {
        transform.localScale = Vector3.zero;
    }

    void Update()
    {

        //隨機的微秒時間內做隨機位置取用
        if (Time.time > _RandomTrans_timer + Random.Range(0.25f, 0.75f))
        {
            _RandomTrans_timer = Time.time;
            RandomTrans = Random.insideUnitSphere * Random.Range(0.5f, 3) / Vector3.Distance(Vector3.Normalize(Camera.main.transform.position), Vector3.Normalize(transform.position));
        }
        //飄移向隨機位置，完成隨機飄動的效果
        transform.Translate(Vector3.Lerp(Vector3.zero, RandomTrans, 0.03f));

        //Size逆透視
        float d = Min_Scale + Vector3.Distance(Vector3.zero,transform.position)/700;

        //Size從0長到原本的Size，再乘上d是做逆透視效果
        transform.localScale = Vector3.Lerp(transform.localScale, OriScale * d, 0.025f);


    }
}
