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

    // Start is called before the first frame update
    void Start()
    {
        localScale = this.transform.localScale;
        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = hitAudio;
        timer = coldTime;
    }

    // Update is called once per frame
    void Update()
    {
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

   

    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag=="ballAttack")
        {
            isHit = true;

            isShake = true;
            if (targetType == 0)
            {
                //this.transform.localScale = new Vector3(this.transform.localScale.x * scaleSize, this.transform.localScale.y * scaleSize, this.transform.localScale.z * scaleSize);
                gameObject.GetComponent<SphereCollider>().radius = 0.1f;
            }
        }
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
        Instantiate(showEffect, transform.position, transform.rotation);
        _audioSource.enabled = true;
        isHit = false;
        if (timer < 0)
        {
            timer = coldTime;
        }
    }
}
