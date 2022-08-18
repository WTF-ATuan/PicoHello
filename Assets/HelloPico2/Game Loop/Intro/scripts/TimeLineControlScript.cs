using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimeLineControlScript : MonoBehaviour
{
    //public PlayableDirector PauseTimeDirector;
    public string SingleType;
    public GameObject showTimeLine;
    public GameObject hideTimeLine;
    public int waitTime;
    public bool isString;
    public string showTLName;
    public string hideTLName;
    public bool isCheck;
    
    // Start is called before the first frame update
    void Start()
    {
        if (isCheck)
        {
            StartCoroutine(WaitTimer());
        }
        
    }
    IEnumerator WaitTimer()
    {
        yield return new WaitForSeconds(waitTime);
        
        PlayTimeLineController();
    }

    public void PlayTimeLineController()
    {
        if (!isString)
        {
            if (showTimeLine != null)
            {
                showTimeLine.SetActive(true);
            }
            if (hideTimeLine != null)
            {
                hideTimeLine.SetActive(false);
            }
            gameObject.SetActive(false);
        }
        else
        {
            GameObject showTimeLineObj = GameObject.Find(showTLName);
            GameObject hideTimeLineObj = GameObject.Find(hideTLName);

            if (showTimeLineObj != null)
            {
                showTimeLineObj.SetActive(true);
            }
            if (hideTimeLineObj != null)
            {
                hideTimeLineObj.SetActive(false);
            }
            gameObject.SetActive(false);
        }
        //PauseTimeDirector.Play();
        //TimeDirector.playableGraph.GetRootPlayable(0).SetSpeed(1);
    }

}
