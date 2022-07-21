using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace HelloPico2.LevelTool
{
    public class LevelObjectSpawner : BaseSpawner
    {
        public bool _Testing = false;
        [ShowIf("_Testing")] public HelloPico2.InteractableObjects.HitTargetBase prefab;
        [ShowIf("_Testing")] public float _SpawnCD = 1;
        HelloPico2.LevelTool.SpawnersManager.SpawnDirection dir = SpawnersManager.SpawnDirection.SpawnerForward; 
        
        private void OnEnable()
        {
            if (_Testing)
                StartCoroutine(Spawning());
        }
        private IEnumerator Spawning() {
            while (true)
            {
                SpawnHitTarget(this, _Speed, _UseExternalForce, _Force);
                yield return new WaitForSeconds(_SpawnCD);
            }
        }
        public void SpawnHitTarget(BaseSpawner spawner, float speed, bool useGravity = false, float gravity = 0)
        {
            var clone = Instantiate(prefab, spawner.transform.position, Quaternion.LookRotation(transform.forward));
            clone.transform.SetParent(transform);
            //clone.SetUpMoveBehavior(transform.forward, speed, useGravity, gravity);
            clone.gameObject.AddComponent<MoveLevelObject>().SetUpMoveLevelObject(transform.forward, spawner._Speed, spawner._UseExternalForce, spawner._ForceDir, spawner._Force);
            Destroy(clone.gameObject, 90f);
        }
    }
}
