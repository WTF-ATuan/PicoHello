using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UltEvents;

public class DisableEvent : MonoBehaviour
{
    public UltEvent WhenDisable;
    public void OnDisable()
    {
        WhenDisable.Invoke();
    }
}
