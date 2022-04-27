using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using Project;
using HelloPico2.Hand.Scripts.Event;

public class HandAnim : MonoBehaviour, ITrigger, ITouchPad, IPrimaryButton, ISecondaryButton
{
	private XRBaseInteractable _interactable;
	 
	[SerializeField] private Animator handAnimator;


	private bool _isSelect;
	public void Start()
	{
		_interactable = GetComponent<XRBaseInteractable>();
	
		_interactable.selectEntered.AddListener(x => { _isSelect = true; });
		_interactable.selectExited.AddListener(x => { _isSelect = false; });
		
	}
    
    public void OnTrigger(float triggerValue)
	{
		Debug.Log(handAnimator);
		handAnimator.SetFloat("Trigger", triggerValue);
	}

	public void OnTouchPad(Vector2 padAxis)
	{
		Debug.Log(handAnimator);
	}

	public void OnPrimaryButtonClick()
	{
		Debug.Log(handAnimator);
		handAnimator.SetBool("PrimaryBtn",true);
	}

	public void OnSecondaryButtonClick()
	{
		Debug.Log(handAnimator);
		handAnimator.SetBool("SecondBtn", true);
	}


}
