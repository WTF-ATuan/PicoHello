using System;
using System.Collections;
using System.Linq;
using DG.Tweening;
using HelloPico2.InteractableObjects;
using HelloPico2.PlayerController.Arm;
using HelloPico2.Singleton;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HelloPico2.LevelTool
{
    public class FloatingObjectMover : MonoBehaviour
    {
        public enum ItemShowingType { 
            Floating, PopOut
        }
        public CollectableItemTrackingList _CollectableItemsSO;
        public TargetItem_SO menuItemSo;
        public ArmorType _StartingType => GetArmorTypeFromSo();
        public ArmorPart _StartingParts;
        public float _ChangeTypeTimer = 10;
        public float _ChangeTypeFastTimer = 2;
        public float _SpawnScalingRatio = .5f;
        public float _SpawnScalingDuration = .5f;
        public int _vibrato = 5;

        [Header("Floating Behavior Settings")]
        public ItemShowingType _ItemShowingType = ItemShowingType.Floating;
        public Vector2 _DepthRange = new Vector2(10,15);
        public Vector2 _TiltRotationRange = new Vector2(-30,30);
        public Vector2 _VerticalRotationRange = new Vector2(-60,60);
        public float _EndDepthValue = 5;
        public float _DepthDuration = 100;
        public float _TiltDuration = 30;
        public float _HorizontalSpeed = 5;
        public float _VerticalDuration = 20;
        public Ease _TiltEase = Ease.InOutCubic;
        public Ease _VerticalEase = Ease.InOutCubic;
        public Transform _Container;
        public Transform currentFloatingObject;

        [Header("Poping Out Settings")]
        public Transform _PopOutCenter;
        public float _ScaleMultiplier;
        public float _Radius;
        public float _Duration;
        public float _StayDuration;
        public float _ExplosionAngle;
        public float _OffsetTiming;
        public float _OffsetAutoGrabTiming;
        public float _SpeedUpMul;

        [Header("Auto Update Wave")]
        public bool _EnableAutoUpdateWave = true;

        [Header("Auto Grab")]
        public float _AutoGrabDuration = 5;

        [Header("Wave")]
        public float _NextItemDelayDuration = 5;
        //[MinMaxSlider(0f, 360f,true)]
        public Vector2[] _WaveStartHorizonRot;

        [SerializeField] private string _SpawnedAudioClipsName;

        [ReadOnly][SerializeField] private float totalDuration;
        [ReadOnly][SerializeField] private int currentWave;

        public ArmorType currentType { get; set; }
        public ArmorPart currentParts { get; set; }

        Coroutine typeChangerProcess;
        private void OnValidate()
        {
            totalDuration = _DepthDuration + _AutoGrabDuration;
        }
        private void OnEnable()
        {
            _PopOutCenter = GameManagerHelloPico.Instance.Spirit.transform;
            ArmorUpgradeSequence.Instance.WhenStartArmorUpgradeSequence += OnNotifyArmorUpgradeStart;
        }
        private void OnDisable()
        {
            ArmorUpgradeSequence.Instance.WhenStartArmorUpgradeSequence -= OnNotifyArmorUpgradeStart;            
        }
        private void OnNotifyArmorUpgradeStart() {
            if (typeChangerProcess != null)
                StopCoroutine(typeChangerProcess);
        }
        public void StartSendingCollectableItem()
        {
            currentType = _StartingType;
            currentParts = _StartingParts;
            CheckSpawnObject(_StartingType, _StartingParts);
            typeChangerProcess = StartCoroutine(TypeChanger());
        }
        public void SendOutAllArmor() {
            StartCoroutine(SendingArmor());
        }
        private IEnumerator SendingArmor () {
            for (int i = 0; i < 4; i++)
            {
                yield return new WaitForSeconds(_OffsetTiming);
                NextArmorParts();
            }
        }
        public void NextArmorParts()
        {
            if (currentParts == ArmorPart.UpperArm)
                return;
            else
                currentParts++;

            currentType = _StartingType;

            CheckSpawnObject(currentType, currentParts);
            
            if(typeChangerProcess != null)
                StopCoroutine(typeChangerProcess);

            typeChangerProcess = StartCoroutine(TypeChanger());
        }
        [ReadOnly][SerializeField] private float timer;
        [ReadOnly][SerializeField] private float TotalTimer;
        private void Update()
        {
            timer += Time.deltaTime;
            TotalTimer += Time.deltaTime;
            if (currentFloatingObject != null) return;
            
            if(typeChangerProcess != null)
                StopCoroutine(typeChangerProcess);

            if (_EnableAutoUpdateWave && timer > _NextItemDelayDuration)
                NextArmorParts();
        }
        private void CheckSpawnObject(ArmorType type, ArmorPart part) {
            var item = _CollectableItemsSO.GetItem(type, part);
            var clone = CreateObject(item);

            currentFloatingObject = clone.transform;

            if (_ItemShowingType == ItemShowingType.Floating)
                SetUpFloating(clone.transform, currentWave);
            if (_ItemShowingType == ItemShowingType.PopOut)
                SetUpExplosionItem(clone.transform, currentWave);

            AudioPlayerHelper.PlayMultipleAudio(_SpawnedAudioClipsName, currentWave, clone.transform.position);

            UpdateWave();
        }
        private void ChangeObjectType(ArmorType type, ArmorPart part)
        {
            var item = _CollectableItemsSO.GetItem(type, part);
            var clone = CreateObject(item);
            clone.transform.SetParent(currentFloatingObject.parent);
            clone.transform.localPosition = currentFloatingObject.localPosition;        

            Destroy(currentFloatingObject.gameObject);
            currentFloatingObject = clone.transform;
            print("Destroy currentfloatingObject");
        }
        private GameObject CreateObject(InteractableArmorUpgrade item) {
            var clone = Instantiate(item).gameObject;

            clone.transform.DOPunchScale(clone.transform.localScale * _SpawnScalingRatio, _SpawnScalingDuration, _vibrato);

            return clone;
        }
        private void UpdateWave() { 
            if(currentWave < _WaveStartHorizonRot.Length - 1)
                currentWave++;
            else
                currentWave = 0;            
        }
        [Button]
        private void SetUpFloating(Transform obj, int wave) {
            var tiltPivot =  Instantiate(new GameObject(), _Container);
            var horizontalPivot =  Instantiate(new GameObject(), _Container);
            var verticalPivot =  Instantiate(new GameObject(), _Container);
            var depthPivot =  Instantiate(new GameObject(), _Container);

            tiltPivot.name = "TiltPivot";
            horizontalPivot.name = "HorizontalPivot";
            verticalPivot.name = "VerticalPivot";
            depthPivot.name = "DepthPivot";

            var depth = Random.Range(_DepthRange.x, _DepthRange.y);
            //var hAngle = Random.Range(0, 360);
            var hAngle = Random.Range(_WaveStartHorizonRot[currentWave].x, _WaveStartHorizonRot[currentWave].y);
            var vAngle = Random.Range(_VerticalRotationRange.x, _VerticalRotationRange.y);

            obj.SetParent(depthPivot.transform);
            depthPivot.transform.SetParent(verticalPivot.transform);
            verticalPivot.transform.SetParent(horizontalPivot.transform);
            horizontalPivot.transform.SetParent(tiltPivot.transform);

            obj.transform.localPosition = new Vector3(0,0,0);
            depthPivot.transform.localPosition = new Vector3(0,0,depth);
            //tiltPivot.transform.localEulerAngles = new Vector3(0,hAngle,0);
            tiltPivot.transform.localEulerAngles = new Vector3(0,0,0);
            horizontalPivot.transform.localEulerAngles = new Vector3(0,hAngle,0);
            verticalPivot.transform.localEulerAngles = new Vector3(vAngle,0,0);

            timer = 0;

            // horizontal
            var rotControl = horizontalPivot.AddComponent<RotateObject>();
            rotControl.rotateY = _HorizontalSpeed;

            // tilt
            Sequence tiltSeq = DOTween.Sequence();
            var tiltDuration = Mathf.Abs(tiltPivot.transform.localEulerAngles.x - _TiltRotationRange.y) * _TiltDuration / Mathf.Abs(_TiltRotationRange.y - _TiltRotationRange.x);
            tiltSeq.Append(tiltPivot.transform.DOLocalRotate(new Vector3(_TiltRotationRange.y, 0, 0), tiltDuration).From(tiltPivot.transform.localEulerAngles).OnComplete(() => {
                tiltPivot.transform.DOLocalRotate(new Vector3(_TiltRotationRange.x, 0, 0), _VerticalDuration)
                .From(new Vector3(_TiltRotationRange.y, 0, 0))
                .SetLoops(int.MaxValue, LoopType.Yoyo)
                .SetEase(_TiltEase);
            }));
            tiltSeq.Play();

            // vertical
            Sequence verticalSeq = DOTween.Sequence();
            var verticalDuration = Mathf.Abs(_VerticalRotationRange.y - verticalPivot.transform.localEulerAngles.x) * _VerticalDuration / Mathf.Abs(_VerticalRotationRange.y - _VerticalRotationRange.x);
            verticalDuration *= .15f;
            verticalSeq.Append(verticalPivot.transform.DOLocalRotate(new Vector3(_VerticalRotationRange.y, 0, 0), verticalDuration).From(verticalPivot.transform.localEulerAngles).OnComplete(() => {
                verticalPivot.transform.DOLocalRotate(new Vector3(_VerticalRotationRange.x, 0, 0), _VerticalDuration)
                .From(new Vector3(_VerticalRotationRange.y, 0, 0))
                .SetLoops(int.MaxValue, LoopType.Yoyo)
                .SetEase(_VerticalEase);
            }));
            verticalSeq.Play();

            Sequence endSeq = DOTween.Sequence();
            endSeq.Append(depthPivot.transform.DOLocalMoveZ(_EndDepthValue, _DepthDuration).SetEase(Ease.InOutCubic));
                        
            TweenCallback StopHorizontalCallback = () => {
                print("Stop Horizontal " + timer);
                tiltSeq.Kill();
                verticalSeq.Kill();

                float yValue = rotControl.rotateY;                
                DOTween.To(() => yValue, x => yValue = x, 0, .5f).OnUpdate(() => rotControl.rotateY = yValue);

                var horizontalDuration1 = horizontalPivot.transform.localEulerAngles.y / _HorizontalSpeed;

                tiltPivot.transform.DOLocalRotate(new Vector3(0, 0, 0), horizontalDuration1).SetEase(Ease.InOutSine);
                verticalPivot.transform.DOLocalRotate(new Vector3(0, 0, 0), horizontalDuration1).SetEase(Ease.InOutSine);

                horizontalPivot.transform.DOLocalRotate(Vector3.zero, horizontalDuration1, RotateMode.Fast).SetEase(Ease.InOutSine).OnComplete(() => { 
                    depthPivot.transform.SetParent(_Container);
                    Destroy(tiltPivot);

                    var target = transform.position + transform.forward * _EndDepthValue;
                    var endDuration = Vector3.Distance(depthPivot.transform.position, target) / (_HorizontalSpeed * .2f);
                    depthPivot.transform.DOMove(target, endDuration).OnComplete(() => { 
                        obj.SetParent(_Container); 
                        Destroy(depthPivot.gameObject); 
                    });
                    print("Start lerping " + timer);
                    print("horizontalDuration1 " + horizontalDuration1);
                    print("endDuration " + endDuration);

                    //AutoGrabSeq(obj);
                });
            };
            endSeq.AppendCallback(StopHorizontalCallback);   

            endSeq.Play();
            
            AutoGrabSeq(obj, wave);
        }
        private void SetUpExplosionItem(Transform obj, int wave)
        {
            obj.SetParent(_PopOutCenter);
            obj.localScale *= _ScaleMultiplier;
            
            Sequence seq = DOTween.Sequence();
            var dir = Quaternion.AngleAxis(wave * _ExplosionAngle, Vector3.forward) * Vector3.up;
            seq.Append(obj.DOMove(_PopOutCenter.position + dir * _Radius, _Duration).From(_PopOutCenter.position));
            seq.AppendInterval(_StayDuration);

            TweenCallback tweenCallback = () => { AutoGrabSeq(obj, wave); };
            seq.AppendCallback(tweenCallback);
            
            seq.Play();
        }
        private void AutoGrabSeq(Transform obj, int wave) {

            Sequence autoGrabSeq = DOTween.Sequence();

            //autoGrabSeq.AppendInterval(hDur + endDur);
            var duration = _AutoGrabDuration;

            if (_ItemShowingType == ItemShowingType.PopOut)
                duration = _AutoGrabDuration + wave * _OffsetAutoGrabTiming - Mathf.Pow(wave, 2) * _SpeedUpMul;

            autoGrabSeq.AppendInterval(duration);

            TweenCallback StartAutoGrab = () =>
            {
                print("start auto grab " + timer);
                var interactablePower = obj.GetComponentInChildren<InteractableArmorUpgrade>();
                
                if (interactablePower)
                {
                    interactablePower?.OnAutoSelect();

                    print("auto grab " + timer);
                }
                print(obj.name);
                timer = 0;
            };
            autoGrabSeq.AppendCallback(StartAutoGrab);

            autoGrabSeq.Play();
        }
        private IEnumerator TypeChanger() {
            int loop = 0;
            float changeTypeTimer = _ChangeTypeTimer;

            while (true)
            {
                if(loop != 0) changeTypeTimer = _ChangeTypeFastTimer;

                yield return new WaitForSeconds(changeTypeTimer);                               

                if (currentType != ArmorType.Mercy) currentType++;
                else currentType = 0;

                ChangeObjectType(currentType, currentParts);

                loop++;
            }
        }

        private ArmorType GetArmorTypeFromSo(){
            var targetItemName = menuItemSo.targetItemName;
            var armorType = Enum.GetValues(typeof(ArmorType)).Cast<ArmorType>().ToList();
            var armorTypeNames = armorType.Select(x => x.ToString()).ToList();
            var foundIndex = armorTypeNames.FindIndex(x => targetItemName.Contains(x));
            
            if(foundIndex < 0) foundIndex = 0;
            Debug.Log($"name {targetItemName} , {foundIndex} type {armorType[foundIndex]}");

            return armorType[foundIndex];
        }
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position + transform.forward * _DepthRange.x, transform.position + transform
                    .forward * _DepthRange.y);
            Gizmos.DrawSphere(transform.position + transform.forward * _EndDepthValue, .1f);
        }
    }
}
