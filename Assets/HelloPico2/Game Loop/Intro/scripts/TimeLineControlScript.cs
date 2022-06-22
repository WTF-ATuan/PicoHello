using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimeLineControlScript : MonoBehaviour
{
    //public PlayableDirector PauseTimeDirector;
    public string SingleType;
    public GameObject StartTimeDirector;
    public GameObject TutorialObj;
    public bool isTutorial;
    // Start is called before the first frame update
    void Start()
    {
        //PauseTimeDirector = PauseTimeDirector.GetComponent<PlayableDirector>();
    }

    public void PauseTimeLineController()
    {
        //PauseTimeDirector.Pause();
        StartTimeDirector.SetActive(false);
        if (isTutorial)
        {
            TutorialObj.SetActive(true);
        }
        //TimeDirector.playableGraph.GetRootPlayable(0).SetSpeed(0);
    }
    public void PlayTimeLineController()
    {
        StartTimeDirector.SetActive(true);
        if (isTutorial)
        {
            TutorialObj.SetActive(false);
        }
        //PauseTimeDirector.Play();
        //TimeDirector.playableGraph.GetRootPlayable(0).SetSpeed(1);
    }

}
