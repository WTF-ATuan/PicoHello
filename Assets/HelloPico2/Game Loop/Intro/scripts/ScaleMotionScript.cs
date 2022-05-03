using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScaleMotionScript : MonoBehaviour
{
    public int sceneNum;
    public bool isScale;
    public float speed = 0.2f;
    public float speed_s = 0.01f;
    public GameObject[] checkList;
    public GameObject path;
    // Start is called before the first frame update
    // Update is called once per frame
    void Start()
    {
        Debug.Log(checkList[0].name);
        Debug.Log(checkList[1].name);
    }

    void Update()
    {
        
        if (isScale)
        {
            transform.localScale -= new Vector3(speed_s, speed_s, speed_s);
        }
            
        if (transform.localScale.x < 0.01f)
        {
            Destroy(gameObject);
        }
        if (checkList[0] == null && checkList[1] == null)
        {
            transform.position = Vector3.Lerp(transform.position, path.transform.position, speed);
        }
        
    }

}
