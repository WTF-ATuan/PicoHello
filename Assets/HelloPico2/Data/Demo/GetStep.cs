using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetStep : MonoBehaviour
{
    public EnemyScore_SO _getScore;
    
    public void GetBall()
    {
        _getScore.getStepHit += 1;
    }
    public  void HitTarget()
    {
        _getScore.getHit += 1;
    }


}
