using HelloPico2.LevelTool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowHandCheckScript : MonoBehaviour
{
    public GameObject showHandCheck;

    private void Start()
    {
        
    }
    // Start is called before the first frame update

    private void OnTriggerStay(Collider other)
    {
        showHandCheck.SetActive(true);
        showHandCheck.GetComponent<ShackingDetector>().enabled = true;
    }
    private void OnTriggerExit(Collider other)
    {
        showHandCheck.SetActive(false);
        showHandCheck.GetComponent<ShackingDetector>().enabled = false;
    }
}
