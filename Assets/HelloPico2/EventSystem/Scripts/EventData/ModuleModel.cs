using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HelloPico2{
	[System.Serializable]
	public class ModuleModel : ViewEventData{
		[Required] public GameObject modelRoot;
		public List<GameObject> modelComponent = new List<GameObject>();
	}
}