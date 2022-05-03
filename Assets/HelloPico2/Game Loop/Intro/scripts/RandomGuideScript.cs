using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomGuideScript : MonoBehaviour
{
    public GameObject[] GuideList;
    int getRandom;
    // Start is called before the first frame update
    void Start()
    {
        getRandom =Random.Range(0, GuideList.Length);
        GuideList[getRandom].SetActive(true);
    }


}
