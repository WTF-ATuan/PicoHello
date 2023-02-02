using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;

namespace HelloPico2.LevelTool
{
    public class ReminderUI : MonoBehaviour
    {
        public enum State { ShowUI, HideUI}
        [ReadOnly][SerializeField] private State _ReminderState = State.HideUI;
        public EnableEvent _ShowReminder;
        public EnableEvent _HideReminder;
        public EnableEvent _UseSword;
        public EnableEvent _UseShield;
                
        public ParticleSystem _UpArrow;
        public ParticleSystem _DownArrow;
        public ParticleSystem _Whip;
        public ParticleSystem _Shield;

        private Material upArrowMat;
        private Material downArrowMat;
        private Material whipMat;
        private Material shieldMat;

        Sequence swordSeq;
        Sequence shieldSeq;

        private void Awake()
        {
            upArrowMat = _UpArrow.GetComponent<Renderer>().material;
            downArrowMat = _DownArrow.GetComponent<Renderer>().material;
            whipMat = _Whip.GetComponent<Renderer>().material;
            shieldMat = _Shield.GetComponent<Renderer>().material;

            swordSeq = DOTween.Sequence();
            shieldSeq = DOTween.Sequence();
            ChangeOpacity(1, 0, 0);
        }
        private void ChangeOpacity(float from, float to, float duration) => ChangeOpacity(from, to, duration, duration);
        private void ChangeOpacity(float from, float to, float swordDuration, float shieldDuration)
        {
            if (swordSeq.IsPlaying()) swordSeq.Kill();

            float value1 = 0;
            swordSeq.Append(
                DOTween.To(() => value1, x => value1 = x, to, swordDuration).From(from).OnUpdate(() => {
                upArrowMat.SetFloat("_Opacity", value1);
                whipMat.SetFloat("_Opacity", value1);
                })
            );
            swordSeq.Play();

            if (shieldSeq.IsPlaying()) swordSeq.Kill();

            float value2 = 0;
            shieldSeq.Append(
                DOTween.To(() => value2, x => value2 = x, to, shieldDuration).From(from).OnUpdate(() => {
                    downArrowMat.SetFloat("_Opacity", value2);
                    shieldMat.SetFloat("_Opacity", value2);
                })
            );
            shieldSeq.Play();
        }
        public void ShowReminder() {
            if (_ReminderState == State.ShowUI) return;
            _ReminderState = State.ShowUI;
            _ShowReminder.OnEnable();

            ChangeOpacity(0, 1, .1f);
        }
        public void HideReminder()
        {
            if (_ReminderState == State.HideUI) return;
            _ReminderState = State.HideUI;
            _HideReminder.OnEnable();

            ChangeOpacity(1, 0, .3f);
        }
        public void UseSword() { 
            if(_ReminderState == State.HideUI) return;
            _ReminderState= State.HideUI;
            _UseSword.OnEnable();

            ChangeOpacity(1, 0, .5f, .1f);
        }
        public void UseShield()
        {
            if (_ReminderState == State.HideUI) return;
            _ReminderState = State.HideUI;
            _UseShield.OnEnable();

            ChangeOpacity(1, 0, .1f, .5f);
        }
    }
}
