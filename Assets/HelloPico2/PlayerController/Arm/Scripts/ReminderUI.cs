using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;

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
        public ParticleSystem _DuoArrow;
        public ParticleSystem _Whip;
        public ParticleSystem _Shield;

        [SerializeField] private float _IconFadeInDuration = 0.5f;
        [SerializeField] private float _IconFadeOutDuration = 0.1f;

        private Material upArrowMat;
        private Material downArrowMat;
        private Material duoArrowMat;
        private Material whipMat;
        private Material shieldMat;
        private TweenerCore<float, float, FloatOptions> Sword;
        private TweenerCore<float, float, FloatOptions> Shield;
        private TweenerCore<float, float, FloatOptions> DuoArrow;

        private void Awake()
        {
            if (_UpArrow != null) upArrowMat = _UpArrow.GetComponent<Renderer>().material;
            if (_DownArrow != null) downArrowMat = _DownArrow.GetComponent<Renderer>().material;
            if (_DuoArrow != null) duoArrowMat = _DuoArrow.GetComponent<Renderer>().material;
            whipMat = _Whip.GetComponent<Renderer>().material;
            shieldMat = _Shield.GetComponent<Renderer>().material;

            ChangeOpacity(1, 0, 0);
        }
        private void ChangeOpacity(float from, float to, float duration) => ChangeOpacity(from, to, duration, duration);
        private void ChangeOpacity(float from, float to, float swordDuration, float shieldDuration)
        {
            if(Sword != null) DOTween.Pause(Sword);

            float value1 = 0;
            Sword = DOTween.To(() => value1, x => value1 = x, to, swordDuration).From(from).OnUpdate(() =>
            {
                if (_UpArrow != null) upArrowMat.SetFloat("_Opacity", value1);
                whipMat.SetFloat("_Opacity", value1);
            });

            if (Shield != null) DOTween.Pause(Shield);
            float value2 = 0;

            Shield = DOTween.To(() => value2, x => value2 = x, to, shieldDuration).From(from).OnUpdate(() =>
            {
                if (_DownArrow != null) downArrowMat.SetFloat("_Opacity", value2);
                shieldMat.SetFloat("_Opacity", value2);
            });

            if (_DuoArrow == null) return;

            if(DuoArrow != null) DOTween.Pause(DuoArrow);
            float value3 = 0;
            float duration = (swordDuration > shieldDuration) ? shieldDuration : swordDuration;
            DuoArrow = DOTween.To(() => value3, x => value3 = x, to, duration).From(from).OnUpdate(() =>
            {
                duoArrowMat.SetFloat("_Opacity", value3);
            });
        }
        public void ShowReminder() {
            if (!gameObject.activeSelf) return;
            if (_ReminderState == State.ShowUI) return;
            _ReminderState = State.ShowUI;
            _ShowReminder.OnEnable();
            print("Show Reminder");
            ChangeOpacity(0, 1, .1f);
        }
        public void HideReminder()
        {
            if (!gameObject.activeSelf) return;
            if (_ReminderState == State.HideUI) return;
            _ReminderState = State.HideUI;
            _HideReminder.OnEnable();
            print("Hide Reminder");
            ChangeOpacity(1, 0, .3f);
        }
        public void UseSword() {
            if (!gameObject.activeSelf) return;
            if (_ReminderState == State.HideUI) return;
            _ReminderState= State.HideUI;
            _UseSword.OnEnable();
            print("UseSword");
            ChangeOpacity(1, 0, _IconFadeInDuration, _IconFadeOutDuration);
        }
        public void UseShield()
        {
            if (!gameObject.activeSelf) return;
            if (_ReminderState == State.HideUI) return;
            _ReminderState = State.HideUI;
            _UseShield.OnEnable();
            print("UseShield");
            ChangeOpacity(1, 0, _IconFadeOutDuration, _IconFadeInDuration);
        }
    }
}
