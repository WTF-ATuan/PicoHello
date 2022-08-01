using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectCheckChild_Jy : MonoBehaviour
{   // Update is called once per frame
    void Update()
    {
        if(gameObject.transform.childCount == 0)
        {
            Destroy(gameObject);
        }
    }
}
