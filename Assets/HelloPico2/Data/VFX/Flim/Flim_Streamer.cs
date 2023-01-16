using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class Flim_Streamer : MonoBehaviour
{

    VideoPlayer[] vp = new VideoPlayer[20];


    //過場特效
    public TheEffect[] cut_scenes_Effect;
    #region
    [System.Serializable]
    public struct TheEffect
    {
        public GameObject Film_Index_GO;       //要在第幾支影片尾末前開啟特效
        [HideInInspector] public int Film_Index;

        public GameObject Effect_GO;    //過場特效物件
        public float InAdvance_Second;  //影片尾末前幾秒開啟特效
    }
    #endregion


    void Awake()
    {


        for (int k = 0; k < cut_scenes_Effect.Length; k++)
        {
            cut_scenes_Effect[k].Film_Index = cut_scenes_Effect[k].Film_Index_GO.transform.GetSiblingIndex();
        }


        for (int i = 0; i < transform.childCount; i++)
        {
            vp[i] = transform.GetChild(i).GetComponent<VideoPlayer>();
        }

        //初始化: 預設關閉所有影片物件
        for (int i = 0; i < transform.childCount; i++)
        {
            vp[i].gameObject.SetActive(false);
            vp[i].EnableAudioTrack(0, false);
        }

        //開始計時器
        StartCoroutine(FilTime());
    }

    bool IsEffect = false;

    //計時器
    IEnumerator FilTime()
    {
        for (int i = 0; i < transform.childCount; i++)
        {

            vp[i].gameObject.SetActive(true);

            yield return new WaitForSeconds(0.75f); //延遲讓影片開啟時緩衝

            if (i > 0) { vp[i - 1].gameObject.SetActive(false); } //關閉前一支影片


            IsEffect = false;

            for (int k = 0; k < cut_scenes_Effect.Length; k++)
            {
                //有過場特效的影片
                if (i == cut_scenes_Effect[k].Film_Index)
                {
                    IsEffect = true;
                    yield return new WaitForSeconds((float)(vp[i].length) - 0.75f + cut_scenes_Effect[k].InAdvance_Second);
                    cut_scenes_Effect[k].Effect_GO.SetActive(false);
                    cut_scenes_Effect[k].Effect_GO.SetActive(true);
                    yield return new WaitForSeconds(-cut_scenes_Effect[k].InAdvance_Second);  //跑提前的秒數(補完)
                }
            }

            //無過場特效的影片
            if (!IsEffect)
            {
                yield return new WaitForSeconds((float)(vp[i].length) - 0.75f);
            }

        }

    }

}
