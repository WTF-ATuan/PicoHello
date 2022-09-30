using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EveryLevel 
{

    public abstract void Start();
    public abstract void Update();

}

public  class Level_0 : EveryLevel
{
    public override void Start()
    {
        Debug.Log("Level_0 Start");
    }

    public override void Update()
    {
        Debug.Log("Level_0 Update...");
    }
}

public class Level_1 : EveryLevel
{
    public override void Start()
    {
        Debug.Log("Level_1 Start");
    }

    public override void Update()
    {
        Debug.Log("Level_1 Update...");
    }
}


public class Level_2 : EveryLevel
{
    public override void Start()
    {
        Debug.Log("Level_2 Start");
    }

    public override void Update()
    {
        Debug.Log("Level_2 Update...");
    }
}