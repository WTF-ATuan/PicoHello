using UnityEngine;

namespace Game.Project{
	public class ColdDownTimer{
		private float During{ get; set; }
		private float TrackTime{ get; set; }

		public ColdDownTimer(float during){
			During = during;
			TrackTime = Time.time;
		}

		public void ModifyDuring(float during){
			if(during <= 0) return;
			During = during;
		}

		public ColdDownTimer(){
			During = 0;
			TrackTime = Time.time;
		}

		public bool CanInvoke(){
			var currentTime = Time.time;
			return currentTime > TrackTime;
		}

		public void Reset(){
			var currentTime = Time.time;
			TrackTime = currentTime + During;
		}
	}
}