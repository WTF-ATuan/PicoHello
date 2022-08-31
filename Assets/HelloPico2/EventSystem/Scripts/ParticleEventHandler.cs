using Project;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HelloPico2{
	public class ParticleEventHandler : MonoBehaviour{
		[Required] [SerializeField] private ViewEventDataOverview dataOverview;

		private void Start(){
			EventBus.Subscribe<VFXEventRequested, ParticleSystem>(OnVFXEventRequested);
		}

		private ParticleSystem OnVFXEventRequested(VFXEventRequested obj){
			var vfxID = obj.VfxID;
			var spawnPosition = obj.SpawnPosition;
			var attachPoint = obj.AttachPoint;
			var during = obj.During;
			var isBinding = obj.IsBinding;
			var usingMultipleVfXs = obj.UsingMultipleVfXs;
			ParticleSystem particle;
			if(usingMultipleVfXs){
				var vfxData = dataOverview.FindEventData<MultiVFXData>(vfxID);
				var particleSystem = vfxData.GetParticle();
				particle = particleSystem;
			}
			else{
				var particleData = dataOverview.FindEventData<VFXData>(vfxID);
				particle = particleData.particle;
			}

			if(isBinding){
				var particleObject = Instantiate(particle, attachPoint.position, Quaternion.identity, attachPoint);
				particleObject.name = $"VFX{vfxID}";
				return particleObject;
			}
			else{
				var particleObject = Instantiate(particle, spawnPosition, Quaternion.identity);
				particleObject.name = $"VFX{vfxID} (Temporary)";
				Destroy(particleObject.gameObject, during);
				return null;
			}
		}
	}
}