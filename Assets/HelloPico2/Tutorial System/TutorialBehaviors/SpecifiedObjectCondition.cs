using System;
using UnityEngine;

namespace HelloPico2.TutorialSystem{
	[Serializable]
	public class SpecifiedObjectCondition : ConditionBase{
		[SerializeField] private string objectTag;
		[SerializeField] private int count;
		public bool isPass;
		private int _counter;

		public void Condition(SpecifiedObjectBehavior behavior){
			var specified = behavior.Specified;
			if(specified.tag.Equals(objectTag)){
				_counter++;
				if(_counter >= count) isPass = true;
			}
		}
	}
}