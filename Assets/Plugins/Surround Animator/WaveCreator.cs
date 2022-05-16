using UnityEngine;

namespace HelloPico2.Interact.SpineCreator{
	public class WaveCreator{
		public float time;
		public float timeSpeed = 1f;

		public WaveCreator(){
			time = Random.Range(-10f, 10f);
		}

		public float GetValue(){
			#if UNITY_EDITOR
                if (!Application.isPlaying) if (Time.unscaledDeltaTime == 0f) time += 0.015625f * timeSpeed;
			#endif
			time += Time.unscaledDeltaTime * timeSpeed;
			return Mathf.Sin(time);
		}
	}
}