using System;
using System.Collections.Generic;
using Project;
using UnityEngine;

namespace HelloPico2.Data_Structure{
	public class DataAdapter : MonoBehaviour{
		private void RegisterFindEvent(){
			EventBus.Subscribe<FindRequested, List<object>>(OnFindRequested);
		}

		private List<object> OnFindRequested(FindRequested requested){
			var dataType = requested.RequestDataType;
			var findMatchData = FindMatchData(dataType);
			return findMatchData;
		}

		private List<object> FindMatchData(Type matchType){
			//Find MatchType Data in overview
			var matchList = new List<object>();
			return matchList;
		}
	}
}