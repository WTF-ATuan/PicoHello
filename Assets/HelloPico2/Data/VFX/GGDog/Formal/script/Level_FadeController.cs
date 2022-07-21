using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class Level_FadeController : MonoBehaviour
{
    public enum Level
    {
        _0_intro_黑暗中,
        _1_第一關_精靈樂園,
        _2_第二關_樹根,
        _3_第三關_紫色遺跡,
        _4_轉到現實_Seethrough,
        _5_第四關_穿越特效進入_雲洞,
        _6_顏色漸亮_雲洞,
        _7_衝出雲洞_大結局
    }
    public Level level;

    [Range(0, 0.001f)]
    public float speed;
    public bool Enable_Env = true;

    [Header("---------------------------------------")]
    //Shader參數名稱
    public Shader_properties Shader_Properties;
    #region
    [System.Serializable]
    public struct Shader_properties
    {
        public string[] Sky_VarNames_Color;
        public string[] Sky_VarNames_Float;

        public string[] Ground_VarNames_Color;
        public string[] Ground_VarNames_Float;

        public string[] Env_VarNames_Color;
        public string[] Env_VarNames_Float;

    }
    #endregion


    [Header("---------------------------------------")]

    //對應關卡的目標材質
    public _Level BaseMaterial,  Level_0, Level_1, Level_2, Level_3;
    #region
    [System.Serializable]
    public struct _Level
    {
        public Material Sky;
        public Material Ground;
        public Material Env;
        public Material Env_far;
        public Material Env_near;
        public GameObject Env_Spawner;
        [HideInInspector]
        public ObjectPool_Spawner[] Spawner_Emission;
    }
    #endregion

    [Header("---------------------------------------")]

    public GameObject Level_4;

    
    void Env_StartUp(_Level Lv)
    {
        //Get每個關卡的場景發射器的開關
        if (Level_0.Env_Spawner)
        {
            for (int i = 0; i < Level_0.Env_Spawner.transform.childCount; i++)
            {
                Level_0.Spawner_Emission[i].Emission = false;
            }
        }
        if (Level_1.Env_Spawner)
        {
            for (int i = 0; i < Level_1.Env_Spawner.transform.childCount; i++)
            {
                Level_1.Spawner_Emission[i].Emission = false;
            }
        }
        if (Level_2.Env_Spawner)
        {
            for (int i = 0; i < Level_2.Env_Spawner.transform.childCount; i++)
            {
                Level_2.Spawner_Emission[i].Emission = false;
            }
        }
        if (Level_3.Env_Spawner)
        {
            for (int i = 0; i < Level_3.Env_Spawner.transform.childCount; i++)
            {
                Level_3.Spawner_Emission[i].Emission = false;
            }
        }

        if (Enable_Env && Lv.Env_Spawner)
        {
            for (int i = 0; i < Lv.Env_Spawner.transform.childCount; i++)
            {
                Lv.Spawner_Emission[i].Emission = true;
            }
        }

    }

    void OnEnable()
    {

        if (!GetComponent<ParticleSystem>())
        {
            gameObject.AddComponent<ParticleSystem>();
            ParticleSystem _p = GetComponent<ParticleSystem>();
            _p.hideFlags = HideFlags.HideInInspector;
        }

        Level_0.Spawner_Emission = new ObjectPool_Spawner[10];
        Level_1.Spawner_Emission = new ObjectPool_Spawner[10];
        Level_2.Spawner_Emission = new ObjectPool_Spawner[10];
        Level_3.Spawner_Emission = new ObjectPool_Spawner[10];


        //Get每個關卡的場景發射器的開關
        if (Level_0.Env_Spawner)
        {
            for (int i = 0; i < Level_0.Env_Spawner.transform.childCount; i++)
            {
                Level_0.Spawner_Emission[i] = Level_0.Env_Spawner.transform.GetChild(i).GetComponent<ObjectPool_Spawner>();
            }
        }
        if (Level_1.Env_Spawner)
        {
            for (int i = 0; i < Level_1.Env_Spawner.transform.childCount; i++)
            {
                Level_1.Spawner_Emission[i] = Level_1.Env_Spawner.transform.GetChild(i).GetComponent<ObjectPool_Spawner>();
            }
        }
        if (Level_2.Env_Spawner)
        {
            for (int i = 0; i < Level_2.Env_Spawner.transform.childCount; i++)
            {
                Level_2.Spawner_Emission[i] = Level_2.Env_Spawner.transform.GetChild(i).GetComponent<ObjectPool_Spawner>();
            }
        }
        if (Level_3.Env_Spawner)
        {
            for (int i = 0; i < Level_3.Env_Spawner.transform.childCount; i++)
            {
                Level_3.Spawner_Emission[i] = Level_3.Env_Spawner.transform.GetChild(i).GetComponent<ObjectPool_Spawner>();
            }
        }


        Level_switch();
        Level_4.SetActive(false);

    }
    
    void Update()
    {
        Level_switch();
    }

    string Properties_name;

    //天空球轉換
    void ColorFade_Sky(Material m)
    {
        //Color類型的參數
        for (int i = 0; i < Shader_Properties.Sky_VarNames_Color.Length; i++)
        {
            Properties_name = Shader_Properties.Sky_VarNames_Color[i];
            BaseMaterial.Sky.SetColor(Properties_name, Color.Lerp(BaseMaterial.Sky.GetColor(Properties_name), m.GetColor(Properties_name), speed));
        }

        //Float類型的參數
        for (int i = 0; i < Shader_Properties.Sky_VarNames_Float.Length; i++)
        {
            Properties_name = Shader_Properties.Sky_VarNames_Float[i];
            BaseMaterial.Sky.SetFloat(Properties_name, Mathf.Lerp(BaseMaterial.Sky.GetFloat(Properties_name), m.GetFloat(Properties_name), speed));
        }
    }
    //場景轉換
    void ColorFade_Env(Material m, Material m_far, Material m_near)
    {
        //Color類型的參數
        for (int i = 0; i < Shader_Properties.Env_VarNames_Color.Length; i++)
        {
            Properties_name = Shader_Properties.Env_VarNames_Color[i];
            BaseMaterial.Env.SetColor(Properties_name, Color.Lerp(BaseMaterial.Env.GetColor(Properties_name), m.GetColor(Properties_name), speed));
            BaseMaterial.Env_far.SetColor(Properties_name, Color.Lerp(BaseMaterial.Env_far.GetColor(Properties_name), m_far.GetColor(Properties_name), speed));
            BaseMaterial.Env_near.SetColor(Properties_name, Color.Lerp(BaseMaterial.Env_near.GetColor(Properties_name), m_near.GetColor(Properties_name), speed));
        }

        //Float類型的參數
        for (int i = 0; i < Shader_Properties.Env_VarNames_Float.Length; i++)
        {
            Properties_name = Shader_Properties.Env_VarNames_Float[i];
            BaseMaterial.Env.SetFloat(Properties_name, Mathf.Lerp(BaseMaterial.Env.GetFloat(Properties_name), m.GetFloat(Properties_name), speed));
            BaseMaterial.Env_far.SetFloat(Properties_name, Mathf.Lerp(BaseMaterial.Env_far.GetFloat(Properties_name), m_far.GetFloat(Properties_name), speed));
            BaseMaterial.Env_near.SetFloat(Properties_name, Mathf.Lerp(BaseMaterial.Env_near.GetFloat(Properties_name), m_near.GetFloat(Properties_name), speed));
        }
    }

    //水面轉換
    void ColorFade_Ground(Material m)
    {
        //Color類型的參數
        for (int i = 0; i < Shader_Properties.Ground_VarNames_Color.Length; i++)
        {
            Properties_name = Shader_Properties.Ground_VarNames_Color[i];
            BaseMaterial.Ground.SetColor(Properties_name, Color.Lerp(BaseMaterial.Ground.GetColor(Properties_name), m.GetColor(Properties_name), speed));
        }

        //Float類型的參數
        for (int i = 0; i < Shader_Properties.Ground_VarNames_Float.Length; i++)
        {
            Properties_name = Shader_Properties.Ground_VarNames_Float[i];
            BaseMaterial.Ground.SetFloat(Properties_name, Mathf.Lerp(BaseMaterial.Ground.GetFloat(Properties_name), m.GetFloat(Properties_name), speed));
        }
    }

    

    void Level_switch()
    {
        switch (level)
        {
            case Level._0_intro_黑暗中:
                Level_4.SetActive(false);
                ColorFade_Sky(Level_0.Sky);
                ColorFade_Ground(Level_0.Ground);
                ColorFade_Env(Level_0.Env, Level_0.Env_far, Level_0.Env_near);
                Env_StartUp(Level_0);
                break;


            case Level._1_第一關_精靈樂園:
                Level_4.SetActive(false);
                ColorFade_Sky(Level_1.Sky);
                ColorFade_Ground(Level_1.Ground);
                ColorFade_Env(Level_1.Env, Level_1.Env_far, Level_1.Env_near);
                Env_StartUp(Level_1);
                break;


            case Level._2_第二關_樹根:
                Level_4.SetActive(false);
                ColorFade_Sky(Level_2.Sky);
                ColorFade_Ground(Level_2.Ground);
                ColorFade_Env(Level_2.Env, Level_2.Env_far, Level_2.Env_near);
                Env_StartUp(Level_2);
                break;


            case Level._3_第三關_紫色遺跡:
                Level_4.SetActive(false);
                ColorFade_Sky(Level_3.Sky);
                ColorFade_Ground(Level_3.Ground);
                ColorFade_Env(Level_3.Env, Level_3.Env_far, Level_3.Env_near);
                Env_StartUp(Level_3);
                break;


            case Level._4_轉到現實_Seethrough:
                Enable_Env = false;
                Level_4.SetActive(false);
                break;
                
            case Level._5_第四關_穿越特效進入_雲洞:
                Enable_Env = false;
                Level_4.SetActive(true);
                break;


            case Level._6_顏色漸亮_雲洞:
                break;


            case Level._7_衝出雲洞_大結局:
                break;
                
        }
    }
}
