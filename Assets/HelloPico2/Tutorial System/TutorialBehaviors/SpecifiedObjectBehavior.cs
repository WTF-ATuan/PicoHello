using UnityEngine;

namespace HelloPico2.TutorialSystem{
	public class SpecifiedObjectBehavior{
		public GameObject Specified{ get; }

		public SpecifiedObjectBehavior(GameObject specified){
			Specified = specified;
		}
	}
}