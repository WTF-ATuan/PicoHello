using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LC : MonoBehaviour
{
   public int i = 0;

    EveryLevel Level = null;

    public void OnEnable()
    {
        switch (i)
        {
            case 0: Level = new Level_0(); break;
            case 1: Level = new Level_1(); break;
            case 2: Level = new Level_2(); break;
        }
        Level.Start();
    }

    public void Update()
    {
        switch (i)
        {
            case 0: Level = new Level_0(); break;
            case 1: Level = new Level_1(); break;
            case 2: Level = new Level_2(); break;
        }
        Level.Update();
    }
}
