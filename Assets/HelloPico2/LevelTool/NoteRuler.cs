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
            
            // Draw note line
            for (int i = 0; i < _SpawnerDatas.Count; i++)            
            {                
                Gizmos.color = Color.yellow;

                switch (_SpawnerDatas[i].HitTargetSpawner._SpawnDirection)
                {
                    case SpawnersManager.SpawnDirection.useSpawnerForward:
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

            // Draw Spawner
            for (int i = 0; i < _SpawnerDatas.Count; i++)
            {
                

                switch (_SpawnerDatas[i].HitTargetSpawner._SpawnDirection)
                {
                    case SpawnersManager.SpawnDirection.useSpawnerForward:
                        var spawnerPos = _Player.position + _Player.up * _PlayerHight + _Player.right * start - _Player.right * _LaneOffset * i + _Player.forward * _Depth;
                        Gizmos.color = Color.green;
                        Gizmos.DrawWireSphere(spawnerPos, _SpawnerRadius);

                        // Draw Arrow                
                        var arrowTip = spawnerPos + _SpawnerDatas[i].HitTargetSpawner.transform.forward * 2;
                        Gizmos.DrawLine(spawnerPos, arrowTip);

                        var dirL = Quaternion.Euler(0, -30, 0) * (-_SpawnerDatas[i].HitTargetSpawner.transform.forward);
                        var dirR = Quaternion.Euler(0, 30, 0) * (-_SpawnerDatas[i].HitTargetSpawner.transform.forward);
                        Gizmos.DrawLine(arrowTip, arrowTip + dirL * 1.3f);
                        Gizmos.DrawLine(arrowTip, arrowTip + dirR * 1.3f);
                        break;
                    case SpawnersManager.SpawnDirection.AimPlayer:
                        spawnerPos = _SpawnerDatas[i].HitTargetSpawner.transform.position;
                        Gizmos.color = Color.green;
                        Gizmos.DrawWireSphere(spawnerPos, _SpawnerRadius);

                        // Draw Arrow                
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
    }
}
