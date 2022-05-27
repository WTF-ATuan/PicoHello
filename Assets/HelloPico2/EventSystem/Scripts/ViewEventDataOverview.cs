using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HelloPico2{
	[CreateAssetMenu(fileName = "ViewEventDataOverview",
		menuName = "HelloPico2/ScriptableObject/ ViewEventData Overview",
		order = 0)]
	public class ViewEventDataOverview : ScriptableObject{
		[Button]
		[PropertyOrder(0)]
		public void CreateAudioEvent(){
			var audioEvent = new AudioData();
			viewEventDataList.Add(audioEvent);
		}
		[Button]
		[PropertyOrder(0)]
		public void CreateParticleEvent(){
			var particleData = new ParticleData();
			viewEventDataList.Add(particleData);
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