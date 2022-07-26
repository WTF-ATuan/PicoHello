using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimeScaleVisualizer : MonoBehaviour, ITimeScaleChange
{
    private TextMeshProUGUI m_Text;
    public float m_ShowTextDelayTime = 1f;

    Coroutine Process { get; set; }
    private void Awake()
    {
        m_Text = GetComponent<TextMeshProUGUI>();
    }
    private void OnEnable()
    {
        TimeManager.Instance?.AddObserver(this);
    }
    private void OnDisable()
    {
        TimeManager.Instance?.RemoveObserver(this);
    }
    public void NotifyTimeScaleChange()
    {
        if (Process == null)
            Process = StartCoroutine(ShowTextDelayer());
        else {
            StopCoroutine(Process);
            Process = StartCoroutine(ShowTextDelayer());
        }
    }

    private IEnumerator ShowTextDelayer() {
        m_Text.enabled = true;
        m_Text.text = "Time Scale: " + Mathf.Clamp(Time.timeScale,0,100).ToString("F1");
        yield return new WaitForSecondsRealtime(m_ShowTextDelayTime);
        m_Text.enabled = false;
    }
}
