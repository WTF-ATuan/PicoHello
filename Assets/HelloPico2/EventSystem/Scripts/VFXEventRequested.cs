using System;
using UnityEngine;

namespace HelloPico2{
	public class VFXEventRequested{
		public string VfxID{ get; }
		public bool IsBinding{ get; }
		public float During{ get; }
		public Transform AttachPoint{ get; }
		public Vector3 SpawnPosition{ get; }

		public VFXEventRequested(string vfxID, bool isBinding, float during,
			Transform attachPoint = null,
			Vector3 spawnPosition = default){
			VfxID = vfxID;
			IsBinding = isBinding;
			During = during;
			AttachPoint = attachPoint;
			SpawnPosition = spawnPosition;
			CheckData();
		}

		public VFXEventRequested(string vfxID, bool isBinding, float during,
			Vector3 spawnPosition = default,
			Transform attachPoint = null){
			VfxID = vfxID;
			IsBinding = isBinding;
			During = during;
			AttachPoint = attachPoint;
			SpawnPosition = spawnPosition;
			CheckData();
		}

		private void CheckData(){
			if(!AttachPoint && SpawnPosition == default){
				throw new NullReferenceException("Position and Transform is null");
			}

			if(IsBinding){
				if(!AttachPoint) throw new Exception("is Binding State need attachPoint(Transform)");
			}
			else{
				if(SpawnPosition == default){
					throw new Exception("is non Binding State need SpawnPosition(Vector3)");
				}
			}
		}
	}
}