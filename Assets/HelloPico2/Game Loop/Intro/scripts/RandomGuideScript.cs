using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomGuideScript : MonoBehaviour
{
    public GameObject[] GuideList;
    public TargetItem_SO _targetItem_SO;
    public bool isCheck;
    int getRandom;
    // Start is called before the first frame update
    private void Start()
    {
        if (isCheck)
        {
            getRandom = Random.Range(0, GuideList.Length);
            GuideList[getRandom].SetActive(true);
        }
        else
        {
            RandomGuide();
        }
        
    }
    public void RandomGuide()
    {
        if (_targetItem_SO.targetItemHeld == 1)
        {
            getRandom = Random.Range(0, GuideList.Length);
            GuideList[getRandom].SetActive(true);
        }
        
    }

}
