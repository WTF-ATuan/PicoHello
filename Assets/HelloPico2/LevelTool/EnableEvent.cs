using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UltEvents;

public class EnableEvent : MonoBehaviour
{
    public UltEvent WhenEnable;
    public UltEvent WhenEnableDelay;
    public float m_DelayTime = 1;
    public void OnEnable()
    {
        WhenEnable.Invoke();
        StartCoroutine(Delayer());
    }
    private IEnumerator Delayer() {
        yield return new WaitForSeconds(m_DelayTime);
        WhenEnableDelay.Invoke();
    }
}
