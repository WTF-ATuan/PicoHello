using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSizeXYZ : MonoBehaviour
{
    public Vector3 RandomSize_Adjust_min;
    public Vector3 RandomSize_Adjust_max;

    void OnEnable()
    {
        transform.localScale +=
            new Vector3(
                Random.Range(RandomSize_Adjust_min.x, RandomSize_Adjust_max.x),
                Random.Range(RandomSize_Adjust_min.y, RandomSize_Adjust_max.y),
                Random.Range(RandomSize_Adjust_min.z, RandomSize_Adjust_max.z)
            );

        transform.localScale =
            new Vector3(
                transform.localScale.x * (1 - Random.Range(0, 2) * 2),
                transform.localScale.y * (1 - Random.Range(0, 2) * 2),
                transform.localScale.z * (1 - Random.Range(0, 2) * 2)
                );

    }
}
