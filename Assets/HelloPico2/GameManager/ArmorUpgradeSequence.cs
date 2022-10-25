using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using HelloPico2.InteractableObjects;

namespace HelloPico2.Singleton
{
    public class ArmorUpgradeSequence : MonoBehaviour
    {
        [SerializeField] private float _Speed; 
        [SerializeField] private float _OrbitDuration; 
        [SerializeField] private float _ArmorUpgradeScaleOnApporach;
        [SerializeField] private Vector3 _PlayerPosOffset = new Vector3(0, 1, .8f);
        [SerializeField] private AnimationCurve _XCurve;
        [SerializeField] private AnimationCurve _YCurve;
        [SerializeField] private AnimationCurve _ZCurve;

        [Header("Show Player Item Sequence")]
        public Vector3 _RotatePunch;
        public Vector3 _ScalePunch;
        public int _Vibrato = 10;
        public float _Duration = 1;
        public float _AttractorVFXDuration = 3;

        [Header("Additional Settings")]
        public bool _SkipRotatingSpirit = false;

        [Header("Audio Settings")]
        [SerializeField] private string _ArmorPopClipName = "ArmorPop";

        public ParticleSystem[] _ArmorParticles;
        public HelloPico2.LevelTool.SkinnedMeshEffectPlacement[] _ParticlesTarget;
        public Transform[] _ArmorPosition;
        bool leftRight = false;

        public delegate void NotifyMember();
        public NotifyMember WhenStartArmorUpgradeSequence;

        public static ArmorUpgradeSequence _Instance;
        public static ArmorUpgradeSequence Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = GameObject.FindObjectOfType<ArmorUpgradeSequence>();

                return _Instance;
            }
        }        
        private void OnValidate()
        {
            if (_ArmorParticles.Length > 2)
                throw new System.Exception("_ArmorParticles can only have two target");
            if (_ParticlesTarget.Length > 2)
                throw new System.Exception("_ParticlesTarget can only have two target");
        }
        public void UpdatePlayerArmorPosition(GameObject armor)
        {
            if(!leftRight)
                _ArmorPosition[0] = armor.transform;
            else
                _ArmorPosition[1] = armor.transform;

            leftRight = !leftRight;
        }
        public void StartArmorUpgradeSequence(Transform armorUpgrade, ArmorTransformSequence armorTransformControl, TweenCallback gainArmorCallback)
        {
            WhenStartArmorUpgradeSequence?.Invoke();

            var spiritTarget = HelloPico2.Singleton.GameManagerHelloPico.Instance.Spirit._GainArmorUpgradeRotationPivot;
            var playertarget = HelloPico2.Singleton.GameManagerHelloPico.Instance._Player;
            var orbitDuration = _OrbitDuration;
            var duration = GetDuration(armorUpgrade.position, spiritTarget.position);

            armorUpgrade.transform.localScale = Vector3.one;

            Sequence seq = DOTween.Sequence();

            if (!_SkipRotatingSpirit)
            {
                TweenCallback LerpToSpirit = () =>
                {
                    armorUpgrade.DOMoveX(spiritTarget.position.x, duration);
                    armorUpgrade.DOMoveY(spiritTarget.position.y, duration);
                    armorUpgrade.DOMoveZ(spiritTarget.position.z, duration);
                    armorUpgrade.DOScale(_ArmorUpgradeScaleOnApporach, duration);
                };
                seq.AppendCallback(LerpToSpirit);

                TweenCallback EnterOrbitMode = () =>
                {
                    armorUpgrade.SetParent(spiritTarget);
                    armorUpgrade.DOLocalMove(Vector3.zero, GetDuration(armorUpgrade.position, spiritTarget.position)).OnComplete(() =>
                    {
                        HelloPico2.Singleton.GameManagerHelloPico.Instance.Spirit.OnReceiveArmorUpgrade();
                    });
                };
                seq.AppendCallback(EnterOrbitMode);

                seq.AppendInterval(duration);

                seq.AppendInterval(orbitDuration);

                TweenCallback ExitOrbitMode = () =>
                {
                    armorUpgrade.SetParent(spiritTarget.root.parent);
                };
                seq.AppendCallback(ExitOrbitMode);
            }
            else {
                armorUpgrade.DOScale(_ArmorUpgradeScaleOnApporach, duration);
            }

            var toPlayerDuration = GetDuration(armorUpgrade.position, spiritTarget.position);
            TweenCallback MoveToPlayerFront = () => {
                seq.Append(armorUpgrade.DOMove(playertarget.transform.position + _PlayerPosOffset, toPlayerDuration));
                seq.Append(armorUpgrade.DORotate(new Vector3(0,180,0), toPlayerDuration));                
            };
            seq.AppendCallback(MoveToPlayerFront);
            seq.AppendInterval(toPlayerDuration);

            TweenCallback ShowPlayerItem = () => {
                // Skip poping to save time
                seq.Append(armorUpgrade.DOPunchRotation(_RotatePunch, _Duration, _Vibrato));                
                seq.Append(armorUpgrade.DOPunchScale(_ScalePunch, _Duration, _Vibrato));                
                AudioPlayerHelper.PlayAudio( _ArmorPopClipName, transform.position);
            };
            seq.AppendCallback(ShowPlayerItem);

            seq.AppendInterval(_Duration * .2f);

            TweenCallback Transformation = () => {
                armorTransformControl.StartTransform();
            };
            seq.AppendCallback(Transformation);

            seq.AppendInterval(_Duration * .8f);
            
            seq.Append(armorUpgrade.DOScale(Vector3.zero,.1f).OnComplete(() => { Destroy(armorUpgrade.gameObject); }));;

            seq.AppendCallback(gainArmorCallback);

            TweenCallback SpawnVFX = () => {
                PlayParticle(armorUpgrade);
            };
            seq.AppendCallback(SpawnVFX);

            seq.Play();
        }        
        private float GetDuration(Vector3 from, Vector3 to)
        {
            return Vector3.Distance(from, to) / _Speed;
        }
        private void PlayParticle(Transform VFXPos) {
            for (int i = 0; i < 2; i++)
            {
                //_ParticlesTarget[i].transform.SetParent(_ArmorPosition[i]);
                //_ParticlesTarget[i].transform.localPosition = Vector3.zero;

                _ParticlesTarget[i].SetPosition(_ArmorPosition[i].GetComponent<Renderer>(), _ParticlesTarget[i].transform);

                _ArmorParticles[i].transform.position = VFXPos.position;

                _ArmorParticles[i].Play();
            }
        }
    }
}
