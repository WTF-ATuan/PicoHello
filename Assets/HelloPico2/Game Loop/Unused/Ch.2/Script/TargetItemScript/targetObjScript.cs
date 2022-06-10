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
        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = hitAudio;
        _audioSource.enabled = false;
        /*
        localScale = this.transform.localScale;
        
        timer = coldTime;
        */
    }

    // Update is called once per frame
    void Update()
    {
        
        if(isHit)
        {
            ScaleObj();
            showHit();
            //timer -= Time.deltaTime;
        }
        /*
        localPos = this.transform.localPosition;
        if (!isShake) return;

        Vector3 pos = localPos + Random.insideUnitSphere * shakeAmount;
        transform.localPosition = pos;
        
        Invoke("ShakeStop", 0.5f);
        
        
        if (isTargetCounter)
        {
            isTargetCount();
        }*/
    }
    /*
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
    }*/

    private void OnTriggerEnter(Collider other)
    {
        isHit = true;

        /*
        if (isTargetCounter)
        {
            _enemySO.stepCounter += 1;
        }*/
    }

    void ScaleObj()
    {
        this.transform.localScale = new Vector3(this.transform.localScale.x * scaleSize, this.transform.localScale.y * scaleSize, this.transform.localScale.z * scaleSize);
        Destroy(gameObject,3);
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
        /*
        count -= 1;
        if (!isTargetCounter)
        {
            _enemySO.getHit += 1;
        }*/
        if(isHit == true && timer == coldTime)
        {
            Instantiate(showEffect, transform.position, transform.rotation);
            _audioSource.enabled = true;
            timer -= Time.deltaTime;
        }

    }
}
