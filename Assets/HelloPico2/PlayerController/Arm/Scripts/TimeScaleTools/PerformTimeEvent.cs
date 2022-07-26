using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerformTimeEvent : MonoBehaviour
{
    public bool m_ActiveOnEnable = true;
    public string m_TimeEventName;

    public void OnEnable()
    {
        if(m_ActiveOnEnable)
            DoTimeEvent();
    }

    public void DoTimeEvent() {
        TimeManager.Instance.StartTimeEvent(m_TimeEventName);
    }
    public void DoTimeEvent(string eventName)
    {
        TimeManager.Instance.StartTimeEvent(eventName);
    }
}
