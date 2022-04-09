using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class targetObjScript : MonoBehaviour
{
    public float shakeAmount = 0.05f;
    bool isShake;
    Vector3 firstPos;
    Vector3 localScale;
    float scaleSize = 0.6f;

    // Start is called before the first frame update
    void Start()
    {
        firstPos = this.transform.localPosition;
        localScale = this.transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isShake) return;
        Vector3 pos = firstPos + Random.insideUnitSphere * shakeAmount;
        pos.y = transform.localPosition.y;
        transform.localPosition = pos;
        Invoke("ShakeStop", 1.0f);
        if(this.transform.localScale.z <= localScale.z*0.3f || this.transform.localScale.y<= localScale.z * 0.3f)
        {
            Destroy(this.gameObject);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag=="ball")
        {
            isShake = true;
            this.transform.localScale = new Vector3(this.transform.localScale.x* scaleSize, this.transform.localScale.y * scaleSize, this.transform.localScale.z * scaleSize);
        }
    }
    void ShakeStop()
    {
        isShake = false;
        firstPos.y = transform.localPosition.y;
        transform.localPosition = firstPos;
    }
}
