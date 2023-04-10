using System;
using UnityEngine;

public class BoardCheck : MonoBehaviour
{
    //CN Board
    public GameObject CNBoard;
    //Other Board
    public GameObject Board;

    private void Start()
    {
        //Check area
        if (Application.systemLanguage == SystemLanguage.ChineseSimplified)
        {
            CNBoard.SetActive(true);
        }
        else
        {
            Board.SetActive(true);
        }
    }
}
