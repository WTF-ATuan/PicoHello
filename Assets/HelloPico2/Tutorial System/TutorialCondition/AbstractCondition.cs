using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HelloPico2.TutorialSystem{
	[Serializable]
	public abstract class AbstractCondition{
		[ReadOnly] public bool pass;
		public abstract void EnterTrackedData(BehaviorTrackedData trackedData);

		public void Reset(){
			pass = false;
		}
	}

	public class IsGripCondition : AbstractCondition{
		public override void EnterTrackedData(BehaviorTrackedData trackedData){
			var trackedDataIsGrip = trackedData.IsGrip;
			if(trackedDataIsGrip){
				pass = true;
			}
		}
	}

	[Serializable]
	public class SwordCondition : AbstractCondition{
		[SerializeField] private float minimumYOffset;

		public override void EnterTrackedData(BehaviorTrackedData trackedData){
			var yValue = trackedData.TouchPadAxis.y;
			if(yValue > minimumYOffset){
				pass = true;
			}
		}
	}
}