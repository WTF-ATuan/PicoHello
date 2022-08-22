using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using HelloPico2.PlayerController.Arm;
using Sirenix.OdinInspector;
using Random = UnityEngine.Random;

namespace HelloPico2.LevelTool
{
    public class FloatingObjectMover : MonoBehaviour
    {
        public InteractableObjects.CollectableItemTrackingList _CollectableItemsSO;
        public TargetItem_SO menuItemSo;
        public ArmorType _StartingType => GetArmorTypeFromSo();
        public ArmorPart _StartingParts;
        public float _ChangeTypeTimer = 10;
        public float _SpawnScalingRatio = .5f;
        public float _SpawnScalingDuration = .5f;
        public int _vibrato = 5;

        [Header("Floating Behavior Settings")]
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
                
        public HelloPico2.PlayerController.Arm.ArmorType currentType { get; set; }
        public HelloPico2.PlayerController.Arm.ArmorPart currentParts { get; set; }

        Coroutine typeChangerProcess;

        public void StartSendingCollectableItem()
        {
            currentType = _StartingType;
            currentParts = _StartingParts;
            CheckSpawnObject(_StartingType, _StartingParts);
            typeChangerProcess = StartCoroutine(TypeChanger());
        }
        public void NextArmorParts()
        {
            if (currentParts == PlayerController.Arm.ArmorPart.UpperArm)
                return;
            else
                currentParts++;

            currentType = _StartingType;

            CheckSpawnObject(currentType, currentParts);
            
            if(typeChangerProcess != null)
                StopCoroutine(typeChangerProcess);

            typeChangerProcess = StartCoroutine(TypeChanger());
        }
        private float timer;
        private void Update()
        {
            timer += Time.deltaTime;
            if (currentFloatingObject != null) return;            
            
            NextArmorParts();
        }
        private void CheckSpawnObject(HelloPico2.PlayerController.Arm.ArmorType type, HelloPico2.PlayerController.Arm.ArmorPart part) {
            var item = _CollectableItemsSO.GetItem(type, part);
            var clone = CreateObject(item);

            currentFloatingObject = clone.transform;
            SetUpFloating(currentFloatingObject);
        }
        private void ChangeObjectType(HelloPico2.PlayerController.Arm.ArmorType type, HelloPico2.PlayerController.Arm.ArmorPart part)
        {
            var item = _CollectableItemsSO.GetItem(type, part);
            var clone = CreateObject(item);
            clone.transform.SetParent(currentFloatingObject.parent);
            clone.transform.localPosition = currentFloatingObject.localPosition;        

            Destroy(currentFloatingObject.gameObject);
            currentFloatingObject = clone.transform;            
        }
        private GameObject CreateObject(InteractableObjects.InteractableArmorUpgrade item) {
            var clone = Instantiate(item).gameObject;

            clone.transform.DOPunchScale(clone.transform.localScale * _SpawnScalingRatio, _SpawnScalingDuration, _vibrato);

            return clone;
        }
        
