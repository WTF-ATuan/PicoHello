using System;
using System.Collections.Generic;
using UnityEngine;

namespace HelloPico2.InteractableObjects{
	public interface ISpawner{
		public Action<GameObject> OnSpawn{ get; set; }
		public List<Vector3> SpawnPoint{ get;  }
	}
}