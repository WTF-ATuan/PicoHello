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
    public bool isTutorial;
    // Start is called before the first frame update
    void Start()
    {
        //PauseTimeDirector = PauseTimeDirector.GetComponent<PlayableDirector>();
    }

    public void PauseTimeLineController()
    {
        //PauseTimeDirector.Pause();
        showTimeLine.SetActive(false);
        if (isTutorial)
        {
            hideTimeLine.SetActive(true);
        }
        //TimeDirector.playableGraph.GetRootPlayable(0).SetSpeed(0);
    }
    public void PlayTimeLineController()
    {
        showTimeLine.SetActive(true);
        if (isTutorial)
        {
            hideTimeLine.SetActive(false);
        }
        //PauseTimeDirector.Play();
        //TimeDirector.playableGraph.GetRootPlayable(0).SetSpeed(1);
    }

}
