using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class targetScript : MonoBehaviour
{
    public GameObject tragetObj;
    public GameObject[] showObj;
    public bool isDestroy;
    public bool isSetActive;
    
    int showLength;

    public bool isCheckSel;
    
    // Start is called before the first frame update
    void Start()
    {
        showLength = showObj.Length;
    }
    private void Update()
    {
        if (!isCheckSel) return;
            if (showObj[0] == null || showObj[1] == null || showObj[2] == null || showObj[3] == null)
            {
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, 1.0f);
            Destroy(gameObject,1);
            }
            
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if(tragetObj.name== other.name && isDestroy)
        {
            DestroyGet();
        }
        if (tragetObj.name == other.name && isSetActive)
        {
            StartCoroutine(setActiveObj());
        }
    }
    IEnumerator setActiveObj()
    {
        for(int i = 0;i< showLength;i++)
        {
            showObj[i].gameObject.SetActive(true);
            yield return new WaitForSeconds(1);
        }
        DestroyGet();


    }
    public void setActiveShow()
    {
        StartCoroutine(setActiveObj());
    }

    public void DestroyGet()
    {
        Destroy(gameObject);
    }
}
