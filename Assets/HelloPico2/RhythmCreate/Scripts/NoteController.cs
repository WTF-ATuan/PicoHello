using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HelloPico2.LevelTool;

namespace HelloPico2.RhythmCreate
{
    public class NoteController : MonoBehaviour
    {
        public string _SpawnerName;
        
        private Scripts.LaneSpawner _LaneSpawner;

        private void OnEnable()
        {
            _LaneSpawner = GetComponent<Scripts.LaneSpawner>();
            _LaneSpawner.OnSpawn += NoteSetUp;
        }
        private void OnDisable()
        {
            _LaneSpawner.OnSpawn -= NoteSetUp;
        }
        public void NoteSetUp(GameObject note)
        {
            Destroy(note);
            SpawnersManager.instance.Spawn(_SpawnerName);
        }
    }
}
