using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class targetObjScript : MonoBehaviour
{
    public float shakeAmount = 0.05f;
    public AudioClip[] hitAudio;
    public int targetType=0;
    public GameObject showEffect;
    AudioSource source;
    bool isShake;
    Vector3 firstPos;
    Vector3 localScale;
    float scaleSize = 0.8f;
    int count=3;

    // Start is called before the first frame update
    void Start()
    {
        firstPos = this.transform.localPosition;
        localScale = this.transform.localScale;
        source = GetComponent<AudioSource>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (count < 0 )
        {
            //source.PlayOneShot(hitAudio[1], 1);
            Destroy(gameObject);
        }
        if (!isShake) return;
        Vector3 pos = firstPos + Random.insideUnitSphere * shakeAmount;
        pos.y = transform.localPosition.y;
        transform.localPosition = pos;
        
        Invoke("ShakeStop", 1.0f);

   

    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag=="ball")
        {
            isShake = true;
            if (targetType == 0)
            {
                this.transform.localScale = new Vector3(this.transform.localScale.x * scaleSize, this.transform.localScale.y * scaleSize, this.transform.localScale.z * scaleSize);
                Instantiate(showEffect, transform.position, transform.rotation);
                source.PlayOneShot(hitAudio[0], 1);

            }

            
        }
    }
    void ShakeStop()
    {
        count -= 1;
        isShake = false;
        firstPos.y = transform.localPosition.y;
        transform.localPosition = firstPos;
    }
}
