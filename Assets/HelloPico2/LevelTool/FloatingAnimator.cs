using System;
using DG.Tweening;
using UnityEngine;

namespace HelloPico2.LevelTool{
	public class FloatingAnimator : MonoBehaviour{
		public float offsetY = 2;
		public float duration = 1.5f;
		public bool reset = true;

		private void OnEnable(){
			var targetOffsetY = transform.position.y + offsetY;
			transform.DOMoveY(targetOffsetY, duration)
					.SetLoops(-1, LoopType.Yoyo);
		}

		private void OnDisable(){
			if(reset){
				transform.DORestart();
			}

			transform.DOKill();
		}
	}
}