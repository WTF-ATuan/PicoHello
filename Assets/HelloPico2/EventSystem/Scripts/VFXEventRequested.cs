﻿using System;
using UnityEngine;

namespace HelloPico2{
	public class VFXEventRequested{
		public string VfxID{ get; } //ID
		public bool IsBinding{ get; } //是否要綁定
		public float During{ get; } //持續時間
		public Transform AttachPoint{ get; } //綁定狀態的情境下的父物件
		public Vector3 SpawnPosition{ get; } //無綁定狀態下的生成位置

		public VFXEventRequested(string vfxID, bool isBinding, Transform attachPoint,
			float during = 0,
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