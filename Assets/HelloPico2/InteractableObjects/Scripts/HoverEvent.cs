using System;
using DG.Tweening;
using Game.Project;
using Sirenix.OdinInspector;
using UltEvents;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace HelloPico2.InteractableObjects.Scripts{
	[RequireComponent(typeof(XRBaseInteractable))]
	public class HoverEvent : MonoBehaviour{
		private XRBaseInteractable _interactable;

		[ReadOnly] public bool isHover;
		public float during = 2f;

		private float _timer;

		public UltEvent<float> onHovering;
		public UltEvent onHoverTrigger;

		private HoverEnterEventArgs _hoverEnterEventArgs;

		private void Start(){
			_interactable = GetComponent<XRBaseInteractable>();
			_interactable.hoverEntered.AddListener(x => {
				isHover = true;
				_hoverEnterEventArgs = x;
			});
			_interactable.hoverExited.AddListener(x => isHover = false);
		}

		private void Update(){
			if(!isHover){
				_timer = 0;
				return;
			}

			_timer += Time.fixedDeltaTime;
			onHovering.Invoke(Mathf.Clamp(_timer, 0, 1));
			if(_timer >= during){
				var interactor = _hoverEnterEventArgs.interactorObject;
				var interactorPosition = interactor.transform.position;
				transform.DOMove(interactorPosition, 1f)
						.OnComplete(() => onHoverTrigger.Invoke());
				_timer = 0;
			}
		}
	}
}