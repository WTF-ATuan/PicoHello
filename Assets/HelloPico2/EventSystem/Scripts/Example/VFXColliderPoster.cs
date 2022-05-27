using Project;
using UnityEngine;

namespace HelloPico2{
	public class VFXColliderPoster : MonoBehaviour{
		private void OnTriggerEnter(Collider other){
			if(other.name == "A"){
				EventBus.Post<VFXEventRequested, ParticleSystem>(new VFXEventRequested("B", true, other.transform));
			}

			if(other.name == "B"){
				EventBus.Post<VFXEventRequested, ParticleSystem>(new VFXEventRequested("B", false, 3f,
					other.transform.position));
			}
		}

		private void OnCollisionEnter(Collision collision){
			if(collision.gameObject.name == "A"){
				EventBus.Post<VFXEventRequested, ParticleSystem>(new VFXEventRequested("B", true, collision.transform));
			}
		}
	}
}