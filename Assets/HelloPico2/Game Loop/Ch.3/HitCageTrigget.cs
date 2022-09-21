using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HitCageTrigget : MonoBehaviour
{
    //public GameObject[] getTriggetList;
    int getListCount;
    public int RandomTime;
    float countNum;
    public bool isFadeIn;
    // Start is called before the first frame update
    
    void ScaleFadeIn()
    {
        transform.localScale = Vector3.zero;
        isFadeIn=true;
    }
    // Update is called once per frame
    void Update()
    {
        if (isFadeIn)
        {
            //transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, 0.2f);
            transform.DOScale(1, 2);

            if (transform.localScale.x == 1)
            {
                isFadeIn = false;
            }
        }
        /*
        //transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, 0.02f);
        countNum += Time.deltaTime;
        if (countNum > RandomTime)
        {
            
        }*/
    }
    public void OnTriggerEnter(Collider other)
    {
        Debug.Log(gameObject.name);
        if (other.CompareTag("Player"))
        {

            //transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, 0.02f);
            Debug.Log(gameObject.name);

        }
    }
}
