using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.InputSystem;
    
public class TimeManager : MonoBehaviour
{
    public float m_SpeedControlMax = 3;
    public enum TimeEventType {
        SkipFrame, SlowMotion
    }
    public enum TimerMode { 
        Duration, OnOff
    }
    [System.Serializable]
    public struct TimeEvent {
        public string Name;
        public TimeEventType EventType;
        [ShowIf("EventType", TimeEventType.SkipFrame)]
        public int Frames;
        [ShowIf("EventType", TimeEventType.SlowMotion)]
        public float SlowDownSpeed;
        [ShowIf("EventType", TimeEventType.SlowMotion)]
        public float TransitionTime;
        [ShowIf("EventType", TimeEventType.SlowMotion)]
        public TimerMode TimerMode;
        [ShowIf("TimerMode", TimerMode.OnOff)]
        public bool OnOff;
        [ShowIf("TimerMode", TimerMode.Duration)]
        public float Duration;
    }
    public List<TimeEvent> m_TimeEvents = new List<TimeEvent>();
    List<ITimeScaleChange> TimeScaleChangeObservers = new List<ITimeScaleChange>();
    List<ITimelinePlaying> ITimelinePlayingObservers = new List<ITimelinePlaying>();
    public PlayerInput playerInput;

    public static TimeManager m_Instance;
    public static TimeManager Instance {
        get {
            if (m_Instance == null)
                return GameObject.FindObjectOfType<TimeManager>();
            else
                return m_Instance;
        }
    }

