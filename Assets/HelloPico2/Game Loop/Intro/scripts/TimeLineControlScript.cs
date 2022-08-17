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
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WaitTimer());
        
    }
    IEnumerator WaitTimer()
    {
        yield return new WaitForSeconds(waitTime);
        
        PlayTimeLineController();
    }



    public void PlayTimeLineController()
    {
        Debug.Log(waitTime);
        if (showTimeLine != null)
        {
            showTimeLine.SetActive(true);
        }
        if (hideTimeLine != null)
        {
            hideTimeLine.SetActive(false);
        }
        gameObject.SetActive(false);
        //PauseTimeDirector.Play();
        //TimeDirector.playableGraph.GetRootPlayable(0).SetSpeed(1);
    }

}
