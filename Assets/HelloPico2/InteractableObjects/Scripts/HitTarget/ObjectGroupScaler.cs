using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace HelloPico2.InteractableObjects
{
    [RequireComponent(typeof(ObjectScaler))]
    public class ObjectGroupScaler : MonoBehaviour
    {
        [FoldoutGroup("Scaling Settings")] public ObjectScaler _SettingsRef;
        public bool _TriggerMover;
        [FoldoutGroup("Moving Settings")][ShowIf("_TriggerMover")] public ObjectMover _MoveSettingsRef;
        [FoldoutGroup("Moving Settings")][ShowIf("_TriggerMover")] public Vector3 _MoveFromOffset;
        [FoldoutGroup("Moving Settings")][ShowIf("_TriggerMover")] public Vector3 _MoveToOffset;
        public List<GameObject> _GameObjectGroup = new List<GameObject>();

        [ReadOnly] public List<ObjectScaler> _ObjectScalerGroup = new List<ObjectScaler>();
        [ShowIf("_TriggerMover")][ReadOnly] public List<ObjectMover> _ObjectMoverGroup = new List<ObjectMover>();

        public GameObject _StartingObj;

        [FoldoutGroup("Sequencer Settings")] public float _InterpolateDuration = .1f;
        [FoldoutGroup("Sequencer Settings")] public bool _ActivateBeforeStart = true;        
        [FoldoutGroup("Sequencer Settings")] public bool _ActivateAfterFinished = false;

        Coroutine scalingSequenceProcess { get; set; }
        public delegate void ScalerDel(GameObject obj);
        public ScalerDel WhenStartedScaling;
        public ScalerDel WhenFinishedScaling;

        private void Start()
        {
            foreach (var item in _ObjectScalerGroup)
            {
                item.gameObject.SetActive(_ActivateBeforeStart);
            }
        }
        [FoldoutGroup("Scaling Settings")][Button]
        private void ApplyObjectScaler() {
            _ObjectScalerGroup.Clear();
            foreach (var item in _GameObjectGroup)
            {
                if (item.GetComponent<ObjectScaler>() != null)
                    DestroyImmediate(item.GetComponent<ObjectScaler>());

                var objScaler = item.AddComponent<ObjectScaler>();

                ApplySettings(objScaler, item);

                _ObjectScalerGroup.Add(objScaler);
            }    
        }
        [FoldoutGroup("Scaling Settings")][Button]
        private void UpdateSettings() {
            foreach (var item in _GameObjectGroup)
            {
                if (item.GetComponent<ObjectScaler>() != null)
                {
                    var objScaler = item.GetComponent<ObjectScaler>();
                    ApplySettings(objScaler, item);
                }
                else {
                    throw new System.Exception("Object need ObjectScaler");
                }
            }
        }
        [ShowIf("_TriggerMover")][FoldoutGroup("Moving Settings")]
        [Button]
        private void ApplyObjectMover()
        {
            foreach (var item in _GameObjectGroup)
            {
                if (item.GetComponent<ObjectMover>() != null)
                    DestroyImmediate(item.GetComponent<ObjectMover>());

                var objMover = item.AddComponent<ObjectMover>();

                ApplySettings(objMover, item);
            }
        }
        [ShowIf("_TriggerMover")][FoldoutGroup("Moving Settings")]
        [Button]
        private void UpdateMovingSettings()
        {
            foreach (var item in _GameObjectGroup)
            {
                if (item.GetComponent<ObjectMover>() != null)
                {
                    var objMover = item.GetComponent<ObjectMover>();
                    ApplySettings(objMover, item);
                }
                else
                {
                    throw new System.Exception("Object need ObjectScaler");
                }
            }
        }
        [Button]
        private void ReorderByDistance()
        {
            List<ObjectScaler> temp = new List<ObjectScaler>(_ObjectScalerGroup);

            for (int j = 0; j < temp.Count; j++)
            {
                float shortestDist = float.MaxValue;

                for (int i = 0; i < _ObjectScalerGroup.Count; i++)
                {
                    var dist = Vector3.Distance(_StartingObj.transform.position, _ObjectScalerGroup[i].transform.position);

                    if (dist < shortestDist)
                    {
                        shortestDist = dist;
                        temp[j] = _ObjectScalerGroup[i];
                    }
                }

                _ObjectScalerGroup.Remove(temp[j]);
            }

            _ObjectScalerGroup = temp;

            CheckUpdateObjectMoverGroup(temp);
        }
        [Button]
        private void ReorderByConnection() { 
            List<ObjectScaler> temp = new List<ObjectScaler>(_ObjectScalerGroup);

            var index = _GameObjectGroup.IndexOf(_StartingObj);
            temp[index] = temp[0];
            temp[0] = _StartingObj.GetComponent<ObjectScaler>();


            for (int j = 0; j < temp.Count - 1; j++)
            {
                float shortestDist = float.MaxValue;
                GameObject startObj = temp[j].gameObject;

                for (int i = 0; i < _ObjectScalerGroup.Count; i++)
                {
                    var dist = Vector3.Distance(startObj.transform.position, _ObjectScalerGroup[i].transform.position);
                                        
                    if (_ObjectScalerGroup[i] != temp[j] && dist < shortestDist)
                    {
                        shortestDist = dist;
                        temp[j + 1] = _ObjectScalerGroup[i];
                    }
                }

                _ObjectScalerGroup.Remove(temp[j]);
                _ObjectScalerGroup.Remove(temp[j + 1]);
            }

           _ObjectScalerGroup = temp;

            CheckUpdateObjectMoverGroup(temp);
        }
        private void CheckUpdateObjectMoverGroup(List<ObjectScaler> scalerGroupResult) {
            if (_TriggerMover)
            {
                var moverTemp = new List<ObjectMover>();

                for (int i = 0; i < scalerGroupResult.Count; i++)
                {
                    moverTemp.Add(scalerGroupResult[i].GetComponent<ObjectMover>());
                }

                _ObjectMoverGroup = new List<ObjectMover>(moverTemp);
            }
        }
        [FoldoutGroup("Sequencer Settings")]
        [Button]
        public void StartScalingGroup() {
            if (scalingSequenceProcess != null) return;
            scalingSequenceProcess = StartCoroutine(ScalingSequence());
        }
        private IEnumerator ScalingSequence() {
            foreach (var item in _ObjectScalerGroup)
            {
                item.gameObject.SetActive(_ActivateBeforeStart);
            }

            for (int i = 0; i < _ObjectScalerGroup.Count; i++)
            {                
                _ObjectScalerGroup[i].OnStarted += Activate;
                _ObjectScalerGroup[i].OnFinished += Deactivate;

                _ObjectScalerGroup[i].gameObject.SetActive(true);
                _ObjectScalerGroup[i].StartScaling();
                if (_TriggerMover) _ObjectMoverGroup[i].StartMoving();                

                yield return new WaitForSeconds(_InterpolateDuration);
            }

            scalingSequenceProcess = null;
        }
        private void Activate(GameObject obj) {
            WhenStartedScaling?.Invoke(obj);
        }
        private void Deactivate(GameObject obj) {
            WhenFinishedScaling?.Invoke(obj);
            obj.SetActive(_ActivateAfterFinished);
        }
        private void ApplySettings(ObjectScaler objScaler, GameObject obj) {

            objScaler.controlMode = _SettingsRef.controlMode;
            objScaler.scalingObject = obj;
            objScaler.from = MultiplyVector3(_SettingsRef.from, obj.transform.localScale);
            objScaler.to = MultiplyVector3(_SettingsRef.to, obj.transform.localScale);
            objScaler.duration = _SettingsRef.duration;
            objScaler.loop = _SettingsRef.loop;
            objScaler.easeCurve = _SettingsRef.easeCurve;
        }
        private void ApplySettings(ObjectMover objMover, GameObject obj)
        {
            objMover.moveObject = obj.transform;
            
            objMover.from = obj.transform.localPosition + _MoveFromOffset;
            objMover.to = obj.transform.localPosition + _MoveToOffset;

            objMover.duration = _MoveSettingsRef.duration;
            objMover.easeCurve = _MoveSettingsRef.easeCurve;
        }
        private Vector3 MultiplyVector3(Vector3 a, Vector3 b) {
            Vector3 result = new Vector3( 
                a.x * b.x,
                a.y * b.y,
                a.z * b.z                
                );
            return result;
        }
    }
}