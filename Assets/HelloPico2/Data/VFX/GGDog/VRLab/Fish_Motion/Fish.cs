using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{

    public float RunRange = 0.25f;

    Vector3 targetpoint;

    public bool _StayMotion = false;

    public bool _RunAway = false;


    void Awake()
    {
        StartCoroutine(update());

        StartCoroutine(Noise());
        StartCoroutine(StayMotion());
        StartCoroutine(RunAway());

    }

    bool targetpoint_bool = true;

    IEnumerator update()
    {
        while (true)
        {
            yield return new WaitForSeconds(0);

            //進入逃跑
            if (_RunAway)
            {
                _StayMotion = false;

            }
            //恢復Stay
            else if (!_RunAway)
            {
                _StayMotion = true;
            }
        }
    }


    IEnumerator StayMotion()
    {
        while (true)
        {
            yield return new WaitForSeconds(0);

            while (_StayMotion)
            {
                yield return new WaitForSeconds(0);

            }
        }
    }

    float SpeedVary_d;
    IEnumerator RunAway()
    {
        while (true)
        {
            yield return new WaitForSeconds(0);

            while (_RunAway)
            {
                yield return new WaitForSeconds(0);

                if (targetpoint_bool)
                {
                    targetpoint = transform.position * RunRange + Random.insideUnitSphere * Random.Range(0.35f, 1f);

                    SpeedVary_d = RawLength(targetpoint, transform.position);  //觸發當下的距離差(最大值，放分母用)

                    targetpoint_bool = false;
                }

                float SpeedVary = RawLength(targetpoint, transform.position) / SpeedVary_d; //(此距離必定界在0~1之間)

                Vector3 NoisePos = transform.position + NoiseOffSet * 1.85f * SpeedVary * SpeedVary;

                transform.position = Vector3.Lerp(transform.position, NoisePos, 0.15f * SpeedVary );


                transform.position = Vector3.Lerp(transform.position, targetpoint, 0.1f * SpeedVary );


                if (RawLength(targetpoint, transform.position) / SpeedVary_d < 0.02f)
                {
                    _RunAway = false;
                }

            }
        }

    }
    float Noise_f=0.025f;
    Vector3 NoiseOffSet;
    IEnumerator Noise()
    {
        while (true)
        {
            NoiseOffSet = Random.onUnitSphere;
            NoiseOffSet.y = 0.1f;

            yield return new WaitForSeconds(Noise_f);
        }
    }

    float RawLength(Vector3 B, Vector3 A)
    {
        float Lx = Mathf.Abs(B.x - A.x);
        float Ly = Mathf.Abs(B.y - A.y);
        float Lz = Mathf.Abs(B.z - A.z);

        float L = Lx * Lx + Ly * Ly + Lz * Lz;

        return L; 
    }

}
