using System.Collections.Generic;
using System.Linq;
using Game.Project;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace HelloPico2.InteractableObjects{
	public class RotateSpawner : MonoBehaviour{
		[Required] public GameObject spawnPrefab;


		[TitleGroup("RotateSetting")] [OnValueChanged("EditSpawnPosition")]
		public float spawnRadius = 5f;

		[TitleGroup("RotateSetting")] [OnValueChanged("EditSpawnPosition")]
		public int spawnCount = 2;

		[SerializeField] [TitleGroup("During Setting")] [MinMaxSlider(0, 20, true)]
		private Vector2 duringMinMax = new Vector2(0.5f, 1f);


		[TitleGroup("Edit")] public bool edit;

		[TitleGroup("Edit")] [ShowIf("edit")] [ReadOnly]
		public List<Transform> spawnPointList = new List<Transform>();

		[TitleGroup("Edit")] [ShowIf("edit")] [SerializeField] [ReadOnly]
		private List<GameObject> cloneList = new List<GameObject>();

		[TitleGroup("Edit")] [ShowIf("edit")] [SerializeField]
		private Color gizmosColor = Color.green;

		private ColdDownTimer _timer;

		private void Start(){
			_timer = new ColdDownTimer();
			spawnCount = spawnPointList.Count;
		}

		private void FixedUpdate(){
			if(!_timer.CanInvoke()) return;
			Spawn();
			var randomDuring = Random.Range(duringMinMax.x, duringMinMax.y);
			_timer.ModifyDuring(randomDuring);
			_timer.Reset();
		}

		private void Spawn(){
			var spawnIndex = cloneList.Count;
			if(spawnIndex >= spawnCount) return;
			var spawnPoint = spawnPointList[spawnIndex];
			var spawnObject = Instantiate(spawnPrefab, spawnPoint.position, Quaternion.identity, spawnPoint);
			cloneList.Add(spawnObject);
		}

		public void EditSpawnPosition(){
			spawnPointList.ForEach(x => DestroyImmediate(x.gameObject));
			spawnPointList.Clear();
			var angle = 360 / spawnCount;
			for(var i = 0; i < spawnCount; i++){
				var point = CreatePoint(spawnRadius, angle * (i + 1));
				point.name = $"Edit Point [ index : {i} radius : {spawnRadius}]";
				spawnPointList.Add(point.transform);
			}
		}

		private GameObject CreatePoint(float radius, float angle){
			var spawnerTransform = transform;
			var position = spawnerTransform.position;
			var spawnPosition = position + radius * Vector3.up;
			var vectorAngle = angle * Vector3.right;
			var rotatePosition = Quaternion.Euler(vectorAngle) * (spawnPosition - position) + position;
			var spawnPoint = new GameObject{
				transform ={
					position = rotatePosition,
					parent = spawnerTransform
				}
			};
			return spawnPoint;
		}

		private void OnDrawGizmos(){
			if(spawnPointList.Exists(x => x == null)) spawnPointList.RemoveAll(x => x == null);

			if(!edit){
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
}