        [Button]
        private void SetUpFloating(Transform obj) {
            var tiltPivot =  Instantiate(new GameObject(), _Container);
            var horizontalPivot =  Instantiate(new GameObject(), _Container);
            var verticalPivot =  Instantiate(new GameObject(), _Container);
            var depthPivot =  Instantiate(new GameObject(), _Container);

            tiltPivot.name = "TiltPivot";
            horizontalPivot.name = "HorizontalPivot";
            verticalPivot.name = "VerticalPivot";
            depthPivot.name = "DepthPivot";

            var depth = Random.Range(_DepthRange.x, _DepthRange.y);
            var hAngle = Random.Range(0, 360);
            var vAngle = Random.Range(_VerticalRotationRange.x, _VerticalRotationRange.y);

            obj.SetParent(depthPivot.transform);
            depthPivot.transform.SetParent(verticalPivot.transform);
            verticalPivot.transform.SetParent(horizontalPivot.transform);
            horizontalPivot.transform.SetParent(tiltPivot.transform);

            obj.transform.localPosition = new Vector3(0,0,0);
            depthPivot.transform.localPosition = new Vector3(0,0,depth);
            tiltPivot.transform.localEulerAngles = new Vector3(0,hAngle,0);
            horizontalPivot.transform.localEulerAngles = new Vector3(0,hAngle,0);
            verticalPivot.transform.localEulerAngles = new Vector3(vAngle,0,0);

            timer = 0;

            // horizontal
            var rotControl = horizontalPivot.AddComponent<RotateObject>();
            rotControl.rotateY = _HorizontalSpeed;

            print("Start horizontal " + timer);

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

            print("Start tilt " + timer);

            // vertical
            Sequence verticalSeq = DOTween.Sequence();
            var verticalDuration = Mathf.Abs(verticalPivot.transform.localEulerAngles.x - _VerticalRotationRange.y) * _VerticalDuration / Mathf.Abs(_VerticalRotationRange.y - _VerticalRotationRange.x);
            verticalSeq.Append(verticalPivot.transform.DOLocalRotate(new Vector3(_VerticalRotationRange.y, 0, 0), verticalDuration).From(verticalPivot.transform.localEulerAngles).OnComplete(() => {
                verticalPivot.transform.DOLocalRotate(new Vector3(_VerticalRotationRange.x, 0, 0), _VerticalDuration)
                .From(new Vector3(_VerticalRotationRange.y, 0, 0))
                .SetLoops(int.MaxValue, LoopType.Yoyo)
                .SetEase(_VerticalEase);
            }));
            verticalSeq.Play();

            print("Start vertical " + timer);

            Sequence endSeq = DOTween.Sequence();
            endSeq.Append(depthPivot.transform.DOLocalMoveZ(_EndDepthValue, _DepthDuration).SetEase(Ease.InOutCubic));

            print("Start depth " + timer);

            TweenCallback StopHorizontalCallback = () => {
                print("Stop Horizontal " + timer);
                tiltSeq.Kill();
                verticalSeq.Kill();

                float yValue = rotControl.rotateY;                
                DOTween.To(() => yValue, x => yValue = x, 0, .5f).OnUpdate(() => rotControl.rotateY = yValue);

                var horizontalDuration1 = horizontalPivot.transform.localEulerAngles.y * _VerticalDuration / 360;
                horizontalPivot.transform.DOLocalRotate(Vector3.zero, horizontalDuration1, RotateMode.Fast).SetEase(Ease.Linear).OnComplete(() => { 
                    depthPivot.transform.SetParent(_Container);
                    Destroy(tiltPivot);

                    var target = transform.position + transform.forward * _EndDepthValue;
                    var endDuration = Vector3.Distance(depthPivot.transform.position, target);
                    depthPivot.transform.DOMove(target, endDuration).OnComplete(() => { obj.SetParent(_Container); Destroy(depthPivot.gameObject); });

                    print("Start lerping " + timer);
                });

            };
            endSeq.AppendCallback(StopHorizontalCallback);            
            endSeq.Play();
        }
        private IEnumerator TypeChanger() {
            while (true)
            {
                yield return new WaitForSeconds(_ChangeTypeTimer);
                               

                if (currentType != PlayerController.Arm.ArmorType.Mercy) currentType++;
                else currentType = 0;

                ChangeObjectType(currentType, currentParts);
            }
        }

        private ArmorType GetArmorTypeFromSo(){
            var targetItemName = menuItemSo.targetItemName;
            var armorType = Enum.GetValues(typeof(ArmorType)).Cast<ArmorType>().ToList();
            var armorTypeNames = armorType.Select(x => x.ToString()).ToList();
            var foundIndex = armorTypeNames.FindIndex(x => x.Equals(targetItemName));
            
            if(foundIndex < 0) foundIndex = 0;

            return armorType[foundIndex];
        }
    }
}
