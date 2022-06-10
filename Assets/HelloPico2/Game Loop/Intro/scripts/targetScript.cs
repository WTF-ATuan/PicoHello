using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class targetScript : MonoBehaviour
{
    public GameObject tragetObj;
    public TargetItem_SO menuCheck;
    public GameObject hideObj;
    public GameObject[] showObj;
    public int checkHeld;
    public bool isOnTrigget;
    public bool isTimeLine;
    TimeLineControlScript _timeLineControl;
    
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
            
        if(menuCheck.targetItemHeld == checkHeld)
        {
                transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, 0.1f);
            if (isTimeLine)
            {
                showObj[0].SetActive(true);
                hideObj.SetActive(false);
            }
            Destroy(gameObject, 3); 
        }
    }
    
    public void AddItemHeld()
    {
        menuCheck.targetItemHeld += 1;
    }

    IEnumerator setActiveObj()
    {
        yield return new WaitForSeconds(3);
        Destroy(gameObject, 1);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && isOnTrigget)
        {
            menuCheck.targetItemHeld += 1;
            isCheckSel = true;
        }
    }
}
