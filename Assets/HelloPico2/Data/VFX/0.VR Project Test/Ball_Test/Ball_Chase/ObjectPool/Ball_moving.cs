using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class Ball_moving : MonoBehaviour
{
    private float _timer;
    private Transform _myTransform;

    ObjectPool _ObjectPool;

    Ball_Spawner_Range Ball_Spawner_Range;
    Ball_Spawner Ball_Spawner;

    void Awake()
    {
        Ball_Spawner_Range = GameObject.Find("Spawner").GetComponent<Ball_Spawner_Range>();
        Ball_Spawner = GameObject.Find("Spawner").GetComponent<Ball_Spawner>();

        _ObjectPool = GameObject.Find("ObjectPool").GetComponent<ObjectPool>();

        _myTransform = transform;
    }


    void OnEnable()
    {
        _timer = Time.time;
    }


    float speed = 1;
    float recoveryTime = 1;

    void Update()
    {
        if (!gameObject.activeInHierarchy)
            return;

        speed = Ball_Spawner_Range.Range_Z * 2 / recoveryTime;
        recoveryTime = Ball_Spawner_Range.Range_Z * 2 / speed;

        if (Time.time > _timer + recoveryTime/ (Ball_Spawner.SeedSpeed / Ball_Spawner_Range.Range_Z))
        {
            _ObjectPool.Destroy(gameObject);
        }

        _myTransform.Translate(_myTransform.forward * -speed * (Ball_Spawner.SeedSpeed/ Ball_Spawner_Range.Range_Z) * Time.deltaTime);
    }
}