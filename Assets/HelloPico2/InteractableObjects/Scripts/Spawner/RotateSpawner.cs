using System.Collections.Generic;
using System.Linq;
using Game.Project;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace HelloPico2.InteractableObjects{
	public class RotateSpawner : MonoBehaviour{
		[TitleGroup("RotateSetting")] [OnValueChanged("EditSpawnPosition")]
		public float spawnRadius = 5f;

		[TitleGroup("RotateSetting")] [OnValueChanged("EditSpawnPosition")]
		public int spawnCount = 2;

		[TitleGroup("Debug")] public bool debug;

		[TitleGroup("Debug")] [ShowIf("debug")] [SerializeField] [ReadOnly]
		private List<Transform> spawnPointList = new List<Transform>();

		[TitleGroup("Debug")] [ShowIf("debug")] [SerializeField] [ReadOnly]
		private List<GameObject> cloneList = new List<GameObject>();

		[TitleGroup("Debug")] [ShowIf("debug")] [SerializeField]
		private Color gizmosColor = Color.green;

		private ColdDownTimer _timer;

		public void EditSpawnPosition(){
			spawnPointList.ForEach(x => DestroyImmediate(x.gameObject));
			spawnPointList.Clear();
			var angle = 360 / spawnCount;
			for(var i = 0; i < spawnCount; i++) CreateRotatePoint(spawnRadius, angle * (i + 1));
		}

		private void CreateRotatePoint(float radius, float angle){
			var spawnerTransform = transform;
			var position = spawnerTransform.position;
			var spawnPosition = position + radius * spawnerTransform.up;
			var vectorAngle = angle * spawnerTransform.right;
			var rotatePosition = Quaternion.Euler(vectorAngle) * (spawnPosition - position) + position;
			var spawnPoint = new GameObject($"Rotate Point + [angle : {angle} radius : {radius}]"){
				transform ={
					position = rotatePosition,
					parent = spawnerTransform
				}
			};
			spawnPointList.Add(spawnPoint.transform);
		}

		private void OnDrawGizmos(){
			if(spawnPointList.Exists(x => x == null)) spawnPointList.Clear();

			if(!debug){
				if(spawnPointList.IsNullOrEmpty()) return;
				spawnPointList.ForEach(x => DestroyImmediate(x.gameObject));
				spawnPointList.Clear();
				return;
			}

			Gizmos.color = gizmosColor;

			foreach(var spawnPosition in spawnPointList.Select(spawnPoint => spawnPoint.position))
				Gizmos.DrawWireCube(spawnPosition, Vector3.one * 0.5f);
		}
	}

	public enum RotateDirection{
		Right,
		Up,
		Forward
	}
}