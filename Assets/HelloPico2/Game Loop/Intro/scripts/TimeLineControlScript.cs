using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimeLineControlScript : MonoBehaviour
{
    public PlayableDirector PauseTimeDirector;
    public GameObject StartTimeDirector;
    
    // Start is called before the first frame update
    void Start()
    {
        PauseTimeDirector = PauseTimeDirector.GetComponent<PlayableDirector>();
    }

    public void PauseTimeLineController()
    {
        PauseTimeDirector.Pause();
        StartTimeDirector.SetActive(true);
        //TimeDirector.playableGraph.GetRootPlayable(0).SetSpeed(0);
    }
    public void PlayTimeLineController()
    {
        PauseTimeDirector.Play();
        StartTimeDirector.SetActive(false);
        //TimeDirector.playableGraph.GetRootPlayable(0).SetSpeed(1);
    }

}
