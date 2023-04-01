using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object_Kind : MonoBehaviour
{


    public enum Kind { Scissors, Papper, Stone };

    public Kind _Kind;

    public enum Opposition { Enemy, Player };

    public Opposition _Opposition;


    [HideInInspector]
    public int _k;
    [HideInInspector]
    public int _o;


    private void Awake()
    {

        switch(_Opposition)
        {
            case Opposition.Player: _o = 0; break;
            case Opposition.Enemy: _o = 1; break;
        }

        switch (_Kind)
        {
            case Kind.Scissors: _k = 0; break;
            case Kind.Papper: _k = 1; break;
            case Kind.Stone: _k = 2; break;
        }

    }

    void Update()
    {
        
    }
}
