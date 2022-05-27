using System;
using Project;
using UnityEngine;
using UnityEngine.UI;

namespace HelloPico2{
	public class VFXPosterExample : MonoBehaviour{
		[SerializeField] private Button bindingVFX;
		[SerializeField] private Button nonBindingVFX;
		[SerializeField] private Transform bindingTransform;

		private void Start(){
			bindingVFX.onClick.AddListener(OnBindingVFX);
			nonBindingVFX.onClick.AddListener(OnNonBindingVFX);
		}

		private void OnBindingVFX(){
			EventBus.Post<VFXEventRequested, ParticleSystem>(new VFXEventRequested("A", true, bindingTransform));
		}

		private void OnNonBindingVFX(){
			EventBus.Post<VFXEventRequested, ParticleSystem>(new VFXEventRequested("B", false, 3f,
				bindingTransform.position + Vector3.right * 5f));
		}
	}
}