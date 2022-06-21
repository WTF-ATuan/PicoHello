using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HelloPico2.PlayerController
{
    public class TriggerBase : MonoBehaviour
    {
        public UnityEvent WhenTriggerEnter;

        public UnityEvent WhenTriggerStay;

        public UnityEvent WhenTriggerExit;

        public string m_Tag = "";
        public LayerMask m_TargetLayer;

        public UnityEvent WhenTriggerEnterDelay;

        public UnityEvent WhenTriggerExitDelay;

        public delegate void TriggerDelegate(Collider target);
        public TriggerDelegate TriggerEnter;
        public TriggerDelegate TriggerExit;

        public float m_DelayTime = 1f;
        Coroutine Process { get; set; }

        public virtual void OnTriggerEnter(Collider other)
        {            
            if (other.tag == m_Tag || 1 << other.gameObject.layer == m_TargetLayer)
            {
                //print(other.name);
                WhenTriggerEnter.Invoke();
                TriggerEnter?.Invoke(other);

                if (Process == null)
                {
                    Process = StartCoroutine(DelayTrigger(WhenTriggerEnterDelay, m_DelayTime));
                }
                else
                {
                    StopCoroutine(Process);
                    Process = StartCoroutine(DelayTrigger(WhenTriggerEnterDelay, m_DelayTime));
                }
            }
        }
        public virtual void OnTriggerStay(Collider other)
        {
            if (other.tag == m_Tag || 1 << other.gameObject.layer == m_TargetLayer)
            {
                WhenTriggerStay.Invoke();
            }
        }
        public virtual void OnTriggerExit(Collider other)
        {
            if (other.tag == m_Tag || 1 << other.gameObject.layer == m_TargetLayer)
            {
                WhenTriggerExit.Invoke();
                TriggerExit?.Invoke(other);

                if (Process == null)
                {
                    Process = StartCoroutine(DelayTrigger(WhenTriggerExitDelay, m_DelayTime));
                }
                else
                {
                    StopCoroutine(Process);
                    Process = StartCoroutine(DelayTrigger(WhenTriggerExitDelay, m_DelayTime));
                }
            }
        }
        public IEnumerator DelayTrigger(UnityEvent target, float delayTime)
        {
            yield return new WaitForSeconds(delayTime);
            target.Invoke();
        }
    }
}
