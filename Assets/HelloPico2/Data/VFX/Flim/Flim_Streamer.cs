using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class Flim_Streamer : MonoBehaviour
{

    VideoPlayer[] vp = new VideoPlayer[20];


    //�L���S��
    public TheEffect[] cut_scenes_Effect;
    #region
    [System.Serializable]
    public struct TheEffect
    {
        public GameObject Film_Index_GO;       //�n�b�ĴX��v�������e�}�үS��
        [HideInInspector] public int Film_Index;

        public GameObject Effect_GO;    //�L���S�Ī���
        public float InAdvance_Second;  //�v�������e�X��}�үS��
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

        //��l��: �w�]�����Ҧ��v������
        for (int i = 0; i < transform.childCount; i++)
        {
            vp[i].gameObject.SetActive(false);
            vp[i].EnableAudioTrack(0, false);
        }

        //�}�l�p�ɾ�
        StartCoroutine(FilTime());
    }

    bool IsEffect = false;

    //�p�ɾ�
    IEnumerator FilTime()
    {
        for (int i = 0; i < transform.childCount; i++)
        {

            vp[i].gameObject.SetActive(true);

            yield return new WaitForSeconds(0.75f); //�������v���}�Үɽw��

            if (i > 0) { vp[i - 1].gameObject.SetActive(false); } //�����e�@��v��


            IsEffect = false;

            for (int k = 0; k < cut_scenes_Effect.Length; k++)
            {
                //���L���S�Ī��v��
                if (i == cut_scenes_Effect[k].Film_Index)
                {
                    IsEffect = true;
                    yield return new WaitForSeconds((float)(vp[i].length) - 0.75f + cut_scenes_Effect[k].InAdvance_Second);
                    cut_scenes_Effect[k].Effect_GO.SetActive(false);
                    cut_scenes_Effect[k].Effect_GO.SetActive(true);
                    yield return new WaitForSeconds(-cut_scenes_Effect[k].InAdvance_Second);  //�]���e�����(�ɧ�)
                }
            }

            //�L�L���S�Ī��v��
            if (!IsEffect)
            {
                yield return new WaitForSeconds((float)(vp[i].length) - 0.75f);
            }

        }

    }

}
