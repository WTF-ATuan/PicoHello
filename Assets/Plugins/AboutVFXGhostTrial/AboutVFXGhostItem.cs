//Author：AboutVFX
//Email:54315031@qq.com
//QQ Group：156875373
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AboutVFXGhostItem
{
	public Mesh Mesh;
	public Matrix4x4 Matrix;
	public Material Mat;
	public float Age = 0f;
	public float FadeLerp = 0f;
	public float Alpha =1.0f;

	public AboutVFXGhostItem(Mesh _Mesh,Matrix4x4 _Matrix,Material _Mat)
	{
		Mesh = _Mesh;
		Matrix = _Matrix;
		Mat = _Mat;
	}
}