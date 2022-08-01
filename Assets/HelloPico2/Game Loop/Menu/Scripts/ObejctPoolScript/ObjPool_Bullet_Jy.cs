using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjPool_Bullet_Jy : MonoBehaviour
{
    public float recoveryTime = 5.0f;
    public GameObject SelPool;
    public string SelPoolName;
    public bool isMove;
    public float speed;
    private float _timer;
    private Transform _mTransform;

   
    private string PoolNamePos;
    private GameObject poolObj;
    // Start is called before the first frame update
    private void Awake()
    {
        if (SelPool == null)
        {
            PoolNamePos = SelPoolName;
        }
        else
        {
            PoolNamePos = SelPool.name;
        }
        poolObj = GameObject.Find(PoolNamePos);
        _mTransform = transform;
    }
    private void OnEnable()
    {
        _timer = Time.time;
    }
    // Update is called once per frame
    void Update()
    {
        if (!gameObject.activeInHierarchy)
            return;
        
        if (Time.time > _timer + recoveryTime)//Recovery GamePool
        {
            poolObj.GetComponent<ObjectPool_Jy>().Recovery(gameObject);
        }

        if (!isMove) return;
            _mTransform.Translate(_mTransform.forward * speed * Time.deltaTime);
    }
    public void GetPoolItem()
    {
        poolObj.GetComponent<ObjectPool_Jy>().Recovery(gameObject);
    }
    /*
    IEnumerator RecoveryPool()
    {
        yield return new WaitForSeconds(1f);
        GameObject.Find(PoolNamePos).GetComponent<ObjectPool_Jy>().Recovery(gameObject);
        yield return null;
    }*/
}
//https://forum.gamer.com.tw/Co.php?bsn=60602&sn=47