using System;
using System.Collections.Generic;
using UnityEngine;

namespace HelloPico2.Event_Tracker{
	[CreateAssetMenu(fileName = "PlayerBehavior-Overview",
		menuName = "HelloPico2/ScriptableObject/ PlayerBehaviorOverview", order = 0)]
	public class PlayerBehaviorOverview : ScriptableObject{
		public List<BehaviorParams> behaviorParamsList;

		public BehaviorParams FindBehaviorParams(string paramsName){
			var behaviorParams = behaviorParamsList.Find(x => x.paramsName.Equals(paramsName));
			if(behaviorParams != null){
				return behaviorParams;
			}

			throw new Exception($"Can,t find {paramsName}");
		}
	}
}