using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using System.Linq;

public class NewBehaviourScript : MonoBehaviour
{

    void OnEnable()
    {
        StartCoroutine(ABC());
    }


    void Update()
    {
    }



    IEnumerator ABC()
    {

        yield return new WaitForSeconds(0.75f);  //����2��
        transform.position = new Vector3(Camera.main.transform.position.x, transform.position.y, Camera.main.transform.position.z);

    }
}
