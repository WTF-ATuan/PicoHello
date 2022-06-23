using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HelloPico2{
	[CreateAssetMenu(fileName = "ViewEventDataOverview",
		menuName = "HelloPico2/ScriptableObject/ ViewEventData Overview",
		order = 0)]
	public class ViewEventDataOverview : ScriptableObject{
		[TypeFilter("GetEventType")] [HideReferenceObjectPicker] [InlineButton("CreateEvent")]
		public ViewEventData eventData;

		private IEnumerable GetEventType(){
			var eventType = typeof(ViewEventData).Assembly
					.GetTypes()
					.Where(x => !x.IsAbstract)
					.Where(x => !x.IsGenericTypeDefinition)
					.Where(x => typeof(ViewEventData).IsAssignableFrom(x));
			return eventType;
		}

		private void CreateEvent(){
			var type = eventData.GetType();
			if(type == typeof(ViewEventData)) Debug.Log($"Choose else Event");
			var instance = Activator.CreateInstance(type);
			var data = (ViewEventData)instance;
			viewEventDataList.Add(data);
		}

		[SerializeReference] [PropertyOrder(100)]
		private List<ViewEventData> viewEventDataList = new List<ViewEventData>();

		public T FindEventData<T>(string id) where T : ViewEventData{
			var viewEventData = viewEventDataList.Find(x => x.identity.Equals(id));
			if(viewEventData == null){
				throw new NullReferenceException($"Can,t Not Find {id}");
			}

			return viewEventData as T;
		}
	}
}