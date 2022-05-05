using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace HelloPico2.InteractableObjects
{
    public class InteractableSword : InteractableBase
    {
		[Header("Sword")]
		public GameObject _SwordMesh;
		[SerializeField] private float _TransformDuration;
		[SerializeField] private float _SwordLength;
		[SerializeField] private AnimationCurve _TransformMovement;
		private bool _Active;
		
		public UnityEngine.Events.UnityEvent WhenActive;
		public UnityEngine.Events.UnityEvent WhenDeactive;

        public override void OnTouchPad(Vector2 axis)
        {
            base.OnTouchPad(axis);
			
			var axisX = axis.x;
			var axisY = axis.y;
			if (axisX > 0)
			{
				ActivateSword();	
			}
			else
			{

			}

			if (axisY > 0)
			{
				ActivateSword();
			}
			else
			{

			}
		}
		public override void OnDrop()
        {
            base.OnDrop();

			DeactivateSword();
        }
        private void ActivateSword() {
			if (_Active) return;
			
			_Active = true;
			
			WhenActive?.Invoke();

			_SwordMesh.transform.DOScaleZ(_SwordLength, _TransformDuration).SetEase(_TransformMovement);
		}
		private void DeactivateSword()
		{
			if (!_Active) return;

			_Active = false;			

			_SwordMesh.transform.DOScaleZ(0, _TransformDuration).SetEase(_TransformMovement).OnComplete(() => WhenDeactive?.Invoke());
		}		
	}
}
