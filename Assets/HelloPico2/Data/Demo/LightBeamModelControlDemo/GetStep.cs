using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetStep : MonoBehaviour
{
    public EnemyScore_SO _getScore;
    public int stepCounter;
    public GameObject[] openList;
    int lenghtNum;
    public bool isStep;


    private void Start()
    {
        lenghtNum = openList.Length;

    }
    private void Update()
    {
        if (isStep && _getScore.getStepHit == stepCounter)
        {
            for(int i = 0; i < lenghtNum; i++)
            {
                openList[i].SetActive(true);
            }
        }
        else if (!isStep && _getScore.getHit == stepCounter)
        {
            for (int i = 0; i < lenghtNum; i++)
            {
                openList[i].SetActive(true);
            }
        }
    }
    public void GetBall()
    {
        _getScore.getStepHit += 1;
    }
    public  void HitTarget()
    {
        _getScore.getHit += 1;
    }


}
