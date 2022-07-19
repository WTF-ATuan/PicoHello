using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

namespace HelloPico2.AnimationTool
{
    public class EventCatcher : MonoBehaviour
    {
        public List<EventCall> OnEventCall = new List<EventCall>();

        public void AnimationEvent(AnimationEvent _animationEvent)
        {
            foreach (EventCall action in OnEventCall)
            {
                if (action.callName == _animationEvent.stringParameter)
                {
                    action.eventCall.Invoke();
                }
            }
        }
    }

    [System.Serializable]
    public class EventCall
    {
        public string callName;
        [System.Serializable]
        public class OnEventCall : UnityEvent { }
        public OnEventCall eventCall = new OnEventCall();
    }
}