using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllLevelChecking : MonoBehaviour
{
    Level_FadeController lv;

    void Start()
    {
        lv = GetComponent<Level_FadeController>();
        StartCoroutine(ABC());
    }

    IEnumerator ABC()
    {
        yield return new WaitForSeconds(7);

        lv.level = Level_FadeController.Level._1_Level_1;

        yield return new WaitForSeconds(15);

        lv.level = Level_FadeController.Level._2_Level_2;

        yield return new WaitForSeconds(45);

        lv.level = Level_FadeController.Level._3_Level_3;

        yield return new WaitForSeconds(15);

        lv.level = Level_FadeController.Level._4_Level_3_ToReality;

        yield return new WaitForSeconds(10);

        lv.level = Level_FadeController.Level._5_ToLevel_4_BackToGame;

        yield return new WaitForSeconds(15);

        lv.level = Level_FadeController.Level._6_Level_4_CloudBright;

        yield return new WaitForSeconds(15);

        lv.level = Level_FadeController.Level._7_ToLevel_5_OutCloud;

    }
}