    Coroutine Process { get; set; }

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();        
    }
    private void OnEnable()
    {
        playerInput.actions["+"].performed += (InputAction.CallbackContext callback) => { SpeedUp(); };
        playerInput.actions["-"].performed += (InputAction.CallbackContext callback) => { SpeedDown(); };
        playerInput.actions["0"].performed += (InputAction.CallbackContext callback) => { SetTo(0); };
        playerInput.actions["1"].performed += (InputAction.CallbackContext callback) => { SetTo(1); };
        playerInput.actions["2"].performed += (InputAction.CallbackContext callback) => { SetTo(2); };
        playerInput.actions["3"].performed += (InputAction.CallbackContext callback) => { SetTo(3); };
    }
    private void OnDisable()
    {
        playerInput.actions["+"].performed -= (InputAction.CallbackContext callback) => { SpeedUp(); };
        playerInput.actions["-"].performed -= (InputAction.CallbackContext callback) => { SpeedDown(); };
        playerInput.actions["0"].performed -= (InputAction.CallbackContext callback) => { SetTo(0); };
        playerInput.actions["1"].performed -= (InputAction.CallbackContext callback) => { SetTo(1); };
        playerInput.actions["2"].performed -= (InputAction.CallbackContext callback) => { SetTo(2); };
        playerInput.actions["3"].performed -= (InputAction.CallbackContext callback) => { SetTo(3); };
    }
    public void StartTimeEvent(string name) {
        var check = false;
        for (int i = 0; i < m_TimeEvents.Count; i++)
        {
            if (m_TimeEvents[i].Name == name) {
                CheckEventType(i);
                check = true;
            }
        }

        if (!check) {
            Debug.Log("Couldn't found the " + name + " time event");
        }
    }
    public void SpeedUp() {
        if (Time.timeScale < m_SpeedControlMax)
        {
            Time.timeScale += 0.2f;
            NotifyObservers();
        }
    }
    public void SpeedDown()
    {
        if (Time.timeScale > 0.1f)
        {
            Time.timeScale -= 0.2f;
        }
        else
        {
            Time.timeScale = 0f;
        }
        NotifyObservers();
    }    
    public void Pause() {
        Time.timeScale = 0f;
        NotifyObservers();
    }

    public void SetByPercent(float percent){
        var timeScale = Mathf.Lerp(1 , m_SpeedControlMax , percent);
        Time.timeScale = timeScale;
        NotifyObservers();
    }

    public void SetTo(float value) {
        Time.timeScale = value;
        NotifyObservers();
    }    
    private void CheckEventType(int index) {
        switch (m_TimeEvents[index].EventType)
        {
            case TimeEventType.SkipFrame:
                if (Process == null) {
                    Process = StartCoroutine(SkipFrames(m_TimeEvents[index].Frames));
                }
                else
                {
                    StopCoroutine(Process);
                    Process = StartCoroutine(SkipFrames(m_TimeEvents[index].Frames));
                }
                break;
            case TimeEventType.SlowMotion:
                if (Process == null)
                {
                    switch (m_TimeEvents[index].TimerMode)
                    {
                        case TimerMode.Duration:
                            Process = StartCoroutine(SlowMotion(m_TimeEvents[index].SlowDownSpeed, m_TimeEvents[index].TransitionTime, m_TimeEvents[index].Duration));
                            break;
                        case TimerMode.OnOff:
                            Process = StartCoroutine(SlowMotionOnOff(m_TimeEvents[index].SlowDownSpeed, m_TimeEvents[index].TransitionTime, m_TimeEvents[index].OnOff));
                            break;
                        default:
                            break;
                    }                    
                }
                else
                {
                    switch (m_TimeEvents[index].TimerMode)
                    {
                        case TimerMode.Duration:
                            StopCoroutine(Process);
                            Process = StartCoroutine(SlowMotion(m_TimeEvents[index].SlowDownSpeed, m_TimeEvents[index].TransitionTime, m_TimeEvents[index].Duration));
                            break;
                        case TimerMode.OnOff:
                            StopCoroutine(Process);
                            Process = StartCoroutine(SlowMotionOnOff(m_TimeEvents[index].SlowDownSpeed, m_TimeEvents[index].TransitionTime, m_TimeEvents[index].OnOff));
                            break;
                        default:
                            break;
                    }                    
                }
                break;
            default:
                break;
        }
    }

    public IEnumerator SkipFrames(int frames) {
        int counter = 0;

        Time.timeScale = 0;

        while (counter < frames) {
            ++counter; 
            yield return new WaitForEndOfFrame();
        }

        Time.timeScale = 1;
    }

    public IEnumerator SlowMotion(float Speed, float transitionTime, float Duration) {
        var startTime = Time.realtimeSinceStartup;
        var endTime = transitionTime;

        while (Time.realtimeSinceStartup - startTime < endTime)
        {
            Time.timeScale = Mathf.Lerp(1, Speed, (Time.unscaledTime - startTime) / endTime);
            yield return null;
        }

        Time.timeScale = Speed;
        
        yield return new WaitForSecondsRealtime(Duration);

        startTime = Time.realtimeSinceStartup;

        while (Time.realtimeSinceStartup - startTime < endTime)
        {
            Time.timeScale = Mathf.Lerp(Speed, 1, (Time.unscaledTime - startTime) / endTime);
            yield return null;
        }

        Time.timeScale = 1;
    }
    public IEnumerator SlowMotionOnOff(float Speed, float transitionTime, bool onOff)
    {
        var startTime = Time.realtimeSinceStartup;
        var endTime = transitionTime;
        var from = Time.timeScale;
        var to = onOff ? Speed : 1;

        while (Time.realtimeSinceStartup - startTime < endTime)
        {
            Time.timeScale = Mathf.Lerp(from, to, (Time.unscaledTime - startTime) / endTime);
            yield return null;
        }

        Time.timeScale = to;        
    }
    #region TimeScaleObserver
    public void AddObserver(ITimeScaleChange target)
    {
        TimeScaleChangeObservers.Add(target);
    }
    public void RemoveObserver(ITimeScaleChange target)
    {
        TimeScaleChangeObservers.Remove(target);
    }
    private void NotifyObservers() {
        foreach (var item in TimeScaleChangeObservers)
        {
            item.NotifyTimeScaleChange();
        }
    }
    #endregion

    #region TimelinePlayingObserver
    public void AddObserver(ITimelinePlaying target)
    {
        ITimelinePlayingObservers.Add(target);
    }
    public void RemoveObserver(ITimelinePlaying target)
    {
        ITimelinePlayingObservers.Remove(target);
    }
    public void NotifyTimelineObservers(bool value)
    {
        foreach (var item in ITimelinePlayingObservers)
        {
            item.NotifyTimelinePlaying(value);
        }
    }
    #endregion
}
