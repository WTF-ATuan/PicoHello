using System;
using System.Collections.Generic;
using System.Linq;
using Game.Project;
using UnityEngine;
using UnityEngine.Serialization;

namespace HelloPico2.Trajectory{
	public class TrajectorySpawner : MonoBehaviour{
		[FormerlySerializedAs("spawnObject")] [SerializeField]
		private List<GameObject> spawnObjectList;

		[SerializeField] private int generateCount;
		[SerializeField] private float randomDuring;


		[SerializeField] private List<Transform> tipList;

		private ColdDownTimer _timer;

		private void Start(){
			GetTipsChildRecursive(gameObject, tipList);
			_timer = new ColdDownTimer(randomDuring);
		}

		private void Update(){
			if(_timer.CanInvoke()){
				RandomSpawn();
				_timer.Reset();
			}
		}

		private void RandomSpawn(){
			var randomPointList = tipList.OrderBy(arg => Guid.NewGuid()).Take(generateCount).ToList();
			var randomObject = spawnObjectList.OrderBy(arg => Guid.NewGuid()).First();
			foreach(var point in randomPointList){
				Instantiate(randomObject, point.position, Quaternion.identity);
			}
		}

		private void GetTipsChildRecursive(GameObject obj, List<Transform> childList){
			if(null == obj)
				return;
			foreach(Transform child in obj.transform){
				if(null == child)
					continue;
				var childGameObject = child.gameObject;
				if(childGameObject.name.Contains("Tip")){
					childList.Add(childGameObject.transform);
				}

				GetTipsChildRecursive(childGameObject, childList);
			}
		}
	}
}