using Sirenix.OdinInspector;
using UnityEngine;
using HelloPico2.LevelTool;

namespace HelloPico2.RhythmCreate
{
    public class NoteController : MonoBehaviour
    {
        [EnumPaging] public Melanchall.DryWetMidi.MusicTheory.NoteName _SpawnerName;
        
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
