using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [HideInInspector]
    public bool Enable_Env; //(暫無功能)

    [Range(0, 10)]
    public float Env_Speed = 0.5f;

    [Range(0.01f,1f)]
    public float Lerp_Speed = 0.5f;

    Anime_Play Bos_Explosion;

    [Tooltip("Transition: Cloud Tunnel")]
    public Anime_Play CloudPassThrough;

    [Tooltip("Transition: ToReality")]
    public Anime_Play Boss_Explosion;

    //關卡
    public TheLevel The_Level;
    #region
    [System.Serializable]
    public struct TheLevel
    {
        public GameObject[] The_Level;
    }
    #endregion

    //關卡發射器GetComponent
    public _Level Level_0, Level_1, Level_2, Level_3;
    #region
    [System.Serializable]
    public struct _Level
    {
        public ObjectPool_Spawner[] Spawner;
    }
    #endregion

    private void OnEnable()
    {
        Now_Level = The_Level.The_Level[0];
    }


    void Update()
    {
        Level_switch();
    }



    void Level_Spawner_Switch(_Level level ,bool Env_Emit, float Speed)
    {
        for (int i = 0; i < level.Spawner.Length; i++)
        {
            level.Spawner[i].Emission = Env_Emit;
            level.Spawner[i].SeedSpeed = Mathf.Lerp(level.Spawner[i].SeedSpeed, Speed, Lerp_Speed);
        }
    }


    //穿越雲洞用的前後關卡參數
    GameObject Now_Level;
    void CloudPassThrough_Level_Switch(GameObject now_Level)
    {
        CloudPassThrough.Pre_Level = Now_Level;   Boss_Explosion.Pre_Level = Now_Level;
        Now_Level = now_Level;
        CloudPassThrough.Next_Level = Now_Level;  Boss_Explosion.Next_Level = Now_Level;
    }

    int pre_level = 0;
    int now_level = 0;

    _Level pre_Level;
    _Level now_Level;
    void Level_switch()
    {

        switch (level)
        {

            //Level_0
            case Level._0_Level_Intro:

                Level_Spawner_Switch(Level_0, true, Env_Speed);

                now_level = 0;
                if (pre_level != now_level)
                {
                    Level_Spawner_Switch(Level_1, false, 50);  //穿越加速，並關閉Emission
                    CloudPassThrough_Level_Switch(The_Level.The_Level[0]);
                    CloudPassThrough.enabled = true;
                    pre_level = now_level;
                }

                break;


            //Level_1
            case Level._1_Level_1:

                Level_Spawner_Switch(Level_1, true, Env_Speed);  //可調整當前關卡的速度

                now_level = 1;
                if (pre_level != now_level)
                {
                    Level_Spawner_Switch(Level_0, false, 50);  //穿越加速，並關閉Emission
                    CloudPassThrough_Level_Switch(The_Level.The_Level[1]);
                    CloudPassThrough.enabled = true;
                    pre_level = now_level;
                }

                break;


            //Level_2
            case Level._2_Level_2:

                Level_Spawner_Switch(Level_2, true, Env_Speed);  //可調整當前關卡的速度

                now_level = 2;
                if (pre_level != now_level)
                {
                    Level_Spawner_Switch(Level_1, false, 50);  //穿越加速，並關閉Emission
                    CloudPassThrough_Level_Switch(The_Level.The_Level[2]);
                    CloudPassThrough.enabled = true;
                    pre_level = now_level;
                }

                break;



            //Level_3
            case Level._3_Level_3:

                Level_Spawner_Switch(Level_3, true, Env_Speed);  //可調整當前關卡的速度

                now_level = 3;
                if (pre_level != now_level)
                {
                    Level_Spawner_Switch(Level_2, false, 50);  //穿越加速，並關閉Emission
                    CloudPassThrough_Level_Switch(The_Level.The_Level[3]);
                    CloudPassThrough.enabled = true;
                    pre_level = now_level;
                }

                break;



            //Level_3_ToReality
            case Level._4_Level_3_ToReality:

                now_level = 4;
                if (pre_level != now_level)
                {
                    CloudPassThrough_Level_Switch(The_Level.The_Level[4]);  //Boss大爆炸
                    Boss_Explosion.enabled = true;
                    pre_level = now_level;
                }

                break;




            //Level_4
            case Level._5_ToLevel_4_BackToGame:

                now_level = 5;
                if (pre_level != now_level)
                {
                    CloudPassThrough_Level_Switch(The_Level.The_Level[5]);
                    CloudPassThrough.enabled = true;
                    pre_level = now_level;
                }

                break;



            //Level_4_CloudBright
            case Level._6_Level_4_CloudBright:

                break;



            //Level_5
            case Level._7_ToLevel_5_OutCloud:

                now_level = 6;
                if (pre_level != now_level)
                {
                    CloudPassThrough_Level_Switch(The_Level.The_Level[6]);
                    CloudPassThrough.enabled = true;
                    pre_level = now_level;
                }

                break;


        }
    }

    private static class ShaderIDs
    {
        internal static readonly int 

              SkyColor     = Shader.PropertyToID   ("_SkyColor"),
                Color      = Shader.PropertyToID     ("_Color"),
             ShadowColor   = Shader.PropertyToID  ("_ShadowColor"),
                 Far       = Shader.PropertyToID      ("_Far"),
            FarSmoothStep  = Shader.PropertyToID ("_FarSmoothStep"),
             FarPosition   = Shader.PropertyToID  ("_FarPosition"),
              SkyFarPos    = Shader.PropertyToID   ("_SkyFarPos"),
              ViewFarPos   = Shader.PropertyToID   ("_ViewFarPos"),
               ViewYPos    = Shader.PropertyToID    ("_ViewYPos"),
            GroundFogWidth = Shader.PropertyToID ("_GroundFogWidth"),
             TopGlowFogPos = Shader.PropertyToID  ("_TopGlowFogPos"),
               UVColor     = Shader.PropertyToID    ("_UVColor");
    }



    //漸層轉換顏色
    void M2M_Color(int PropertyID, Material M1, Material M2, float speed)
    {
        M1.SetColor(PropertyID, Color.Lerp(M1.GetColor(PropertyID), M2.GetColor(PropertyID), speed));
    }
    //漸層轉換數值
    void M2M_Float(int PropertyID, Material M1, Material M2, float speed)
    {
        M1.SetFloat(PropertyID, Mathf.Lerp(M1.GetFloat(PropertyID), M2.GetFloat(PropertyID), speed));
    }

    //景物
    void Mat2Mat_Env (Material M1 , Material M2 ,float speed)
    {
        M2M_Color(ShaderIDs.SkyColor, M1, M2, speed);
        M2M_Color(ShaderIDs.Color, M1, M2, speed);
        M2M_Color(ShaderIDs.ShadowColor, M1, M2, speed);
        M2M_Float(ShaderIDs.Far, M1, M2, speed);
        M2M_Float(ShaderIDs.FarSmoothStep, M1, M2, speed);
        M2M_Float(ShaderIDs.FarPosition, M1, M2, speed);
        M2M_Float(ShaderIDs.SkyFarPos, M1, M2, speed);
        M2M_Float(ShaderIDs.ViewFarPos, M1, M2, speed);
        M2M_Float(ShaderIDs.ViewYPos, M1, M2, speed);
        M2M_Float(ShaderIDs.GroundFogWidth, M1, M2, speed);
        M2M_Float(ShaderIDs.TopGlowFogPos, M1, M2, speed);
        M2M_Color(ShaderIDs.UVColor, M1, M2, speed);
    }
    //雲材質
    void Mat2Mat_Cloud(Material M1, Material M2, float speed)
    {
        M2M_Color(ShaderIDs.Color, M1, M2, speed);
        M2M_Color(ShaderIDs.ShadowColor, M1, M2, speed);
        M2M_Color(ShaderIDs.SkyColor, M1, M2, speed);
        M2M_Float(ShaderIDs.ViewFarPos, M1, M2, speed);
    }
    //天空球
    void Mat2Mat_Sky(Material M1, Material M2, float speed)
    {
        M2M_Color(ShaderIDs.SkyColor, M1, M2, speed);
        M2M_Color(ShaderIDs.ShadowColor, M1, M2, speed);
        M2M_Float(ShaderIDs.SkyFarPos, M1, M2, speed);
    }

}
