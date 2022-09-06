using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMeetGuideScript : MonoBehaviour
{
    int meetTypeRandom;
    public GameObject[] meetList;
    // Start is called before the first frame update
    void Start()
    {
        meetTypeRandom = Random.Range(0, 2);
        meetList[meetTypeRandom].SetActive(true);
        Debug.Log(meetList[meetTypeRandom].name);
    }

}
