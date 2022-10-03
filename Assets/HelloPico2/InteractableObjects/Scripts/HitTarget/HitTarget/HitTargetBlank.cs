using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using Project;
using DG.Tweening;
using Random = UnityEngine.Random;

namespace HelloPico2.InteractableObjects
{
    public class HitTargetBlank : MonoBehaviour, IInteractCollide
    {
        [FoldoutGroup("Audio Settings")][SerializeField] private string[] _CollideClipName;
        [SerializeField] private float _HitCDDuration = .05f;
        [SerializeField] private bool _AffectByAllInteractType = true;
        [HideIf("_AffectByAllInteractType")][SerializeField] private InteractType _InteractType = InteractType.Energy;

        public InteractType interactType { get { return _InteractType; } }

        public UltEvents.UltEvent WhenCollideUlt;
        protected Game.Project.ColdDownTimer _timer;
        protected float hitCDDuration { get { return _HitCDDuration; } }

        protected virtual void Start()
        {
            _timer = new Game.Project.ColdDownTimer(_HitCDDuration);
        }
        public void SetUpMoveBehavior(Vector3 dir, float speed, bool useGravity, float gravity)
        {
            var mover = gameObject.AddComponent<HelloPico2.LevelTool.MoveLevelObject>();
            mover.speed = speed;
            mover.dir = dir;
            mover.useExternalForce = useGravity;
            mover.force = gravity;
        }
        public virtual void OnCollide(InteractType type, Collider selfCollider)
        {
            _timer ??= new Game.Project.ColdDownTimer(_HitCDDuration);
            if (!_timer.CanInvoke()) return;

            CheckInteractType(type, selfCollider);
            _timer.Reset();
        }

        public Action<InteractType, Collider> ColliderEvent { get; }

        private void CheckInteractType(InteractType type, Collider selfCollider)
        {
            if (_AffectByAllInteractType)
                DoInteraction();
            else if (type == _InteractType)
                DoInteraction();
        }
        protected void DoInteraction() {
            WhenCollideUlt?.Invoke();
            PlayRandomAudio();
        }
        protected void PlayRandomAudio()
        {
            var value = Random.Range(0, _CollideClipName.Length);

            EventBus.Post(new AudioEventRequested(_CollideClipName[value], transform.position));
        }
        protected void PlayAudio(int[] indexs)
        {
            var value = Random.Range(0, indexs.Length);

            EventBus.Post(new AudioEventRequested(_CollideClipName[indexs[value]], transform.position));
        }
    }
}
