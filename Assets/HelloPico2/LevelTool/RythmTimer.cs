using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HelloPico2.LevelTool
{
    public class RythmTimer : MonoBehaviour
    {
        public float _ActivateTime = .6f;
        public float _CriticalDuration = .6f;
        public UltEvents.UltEvent WhenNormalHit;
        public UltEvents.UltEvent WhenCriticalHit;

        bool _CriticalHit = false;

        private void OnEnable()
        {
            StartCoroutine(Timer());
        }
        private IEnumerator Timer() {
            _CriticalHit = false;
            yield return new WaitForSeconds(_ActivateTime);
            _CriticalHit = true;
            yield return new WaitForSeconds(_CriticalDuration);
            _CriticalHit = false;
        }
        public void NotifyTimingEvent() {
            if (_CriticalHit)
                WhenCriticalHit?.Invoke();
            else
                WhenNormalHit?.Invoke();    
        }
    }
}
