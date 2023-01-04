using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomActiveObj : MonoBehaviour
{
    [SerializeField]
    private GameObject[] objList;
    [SerializeField]
    private float closeTime;
    int getListNum;
    // Start is called before the first frame update
    private void OnEnable()
    {
        getListNum = Random.Range(0, objList.Length);
        objList[getListNum].SetActive(true);
        StartCoroutine(CloseObject());
    }
    IEnumerator CloseObject()
    {
        yield return new WaitForSeconds(closeTime);
        objList[getListNum].SetActive(false);
        getListNum = 0;
    }
}
