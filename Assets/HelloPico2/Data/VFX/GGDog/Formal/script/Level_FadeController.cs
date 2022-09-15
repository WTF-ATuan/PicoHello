using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Level_FadeController : MonoBehaviour
{
    public enum Level
    {
        _0_Level_Intro,          //黑暗洞穴
        _1_Level_1,              //精靈的樂園
        _2_Level_2,              //迷霧樹根
        _3_Level_3,              //危險遺跡
        _4_Level_3_ToReality,    //危險遺跡: 轉現實(Boss開大)
        _5_ToLevel_4_BackToGame, //回到遊戲: 雲洞中
        _6_Level_4_CloudBright,  //雲洞: 打完Boss之後，整體顏色變明亮
        _7_ToLevel_5_OutCloud    //飛出雲洞: 最後結局場景
    }
    public Level level;

   // [Range(0, 0.001f)]
    public float speed;
    public bool Enable_Env = true;
    [Range(50, 175)]
    public float Env_Speed = 50;

    //穿出雲洞的速度調整
    [Range(1, 50)]
    public float Level4To5_OutCloud_Speed = 1;

    //最頂層的光(ZTestAlways)的透明度
    [Range(0, 1)]
    public float TopGlow_Alpha = 1;

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

    //關卡限定的粒子系統
    public _SpecificPartical SpecificPartical;
    #region
    [System.Serializable]
    public struct _SpecificPartical
    {
        public ParticleSystem[] Partical;
    }
    #endregion
    [Header("---------------------------------------")]

    //粒子系統材質
    public _Partical ParticalMaterial;
    #region
    [System.Serializable]
    public struct _Partical
    {
        public Material[] Partical;
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
        public Material Env_stuff;
        public GameObject Env_Spawner;
        [HideInInspector]
        public ObjectPool_Spawner[] Spawner_Emission;

        public Color BG_GlowPoint;
        public Color Glory_circle;
        public Color Foggy;
        public Color Particle_defocus;
        public ParticleSystem Particle;
    }
    #endregion

    [Header("---------------------------------------")]

    public GameObject Level_3_ToReality_Sky;
    public GameObject Level_3_ToReality_Ground;
    public Material m_ToReality_Sky;
    public Material m_ToReality_Ground;
    public Animation ToReality_Animation;
    public GameObject Space_CameraEffect;   //鏡頭周圍特效

    public GameObject Level_4;
    public Material Level_4_CloudTunnel;
    public Material Level_4_stone;
    public Material Level_4_GlowFog;
    public Material Level_4_GlowLine;

    public GameObject stone;
    public GameObject stone_big;
    public GameObject stone_piece;
    public GameObject tunnel;
    
    public GameObject Level_5;
    public GameObject[] Level_4_Close;
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

                Lv.Spawner_Emission[i].SeedSpeed = Mathf.Lerp(Lv.Spawner_Emission[i].SeedSpeed, Env_Speed,0.005f);
                
            }
        }

    }
    Material Space_CameraEffect_Material;
    void OnEnable()
    {
        //重置頂光
        ParticalMaterial.Partical[0].SetFloat("_Alpha", 1);
        //景物水反射光
      //  BaseMaterial.Env.SetFloat("_Reflect",1.5f);
      //  BaseMaterial.Env_stuff.SetFloat("_Reflect", 1.5f);

        //轉現實之後原本的天空水面會變成ToReally的材質，每次重置就換回來原本的
        Level_3_ToReality_Sky.GetComponent<MeshRenderer>().sharedMaterial = BaseMaterial.Sky;
            Level_3_ToReality_Ground.GetComponent<MeshRenderer>().sharedMaterial = BaseMaterial.Ground;

        //現實關的鏡頭周圍特效
            Space_CameraEffect.SetActive(false);
            Space_CameraEffect_Material = Space_CameraEffect.GetComponent<MeshRenderer>().sharedMaterial;
            Space_CameraEffect_Material.SetFloat("_Dis", 1);

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

        Level_4.SetActive(false);
        Level_5.SetActive(false);
        Level_switch();
    }

    void Update()
    {
        Level_switch();

        //直接調整頂光透明度
        ParticalMaterial.Partical[0].SetFloat("_Alpha", ParticalMaterial.Partical[0].GetFloat("_Alpha")*TopGlow_Alpha);
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
            BaseMaterial.Env_stuff.SetColor(Properties_name, Color.Lerp(BaseMaterial.Env_stuff.GetColor(Properties_name), m.GetColor(Properties_name), speed));
        }

        //Float類型的參數
        for (int i = 0; i < Shader_Properties.Env_VarNames_Float.Length; i++)
        {
            Properties_name = Shader_Properties.Env_VarNames_Float[i];
            BaseMaterial.Env.SetFloat(Properties_name, Mathf.Lerp(BaseMaterial.Env.GetFloat(Properties_name), m.GetFloat(Properties_name), speed));
            BaseMaterial.Env_far.SetFloat(Properties_name, Mathf.Lerp(BaseMaterial.Env_far.GetFloat(Properties_name), m_far.GetFloat(Properties_name), speed));
            BaseMaterial.Env_near.SetFloat(Properties_name, Mathf.Lerp(BaseMaterial.Env_near.GetFloat(Properties_name), m_near.GetFloat(Properties_name), speed));
            BaseMaterial.Env_stuff.SetFloat(Properties_name, Mathf.Lerp(BaseMaterial.Env_stuff.GetFloat(Properties_name), m.GetFloat(Properties_name), speed));
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

    //關卡限定的粒子特效
    void Specific_Partical(int k)
    {
        for (int i = 0; i < SpecificPartical.Partical.Length; i++)
        {
            ParticleSystem ps = SpecificPartical.Partical[i];
            var em = ps.emission;
            em.enabled = false;

            if(i==k) { em.enabled = true; }
        }
    }


    //粒子的材質顏色轉換
    void Level_Partical(Color[] _Color)
    {
        for (int i = 0; i < ParticalMaterial.Partical.Length; i++)
        {
            ParticalMaterial.Partical[i].SetColor("_Color", Color.Lerp(ParticalMaterial.Partical[i].GetColor("_Color"), _Color[i], speed));
        }
    }

    //雲洞顏色歸0
    void Level_4_ColorFade_Set()
    {
        Level_4_CloudTunnel.SetFloat("_ColorLerp", 0);
        Level_4_stone.SetFloat("_ColorLerp", 0);
    }
    //雲洞顏色漸變，由黑暗到明亮
    void Level_4_ColorFade()
    {
        Level_4_CloudTunnel.SetFloat("_ColorLerp", Mathf.Lerp(Level_4_CloudTunnel.GetFloat("_ColorLerp"), 1, speed));
        Level_4_stone.SetFloat("_ColorLerp", Mathf.Lerp(Level_4_stone.GetFloat("_ColorLerp"), 1, speed));
    }

    Color[] ps_color = new Color[4];
    void Level_switch()
    {
        switch (level)
        {
            case Level._0_Level_Intro:

                Specific_Partical(0);

                ps_color[0] = Level_0.BG_GlowPoint;
                ps_color[1] = Level_0.Glory_circle;
                ps_color[2] = Level_0.Foggy;
                ps_color[3] = Level_0.Particle_defocus;
                Level_Partical(ps_color);


                ColorFade_Sky(Level_0.Sky);
                ColorFade_Ground(Level_0.Ground);
                ColorFade_Env(Level_0.Env, Level_0.Env_far, Level_0.Env_near);
                Env_StartUp(Level_0);
                break;


            case Level._1_Level_1:

                Specific_Partical(1);

                ps_color[0] = Level_1.BG_GlowPoint;
                ps_color[1] = Level_1.Glory_circle;
                ps_color[2] = Level_1.Foggy;
                ps_color[3] = Level_1.Particle_defocus;
                Level_Partical(ps_color);

                ColorFade_Sky(Level_1.Sky);
                ColorFade_Ground(Level_1.Ground);
                ColorFade_Env(Level_1.Env, Level_1.Env_far, Level_1.Env_near);
                Env_StartUp(Level_1);
                break;


            case Level._2_Level_2:

                Specific_Partical(2);

                ps_color[0] = Level_2.BG_GlowPoint;
                ps_color[1] = Level_2.Glory_circle;
                ps_color[2] = Level_2.Foggy;
                ps_color[3] = Level_2.Particle_defocus;
                Level_Partical(ps_color);

                ColorFade_Sky(Level_2.Sky);
                ColorFade_Ground(Level_2.Ground);
                ColorFade_Env(Level_2.Env, Level_2.Env_far, Level_2.Env_near);
                Env_StartUp(Level_2);
                break;


            case Level._3_Level_3:

                Specific_Partical(3);
                ps_color[0] = Level_3.BG_GlowPoint;
                ps_color[1] = Level_3.Glory_circle;
                ps_color[2] = Level_3.Foggy;
                ps_color[3] = Level_3.Particle_defocus;
                Level_Partical(ps_color);

                ColorFade_Sky(Level_3.Sky);
                ColorFade_Ground(Level_3.Ground);
                ColorFade_Env(Level_3.Env, Level_3.Env_far, Level_3.Env_near);
                Env_StartUp(Level_3);
                break;


            case Level._4_Level_3_ToReality:
                Enable_Env = false;

                //Color類型的參數
                for (int i = 0; i < Shader_Properties.Sky_VarNames_Color.Length; i++)
                {
                    Properties_name = Shader_Properties.Sky_VarNames_Color[i];
                    m_ToReality_Sky.SetColor(Properties_name, BaseMaterial.Sky.GetColor(Properties_name));
                }

                //Float類型的參數
                for (int i = 0; i < Shader_Properties.Sky_VarNames_Float.Length; i++)
                {
                    Properties_name = Shader_Properties.Sky_VarNames_Float[i];
                    m_ToReality_Sky.SetFloat(Properties_name, BaseMaterial.Sky.GetFloat(Properties_name));
                }

                //Color類型的參數
                for (int i = 0; i < Shader_Properties.Ground_VarNames_Color.Length; i++)
                {
                    Properties_name = Shader_Properties.Ground_VarNames_Color[i];
                    m_ToReality_Ground.SetColor(Properties_name, BaseMaterial.Ground.GetColor(Properties_name));
                }

                //Float類型的參數
                for (int i = 0; i < Shader_Properties.Ground_VarNames_Float.Length; i++)
                {
                    Properties_name = Shader_Properties.Ground_VarNames_Float[i];
                    m_ToReality_Ground.SetFloat(Properties_name, BaseMaterial.Ground.GetFloat(Properties_name));
                }

                Level_3_ToReality_Sky.GetComponent<MeshRenderer>().sharedMaterial = m_ToReality_Sky;
                Level_3_ToReality_Ground.GetComponent<MeshRenderer>().sharedMaterial = m_ToReality_Ground;

                ToReality_Animation.Play("ToReality");

                Space_CameraEffect.SetActive(true);

                Space_CameraEffect_Material.SetFloat("_Dis", Mathf.Lerp(Space_CameraEffect_Material.GetFloat("_Dis"), 0.00235f, 0.05f));

                Level_4_GlowFog.SetFloat("_Alpha", 0);
                Level_4_GlowLine.SetFloat("_Alpha", 0);

                break;
                
            case Level._5_ToLevel_4_BackToGame:
                Enable_Env = false;

                Space_CameraEffect.SetActive(false);

                Level_4_ColorFade_Set();
                Level_4.SetActive(true);

                Level_4_GlowFog.SetFloat("_Alpha", Mathf.Lerp(Level_4_GlowFog.GetFloat("_Alpha"), 1, 0.05f));
                Level_4_GlowLine.SetFloat("_Alpha", Mathf.Lerp(Level_4_GlowLine.GetFloat("_Alpha"), 1, 0.05f));


                break;


            case Level._6_Level_4_CloudBright:
                Level_4_ColorFade();

                break;


            case Level._7_ToLevel_5_OutCloud:


                Level_4_GlowFog.SetFloat("_Alpha", Mathf.Lerp(Level_4_GlowFog.GetFloat("_Alpha"), 0, 0.025f));
                Level_4_GlowLine.SetFloat("_Alpha", Mathf.Lerp(Level_4_GlowLine.GetFloat("_Alpha"), 0, 0.025f));


                //光減淡到消失
                ParticalMaterial.Partical[0].SetFloat("_Alpha", Mathf.Lerp(ParticalMaterial.Partical[0].GetFloat("_Alpha"), 0, speed));

                //景物水反射效果
               // BaseMaterial.Env.SetFloat("_Reflect", Mathf.Lerp(BaseMaterial.Env.GetFloat("_Reflect"), 0, speed));

                stone_piece.transform.position -= new Vector3(0, 0, 0.03f)* Level4To5_OutCloud_Speed;
                stone.transform.position -= new Vector3(0, 0, 0.06f) * Level4To5_OutCloud_Speed;
                stone_big.transform.position -= new Vector3(0, 0, 0.06f) * Level4To5_OutCloud_Speed;
                tunnel.transform.position -= new Vector3(0, 0, 0.05f) * Level4To5_OutCloud_Speed;

                Level_5.SetActive(true);

                for(int i=0; i< Level_4_Close.Length;i++)
                {
                    Level_4_Close[i].SetActive(false);
                }


                break;
                
        }
    }
}
