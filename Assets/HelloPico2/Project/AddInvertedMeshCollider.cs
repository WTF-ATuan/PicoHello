using System.Linq;
using UnityEngine;

namespace Project{
	public class AddInvertedMeshCollider : MonoBehaviour{
		public bool removeExistingColliders = true;

		public void CreateInvertedMeshCollider(){
			if(removeExistingColliders)
				RemoveExistingColliders();

			InvertMesh();

			gameObject.AddComponent<MeshCollider>();
		}

		private void RemoveExistingColliders(){
			var colliders = GetComponents<Collider>();
			for(int i = 0; i < colliders.Length; i++)
				DestroyImmediate(colliders[i]);
		}

		private void InvertMesh(){
			var mesh = GetComponent<MeshFilter>().mesh;
			mesh.triangles = mesh.triangles.Reverse().ToArray();
		}
	}
}