using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UltEvents;

public class PositionChangeEvent : MonoBehaviour
{
    public float m_Initialdelay = 1f;
    public float m_CheckFrequency = 1f;
    public float m_DistanceBuffer = 1f;
    public float m_TurnOffDelayDuration = 3f;
    public UltEvent WhenPosChanged;
    public UltEvent WhenPosNotChange;

    public bool Checked;// { get; set; }
    public Vector3 CurrentPos { get; set; }
    Coroutine Process { get; set; }

    private void OnEnable()
    {
        WhenPosNotChange?.Invoke();
        Checked = true;
        Process = StartCoroutine(Delayer());
    }

    private void Update()
    {
        if (!Checked)
        {
            if (CurrentPos != transform.position)
            {
                if(Vector3.Distance(CurrentPos, transform.position) > m_DistanceBuffer)
                    WhenPosChanged?.Invoke();
                else
                    WhenPosNotChange?.Invoke();

                CurrentPos = transform.position;
            }
            else
            {
                WhenPosNotChange?.Invoke();
            }
            Checked = true;
        }
        else {
            if (Process == null)
                Process = StartCoroutine(Delayer());            
        }
    }

    private IEnumerator Delayer() {
        yield return new WaitForSeconds(1 / m_CheckFrequency + m_TurnOffDelayDuration);
        Checked = false;
        Process = null;
    }
    
}
