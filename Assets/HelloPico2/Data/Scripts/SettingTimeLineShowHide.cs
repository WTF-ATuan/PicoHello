using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingTimeLineShowHide : MonoBehaviour
{
    public string settingName;
    public GameObject[] showTimeLine;
    public GameObject[] hideTimeLine;
    public bool isShow;
    public GameObject Board;
    // Start is called before the first frame update
    void Start()
    {
        SettingShowHide();
    }

    private void SettingShowHide()
    {
        foreach (GameObject showTyp in showTimeLine)
        {
            showTyp.SetActive(true);
        }
        foreach (GameObject hideTyp in hideTimeLine)
        {
            hideTyp.SetActive(false);
        }
        if (isShow)
        {
            Board.SetActive(true);
        }
        else
        {
            Board.SetActive(true);
        }
        gameObject.SetActive(false);
    }

}
