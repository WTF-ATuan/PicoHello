//Author：AboutVFX
//Email:54315031@qq.com
//QQ Group：156875373

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(SkinnedMeshRenderer))]
public class AboutVFXGhostTrail : MonoBehaviour {

	public bool openGhostTrail = false;
	public Material ghostMaterial;
	[Range(0.05f, 10f)]
	public float intervalTime = 0.2f;
	[Range(0.1f, 60f)]
	public float lifeTime = 0.5f;
	[Range(0.1f, 60f)]
	public float fadeTime = 0.3f;

	private float updateTime = 0f;
	private List<AboutVFXGhostItem> ghostList = new List<AboutVFXGhostItem>(); 
	private SkinnedMeshRenderer skinnedMesh;


	void Start () {
		skinnedMesh = GetComponent<SkinnedMeshRenderer>();
	}
	
	void LateUpdate () {
		if (openGhostTrail == true) {
			updateTime += Time.deltaTime;
			if (updateTime >= intervalTime) {
				updateTime = 0f;
				CreateGhostItem ();
			}
		} 

		if (ghostList.Count > 0) {
			FadeGhostItem ();
			DrawGhostItem ();
		}
	}

	void CreateGhostItem(){
		Mesh GhostMesh = new Mesh ();
		skinnedMesh.BakeMesh (GhostMesh);
		ghostList.Add (new AboutVFXGhostItem (GhostMesh, transform.localToWorldMatrix, new Material(ghostMaterial)));
		}

	void FadeGhostItem(){
		for (int i = (ghostList.Count-1);i>=0;i--) {
			if (ghostList[i].Age > lifeTime) {
				ghostList[i].FadeLerp += (1 / fadeTime) * Time.deltaTime;
				ghostList[i].Alpha = Mathf.Lerp (1f, 0f, ghostList [i].FadeLerp);
				if (ghostList [i].FadeLerp > 1) {
					Destroy (ghostList [i].Mesh);
					Destroy (ghostList [i].Mat);
					ghostList.RemoveAt (i);
				} else {
					ghostList[i].Mat.SetFloat("_Opacity",ghostList[i].Alpha);
				}
			} 
		}
	}

	void DrawGhostItem(){
		foreach (AboutVFXGhostItem item in ghostList) {
			item.Age+=Time.deltaTime;
			Graphics.DrawMesh(item.Mesh, item.Matrix, item.Mat, gameObject.layer);
		}
	}
}