using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cloeseLevel_Env : MonoBehaviour
{
    Level_FadeController _level_FadeController;
    // Start is called before the first frame update
    public GameObject setLevel;

    public void Start()
    {
        setLevel.GetComponent<Level_FadeController>().Enable_Env = false;
    }


}
