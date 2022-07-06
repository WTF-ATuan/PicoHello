using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HelloPico2.InteractableObjects;
using Sirenix.OdinInspector;

namespace HelloPico2.LevelTool
{
    public class SpawnersManager : MonoBehaviour
    {
        public enum SpawnDirection { 
            useSpawnerForward,
            AimPlayer
        }
        [System.Serializable]
        public struct SpawnerInfo
        {
            public string Name;
            public string Description;
            public BaseSpawner Spawner;
        }
        [System.Serializable]
        public struct SpawnerSet
        {
            public string Name;
            public string Description;
            public List<SpawnerInfo> Spawners;
        }
        public List<SpawnerSet> _SpawnerSetsLibrary = new List<SpawnerSet>();
        [FoldoutGroup("Library Settings")][System.Serializable]
        public struct HitTargetInfo
        {
            public string Name;
            public string Description;
            public HitTargetBase Prefab;
        }
        public List<HitTargetInfo> _HitTargetLibrary = new List<HitTargetInfo>();
        [FoldoutGroup("Library Settings")][System.Serializable]
        public struct InteractableInfo
        {
            public string Name;
            public string Description;
            public InteractableBase Prefab;
        }
        public List<InteractableInfo> _InteractableLibrary = new List<InteractableInfo>();
        
        [FoldoutGroup("Help Player When Running out of Power")]
        private bool _StartHelping = false;
        [FoldoutGroup("Help Player When Running out of Power")]        
        public string _HelpObjectName; 
        [FoldoutGroup("Help Player When Running out of Power")]
        public SpawnDirection _HelpObjectDir; 
        [FoldoutGroup("Help Player When Running out of Power")]
        public float _HelpObjectSpeed; 
        [FoldoutGroup("Help Player When Running out of Power")]
        public int _HelpObjectAmount;
        [FoldoutGroup("Help Player When Running out of Power")]
        public float _HelpObjectInterpolate;
        [FoldoutGroup("Help Player When Running out of Power")]
        public float _HelpObjectCoolDown;
        [FoldoutGroup("Help Player When Running out of Power")]
        public List<BaseSpawner> _HelpObjectSpawnPoints;

        public Transform _Player;
        public Transform _SpawnObjContainer;
        public int _CurrentSpawnersSet;
        public int CurrentSpawnersSet { get { return _CurrentSpawnersSet; }set { _CurrentSpawnersSet = value; } }

        Coroutine _SpawnHelpObjectProcess;

        #region Singleton
        public static SpawnersManager _instance;
        public static SpawnersManager instance
        {
            get {
                if (_instance == null)
                    _instance = GameObject.FindObjectOfType<SpawnersManager>();

                return _instance;
            }
        }
        #endregion

        [Button]
        private void RefreshSpawnersName() {
            for (int i = 0; i < _SpawnerSetsLibrary[_CurrentSpawnersSet].Spawners.Count; i++)
            {
                SpawnerInfo info = _SpawnerSetsLibrary[_CurrentSpawnersSet].Spawners[i];
                info.Name = _SpawnerSetsLibrary[_CurrentSpawnersSet].Spawners[i].Spawner.name;
            }
        }
        #region Library
        private List<BaseSpawner> GetSpawners(string name)
        {
            List<BaseSpawner> spawner = new List<BaseSpawner>();
            for (int i = 0; i < _SpawnerSetsLibrary[_CurrentSpawnersSet].Spawners.Count; i++)
            {
                if (_SpawnerSetsLibrary[_CurrentSpawnersSet].Spawners[i].Name == name)
                    spawner.Add(_SpawnerSetsLibrary[_CurrentSpawnersSet].Spawners[i].Spawner);
            }

            return spawner;
        }
        private HitTargetBase GetHitTarget(string prefabName ) {
            for (int i = 0; i < _HitTargetLibrary.Count; i++)
            {
                if (_HitTargetLibrary[i].Name == prefabName)
                    return _HitTargetLibrary[i].Prefab;
            }
            return null;
        }
        private InteractableBase GetInteractable(string prefabName)
        {
            for (int i = 0; i < _InteractableLibrary.Count; i++)
            {
                if (_InteractableLibrary[i].Name == prefabName)
                    return _InteractableLibrary[i].Prefab;
            }
            return null;
        }
        #endregion
        private Vector3 GetDirection(SpawnDirection dirType, BaseSpawner spawner) {
            switch (dirType)
            {
                case SpawnDirection.useSpawnerForward: 
                    return spawner.transform.forward;
                case SpawnDirection.AimPlayer:
                    return (_Player.transform.position - spawner.transform.position).normalized;
                default:
                    return spawner.transform.forward;
            }
        }
        public void Spawn(string spawnerName)
        {
            var spawners = GetSpawners(spawnerName);

            foreach (var spawner in spawners)
            {
                switch (spawner._SpawnType)
                {
                    case BaseSpawner.SpawnType.Interactable:
                        SpawnInteractable(spawner, spawner._SpawnDirection, spawner._InteractableType, spawner._Speed);
                        break;
                    case BaseSpawner.SpawnType.HitTarget:
                        SpawnHitTarget(spawner, spawner._SpawnDirection, spawner._HitTargetType, spawner._Speed);
                        break;
                }
            }
        }
        public void SpawnInteractable(BaseSpawner spawner, SpawnDirection dirType, string InteractableName, float speed)
        {
            var prefab = GetInteractable(InteractableName);
            var dir = GetDirection(dirType, spawner);

            var clone = Instantiate(prefab, spawner.transform.position, Quaternion.LookRotation(dir));
            clone.transform.SetParent(_SpawnObjContainer);
            clone.SetUpMoveBehavior(dir, speed);
            Destroy(clone.gameObject, 90f);
        }
        public void SpawnHitTarget(BaseSpawner spawner, SpawnDirection dirType, string hitTargetName, float speed) { 
            var prefab = GetHitTarget(hitTargetName);
            var dir = GetDirection(dirType, spawner);

            var clone = Instantiate(prefab, spawner.transform.position, Quaternion.LookRotation(dir));
            clone.transform.SetParent(_SpawnObjContainer);
            clone.SetUpMoveBehavior(dir, speed);
            Destroy(clone.gameObject, 90f);
        }
        private void Start()
        {
            _Player = GameObject.FindGameObjectWithTag("Player").transform;

            Project.EventBus.Subscribe<NeedEnergyEventData>(SpawnEnergy);
        }

        #region Help Player
        public void UseHelping(bool value) { 
            _StartHelping = value;
        }
        private void SpawnEnergy(NeedEnergyEventData eventData) {
            if (!_StartHelping || eventData.Energy > 0) return;

            if(_SpawnHelpObjectProcess == null)
                _SpawnHelpObjectProcess = StartCoroutine(SpawnProcess());
        }
        private IEnumerator SpawnProcess() {
            for (int i = 0; i < _HelpObjectAmount; i++)
            {
                var pick = _HelpObjectSpawnPoints[Random.Range(0, _HelpObjectSpawnPoints.Count)];
                SpawnInteractable(pick, _HelpObjectDir, _HelpObjectName, _HelpObjectSpeed);
                yield return new WaitForSeconds(_HelpObjectInterpolate);
            }
            yield return new WaitForSeconds(_HelpObjectCoolDown);
            _SpawnHelpObjectProcess = null;
        }
        #endregion
    }
}
