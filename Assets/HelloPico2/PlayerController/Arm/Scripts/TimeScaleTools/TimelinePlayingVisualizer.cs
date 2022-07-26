using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimelinePlayingVisualizer : MonoBehaviour, ITimelinePlaying
{
    public float m_ShowTextDelayTime = 1f;
    private Image m_PauseImage;

    Coroutine Process { get; set; }
    private void Awake()
    {
        m_PauseImage = GetComponent<Image>();
    }
    private void OnEnable()
    {        
        TimeManager.Instance?.AddObserver(this);
    }
    private void OnDisable()
    {
        TimeManager.Instance?.RemoveObserver(this);
    }   

    private IEnumerator ShowImage(bool isPlaying)
    {
        m_PauseImage.enabled = !isPlaying;
        yield return null;
    }

    public void NotifyTimelinePlaying(bool isPlaying)
    {
        if (Process == null)
            Process = StartCoroutine(ShowImage(isPlaying));
        else
        {
            StopCoroutine(Process);
            Process = StartCoroutine(ShowImage(isPlaying));
        }
    }
}
