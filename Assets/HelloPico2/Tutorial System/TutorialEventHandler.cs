using System;
using Game.Project;
using Project;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace HelloPico2.TutorialSystem{
	public class TutorialEventHandler : MonoBehaviour{
		[SerializeField] [Required] private TutorialDataOverview dataOverview;


		private void Start(){
			dataOverview.tutorialDataList.ForEach(x => x.ResetData());
			EventBus.Subscribe<TutorialBehaviorDetected>(OnTutorialBehaviorDetected);
		}

		private void OnTutorialBehaviorDetected(TutorialBehaviorDetected obj){
			var detectedType = obj.DetectedType;
			var trackedData = obj.TrackedData;
			var condition = dataOverview.FindCondition(detectedType);
			condition.EnterTrackedData(trackedData);
			Debug.Log(condition.pass ? $"Win" : $"You Bad");
		}
	}
}