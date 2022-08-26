using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;
using Project;
using DG.Tweening;
using Random = UnityEngine.Random;

namespace HelloPico2.InteractableObjects
{
    public class HitTargetBase : MonoBehaviour, IInteractCollide{

        [FoldoutGroup("PushBack Feedback")][SerializeField] protected bool _UsephPushBackFeedback = false;
        [FoldoutGroup("PushBack Feedback")][ShowIf("_UsephPushBackFeedback")][SerializeField] protected float _PushBackDuration = .3f;
        [FoldoutGroup("PushBack Feedback")][ShowIf("_UsephPushBackFeedback")][SerializeField] protected float _PushBackDist = 3f;
        [FoldoutGroup("PushBack Feedback")][ShowIf("_UsephPushBackFeedback")][SerializeField] protected AnimationCurve _PushBackEasingCureve;

        [FoldoutGroup("Audio Settings")][SerializeField] private string[] _CollideClipName;
        [SerializeField] private int _DamageAmount = 0;
        [SerializeField] private float _HitCDDuration = .05f;
        [SerializeField] private InteractType _InteractType = InteractType.Energy;
        [SerializeField] public bool _ShieldReflectableItem = true;

        public int damageAmount { get { return _DamageAmount; } }
        public InteractType interactType { get { return _InteractType; } }

        public delegate void InteractDel(Collider selfCollider);
        public InteractDel OnBeamInteract;
        public InteractDel OnWhipInteract;
        public InteractDel OnEnergyBallInteract;
        public InteractDel OnShieldInteract;
        public InteractDel OnEnergyInteract;

        public UnityEvent WhenCollide;
        public UltEvents.UltEvent WhenCollideUlt;
        protected Game.Project.ColdDownTimer _timer;
        protected float hitCDDuration { get { return _HitCDDuration; } }

        protected virtual void Start()
        {
            _timer = new Game.Project.ColdDownTimer(_HitCDDuration);
        }
        public void SetUpMoveBehavior(Vector3 dir, float speed, bool useGravity, float gravity) {
            var mover = gameObject.AddComponent<HelloPico2.LevelTool.MoveLevelObject>();
            mover.speed = speed;
            mover.dir = dir;
            mover.useExternalForce = useGravity;
            mover.force = gravity;
        }
        public virtual void OnCollide(InteractType type, Collider selfCollider){
            if (!_timer.CanInvoke()) return;

            CheckInteractType(type, selfCollider);
            NotifyTracker(type);
            _timer.Reset();
        }

        public Action<InteractType, Collider> ColliderEvent{ get; }

        private void CheckInteractType(InteractType type, Collider selfCollider) {            
            switch (type) {
                case InteractType.Beam:
                    OnBeamInteract?.Invoke(selfCollider);
                    break;
                case InteractType.Whip:
                    OnWhipInteract?.Invoke(selfCollider);
                    break;
                case InteractType.EnergyBall:
                    OnEnergyBallInteract?.Invoke(selfCollider);
                    break;
                case InteractType.Shield:
                    OnShieldInteract?.Invoke(selfCollider);
                    break;
                case InteractType.Energy:
                    OnEnergyInteract?.Invoke(selfCollider);
                    break;
                default:
                    WhenCollide?.Invoke();
                    WhenCollideUlt?.Invoke();
                    break;
            }
            if (type == _InteractType) {
                WhenCollide?.Invoke();
                WhenCollideUlt?.Invoke();
            }
        }
        private void NotifyTracker(InteractType type) {
            if (TryGetComponent<HelloPico2.LevelTool.ITrackInteractableState>(out var tracker))
                tracker.WhenCollideWith(type);
        }

        protected void PlayRandomAudio()
        {
            var value = Random.Range(0, _CollideClipName.Length);

            EventBus.Post(new AudioEventRequested(_CollideClipName[value], transform.position));
        }

        protected virtual void PushBackFeedback(Collider hitCol) {
            if (!_UsephPushBackFeedback) return;
        }
        private void OnDrawGizmosSelected()
        {
            #if UNITY_EDITOR
            if (_UsephPushBackFeedback)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawRay(transform.position, transform.forward * _PushBackDist);
                Handles.Label(transform.position + transform.forward * _PushBackDist / 2, "Pushback Distance");
            }
            #endif
        }
    }
}
