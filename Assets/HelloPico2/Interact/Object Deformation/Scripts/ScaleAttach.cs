using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace HelloPico2.Interact.Object_Deformation.Scripts{
	public class ScaleAttach : MonoBehaviour{
		private XRBaseInteractable _interactable;

		private void Start(){
			_interactable = GetComponent<XRBaseInteractable>();
			_interactable.selectEntered.AddListener(OnSelectEnter);
			_interactable.selectExited.AddListener(OnSelectExited);
		}

		private void OnSelectEnter(SelectEnterEventArgs arg0){
			transform.localScale *= 2;
		}

		private void OnSelectExited(SelectExitEventArgs arg0){
			transform.localScale /= 2;
		}
	}
}