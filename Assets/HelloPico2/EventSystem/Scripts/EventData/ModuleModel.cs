using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HelloPico2{
	[System.Serializable]
	public class ModuleModel : ViewEventData{
		[Required] [OnValueChanged("OnModelRootChange")]
		public GameObject modelRoot;

		public List<GameObject> modelComponent = new List<GameObject>();

		private void OnModelRootChange(){
			if(!modelRoot) return;
		}
	}
}