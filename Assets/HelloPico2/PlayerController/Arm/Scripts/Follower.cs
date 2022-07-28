﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

public class Follower : MonoBehaviour{
	[SerializeField] private Transform m_Target;
	[SerializeField] private bool m_FollowXAxis = true;
	[SerializeField] private bool m_FollowZAxis = true;
	[SerializeField] private bool m_FollowYAxis = false;
	[SerializeField] private bool m_FollowYRot = false;

	public Vector3 m_AdditionalOffset;
	public bool m_HaveOffset = true;
	public bool m_AlwaysSync = false;
	public bool m_Dismont = false;
	public bool m_UseLerp = false;
	[ShowIf("m_UseLerp")] public float m_Duration = .05f;
	[ShowIf("m_UseLerp")] public AnimationCurve m_EasingCureve;
	public bool m_Sync = false;
	[ShowIf("m_Sync")] public bool m_Activation;
	[ShowIf("m_Sync")] public bool m_Rotation;
	[ShowIf("m_Sync")] public Vector3 m_AdditionalRotOffset;
	[ShowIf("m_Sync")] public bool m_Scale;

	public Transform Target{
		get{ return m_Target; }
		set{ m_Target = value; }
	}

	public Vector3 offset{ get; set; }
	public Vector3 PosPre{ get; set; }

	public void OnEnable(){
		if(m_HaveOffset && m_Target)
			offset = transform.position - m_Target.position;
		if(m_Dismont)
			transform.SetParent(transform.parent.root);
	}

	public void Update(){
		if(m_Target == null) return;

		FollowingParentPosition();
		SyncState();
	}

	private void FollowingParentPosition(){
		if(m_AlwaysSync || m_Target.position != PosPre){
			var moveXfer = transform.position;

			if(m_FollowXAxis)
				moveXfer.x = m_Target.position.x + offset.x;
			if(m_FollowZAxis)
				moveXfer.z = m_Target.position.z + offset.z;
			if(m_FollowYAxis)
				moveXfer.y = m_Target.position.y + offset.y;
			if(m_FollowYRot)
				transform.rotation =
						Quaternion.LookRotation(Vector3.ProjectOnPlane(m_Target.forward, Vector3.up).normalized,
							Vector3.up);

			if(!m_UseLerp)
				transform.position = moveXfer + m_AdditionalOffset;
			else
				transform.DOMove(moveXfer + m_AdditionalOffset, m_Duration).SetEase(m_EasingCureve);

			PosPre = transform.position;
		}
	}

	private void SyncState(){
		if(!m_Sync) return;

		if(m_Activation) gameObject.GetComponent<MeshRenderer>().enabled = m_Target.gameObject.activeSelf;
		if(m_Rotation){
			var followRotationOffset = m_Target.rotation * Quaternion.Euler(m_AdditionalRotOffset);
			transform.rotation = followRotationOffset;
		}

		if(m_Scale) transform.localScale = m_Target.localScale;
	}
}