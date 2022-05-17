using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class targetObjScript : MonoBehaviour
{
    public float shakeAmount = 0.05f;
    public AudioClip hitAudio;
    public int targetType=0;
    public GameObject showEffect;
    AudioSource _audioSource;
    bool isShake;
    Vector3 localPos;
    Vector3 localScale;
    float scaleSize = 0.8f;
    public int count=3;
    bool isHit;
    float timer;
    float coldTime = 3.0f;

    public EnemyScore_SO _enemySO;
    public bool isTargetCounter;
    public int setTarget;


    // Start is called before the first frame update
    void Start()
    {
        /*
        localScale = this.transform.localScale;
        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = hitAudio;
        timer = coldTime;
        */
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (isHit == true && timer==coldTime)
        {
            showHit();
            timer -= Time.deltaTime;
        }
        localPos = this.transform.localPosition;
        if (!isShake) return;

        Vector3 pos = localPos + Random.insideUnitSphere * shakeAmount;
        transform.localPosition = pos;
        
        Invoke("ShakeStop", 0.5f);
        */
        if (isTargetCounter)
        {
            isTargetCount();
        }
    }
    void isTargetCount()
    {
        if(_enemySO.stepCounter > 5)
        {
            _enemySO.getStepHit = 1;
            if(setTarget== _enemySO.getStepHit)
            {
                Destroy(gameObject);
            }
        }
        if(_enemySO.stepCounter > 10)
        {
            _enemySO.getStepHit = 2;
            if (setTarget == _enemySO.getStepHit)
            {
                Destroy(gameObject);
            }
        }
    }
    void hitGet()
    {
        if (isTargetCounter)
        {
            _enemySO.stepCounter += 1;
            Destroy(gameObject,1);
        }
        else
        {
            count -= 1;
            if(count < 0)
            {
                Destroy(gameObject, 1);
            }
        }
   
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("hit");
        hitGet();
    }
    private void OnTriggerEnter(Collider other)
    {   

        /*
        if(other.tag=="ballAttack")
        {
            isHit = true;

            isShake = true;
            if (targetType == 0)
            {
                //this.transform.localScale = new Vector3(this.transform.localScale.x * scaleSize, this.transform.localScale.y * scaleSize, this.transform.localScale.z * scaleSize);
                if (!isTargetCounter)
                {
                    gameObject.GetComponent<SphereCollider>().radius = 0.1f;
                }
                
            }
            if (isTargetCounter)
            {
                _enemySO.stepCounter += 1;
            }

        }*/

    }


    void ShakeStop()
    {
        count -= 1;
        if (count < 0)
        {
            Destroy(gameObject);
        }
        isShake = false;
    }
    void showHit()
    {
        count -= 1;
        if (!isTargetCounter)
        {
            _enemySO.getHit += 1;
        }
        

        Instantiate(showEffect, transform.position, transform.rotation);
        _audioSource.enabled = true;
        isHit = false;
        if (timer < 0)
        {
            timer = coldTime;
        }
    }
}
