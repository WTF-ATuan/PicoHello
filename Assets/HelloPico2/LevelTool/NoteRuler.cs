using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HelloPico2.LevelTool
{
    public class NoteRuler : MonoBehaviour
    {
        [System.Serializable]
        public struct SpawnerData { 
            public Melanchall.DryWetMidi.MusicTheory.NoteName Stamp;
            public LevelObjectSpawner HitTargetSpawner;
        }
        public List<SpawnerData> _SpawnerDatas = new List<SpawnerData>();
        public Transform _Player;
        public bool _AutoAdjustSpawnerPosition = true;
        public float _PlayerHight;
        public float _LaneOffset;
        public float _Depth;
        [Header("Gizmos")]
        public float _SpawnerRadius;
        
        public void OnValidate()
        {
            if (_Player == null || !_AutoAdjustSpawnerPosition) return;

            var start = Mathf.FloorToInt(_SpawnerDatas.Count / 2) * _LaneOffset;

            // even
            if (_SpawnerDatas.Count % 2 == 0) start -= _LaneOffset / 2;

            for (int i = 0; i < _SpawnerDatas.Count; i++)
            {
                if(_SpawnerDatas[i].HitTargetSpawner == null) continue;
                _SpawnerDatas[i].HitTargetSpawner.transform.position = _Player.position + _Player.up * _PlayerHight + _Player.right * start - _Player.right * _LaneOffset * i + _Player.forward * _Depth;
            }
        }
        public void OnDrawGizmos()
        {
            if (_Player == null) return;

            // Draw Player
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(_Player.transform.position, Vector3.one * 1f);

            var start = Mathf.FloorToInt(_SpawnerDatas.Count / 2) * _LaneOffset;

            // even
            if (_SpawnerDatas.Count % 2 == 0) start -= _LaneOffset / 2;
            
            // Draw note line
            for (int i = 0; i < _SpawnerDatas.Count; i++)            
            {                
                Gizmos.color = Color.yellow;

                switch (_SpawnerDatas[i].HitTargetSpawner._SpawnDirection)
                {
                    case SpawnersManager.SpawnDirection.SpawnerForward:
                        Gizmos.DrawLine(
                        _SpawnerDatas[i].HitTargetSpawner.transform.position,
                        _SpawnerDatas[i].HitTargetSpawner.transform.position + _SpawnerDatas[i].HitTargetSpawner.transform.forward * _Depth);
                        break;
                    case SpawnersManager.SpawnDirection.PlayerCentered:
                        Gizmos.DrawLine(
                        _Player.position + _Player.up * _PlayerHight + _Player.right * start - _Player.right * _LaneOffset * i,
                        _Player.position + _Player.up * _PlayerHight + _Player.right * start - _Player.right * _LaneOffset * i + _Player.forward * _Depth);
                        break;
                    case SpawnersManager.SpawnDirection.AimPlayer:
                        Gizmos.DrawLine(
                        _Player.position,
                        _SpawnerDatas[i].HitTargetSpawner.transform.position);
                        break;
                    default:
                        break;
                }                
            }
            Vector3 spawnerPos;
            Vector3 arrowTip;
            Vector3 dirL;
            Vector3 dirR;
            // Draw Spawner
            for (int i = 0; i < _SpawnerDatas.Count; i++)
            {
                spawnerPos = _SpawnerDatas[i].HitTargetSpawner.transform.position;
                DrawSpawner(_SpawnerDatas[i], spawnerPos);

                // Draw Arrow                
                switch (_SpawnerDatas[i].HitTargetSpawner._SpawnDirection)
                {
                    case SpawnersManager.SpawnDirection.SpawnerForward:
                        
                        break;
                    case SpawnersManager.SpawnDirection.PlayerCentered:                        
                        arrowTip = spawnerPos + _SpawnerDatas[i].HitTargetSpawner.transform.forward * 2;
                        Gizmos.DrawLine(spawnerPos, arrowTip);

                        dirL = Quaternion.Euler(0, -30, 0) * (-_SpawnerDatas[i].HitTargetSpawner.transform.forward);
                        dirR = Quaternion.Euler(0, 30, 0) * (-_SpawnerDatas[i].HitTargetSpawner.transform.forward);
                        Gizmos.DrawLine(arrowTip, arrowTip + dirL * 1.3f);
                        Gizmos.DrawLine(arrowTip, arrowTip + dirR * 1.3f);
                        break;
                    case SpawnersManager.SpawnDirection.AimPlayer:             
                        var playerDir = (_Player.transform.position - _SpawnerDatas[i].HitTargetSpawner.transform.position).normalized;
                        arrowTip = spawnerPos + playerDir * 2;
                        Gizmos.DrawLine(spawnerPos, arrowTip);

                        dirL = Quaternion.Euler(0, -30, 0) * (-playerDir);
                        dirR = Quaternion.Euler(0, 30, 0) * (-playerDir);
                        Gizmos.DrawLine(arrowTip, arrowTip + dirL * 1.3f);
                        Gizmos.DrawLine(arrowTip, arrowTip + dirR * 1.3f);
                        break;
                    default:
                        break;
                }
            }
        }
        private void DrawSpawner(SpawnerData data, Vector3 spawnerPos) {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(spawnerPos, _SpawnerRadius);    
        }
    }
}
