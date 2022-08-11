using System;
using System.Collections.Generic;
using System.Linq;
using Game.Project;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HelloPico2.InteractableObjects{
	public class CurveSpawner : MonoBehaviour{
		[Required] public GameObject spawnPrefab;

		[TitleGroup("Point Setting")] [OnValueChanged("EditCurve")]
		public AnimationCurve curvePoint = AnimationCurve.Linear(0, 0, 1, 1);

		[TitleGroup("Point Setting")] [SerializeField] [OnValueChanged("EditCurve")] [Min(1)]
		private int spawnCount = 2;

		[SerializeField] [TitleGroup("During Setting")] [MinMaxSlider(0, 5, true)]
		private Vector2 duringMinMax = new Vector2(0.5f, 1f);

		[TitleGroup("Debug")] public bool debug;

		[TitleGroup("Debug")] [ShowIf("debug")] [SerializeField] [ReadOnly]
		private List<Vector3> spawnPositions = new List<Vector3>();

		[TitleGroup("Debug")] [ShowIf("debug")] [SerializeField] [ReadOnly]
		private List<GameObject> cloneList = new List<GameObject>();

		[TitleGroup("Debug")] [ShowIf("debug")] [SerializeField]
		private Color gizmosColor = Color.green;

		private ColdDownTimer _timer;
		private void Start(){
			InitSpawnPosition();
		}

		private void EditCurve(){
			var pointLength = curvePoint.length;
			if(pointLength > spawnCount){
				RemovePoint();
			}

			if(pointLength < spawnCount){
				AddPoint();
			}

			InitSpawnPosition();
		}

		private void InitSpawnPosition(){
			spawnPositions.Clear();
			foreach(var curveKey in curvePoint.keys){
				var xValue = curveKey.time;
				var yValue = curveKey.value;
				var position = transform.position;
				var spawnPos = new Vector3(position.x + xValue, position.y + yValue, position.z);
				spawnPositions.Add(spawnPos);
			}
		}

		private void AddPoint(){
			while(curvePoint.length < spawnCount){
				var maxX = curvePoint.keys.Max(x => x.time);
				var maxY = curvePoint.keys.Max(x => x.value);
				curvePoint.AddKey(maxX + 1, maxY);
			}
		}

		private void RemovePoint(){
			while(curvePoint.length > spawnCount){
				curvePoint.RemoveKey(curvePoint.length - 1);
			}
		}

		private void OnDrawGizmos(){
			if(!debug) return;
			Gizmos.color = gizmosColor;
			if(spawnPrefab){
				var mesh = spawnPrefab.GetComponent<MeshFilter>().sharedMesh;
				foreach(var spawnPosition in spawnPositions){
					Gizmos.DrawWireMesh(mesh, spawnPosition);
				}
			}
			else{
				foreach(var spawnPosition in spawnPositions){
					Gizmos.DrawWireCube(spawnPosition, Vector3.one * 0.5f);
				}
			}
		}
	}
}