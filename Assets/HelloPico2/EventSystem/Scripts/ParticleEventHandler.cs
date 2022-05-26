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
			var particleData = dataOverview.FindEventData<ParticleData>(vfxID);
			if(isBinding){
				var particle = particleData.particle;
				var particleObject = Instantiate(particle, attachPoint.position, Quaternion.identity, attachPoint);
				particleObject.name = $"VFX{vfxID}";
				return particleObject;
			}
			else{
				var particle = particleData.particle;
				var particleObject = Instantiate(particle, spawnPosition, Quaternion.identity);
				particleObject.name = $"VFX{vfxID} (Temporary)";
				Destroy(particleObject.gameObject, during);
				return null;
			}
		}
	}
}