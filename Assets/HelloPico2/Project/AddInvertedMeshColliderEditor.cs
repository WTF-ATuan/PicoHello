#if UNITY_EDITOR
using Project;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AddInvertedMeshCollider))]
public class AddInvertedMeshColliderEditor : Editor{
	public override void OnInspectorGUI(){
		DrawDefaultInspector();
		AddInvertedMeshCollider script = (AddInvertedMeshCollider)target;
		if(GUILayout.Button("Create Inverted Mesh Collider"))
			script.CreateInvertedMeshCollider();
	}
}
#endif