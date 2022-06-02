using UnityEngine;

namespace HelloPico2{
	public class ModuleModelEventRequested{
		public string ModelID{ get; }
		public Vector3 Position{ get; }

		public ModuleModelEventRequested(string modelID, Vector3 position){
			ModelID = modelID;
			Position = position;
		}
	}
}