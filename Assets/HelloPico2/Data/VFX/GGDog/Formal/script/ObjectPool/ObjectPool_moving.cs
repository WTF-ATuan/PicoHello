using UnityEngine;
using System.Collections;

public class ObjectPool_moving : MonoBehaviour
{
    public bool Rotate_Moving = false;
    public float Rotate_IsReverse = 1;

    private float _timer;
    
    [HideInInspector] public ObjectPool _ObjectPool;
    [HideInInspector] public ObjectPool_Spawner_Range ObjectPool_Spawner_Range;
    [HideInInspector] public ObjectPool_Spawner ObjectPool_Spawner;
    
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

        speed = ObjectPool_Spawner_Range.Range_Z * 2 / recoveryTime;
        recoveryTime = ObjectPool_Spawner_Range.Range_Z * 2 / speed;

        if (Time.time > _timer + recoveryTime/ (ObjectPool_Spawner.SeedSpeed / ObjectPool_Spawner_Range.Range_Z))
        {
            _ObjectPool.Destroy(gameObject);
        }

        if (!Rotate_Moving)
        {
            transform.localPosition += new Vector3(0, 0, 1) * -speed * (ObjectPool_Spawner.SeedSpeed / ObjectPool_Spawner_Range.Range_Z) * Time.deltaTime;
            return;
        }

        if (Rotate_Moving)
        {
            transform.RotateAround(new Vector3(Rotate_IsReverse * 500, 0, 0), new Vector3(0, 1, 0), Rotate_IsReverse *- speed * (ObjectPool_Spawner.SeedSpeed / (ObjectPool_Spawner_Range.Range_Z)) * Time.deltaTime / 15);
            
        }
    }
}