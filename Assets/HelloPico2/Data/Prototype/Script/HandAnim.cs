using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using HelloPico2.InteractableObjects;
using HelloPico2.InputDevice.Scripts;

public class HandAnim : MonoBehaviour
{
    public XRController _controller;
    public Interactor controllerInteractor { get; set; }
	public Animator _handAnimator;

	public TargetItem_SO _getStaff;
	public GameObject spawnedController;
	public bool showController = false;
	// Start is called before the first frame update
	private void Start()
    {
        controllerInteractor = _controller.GetComponent<Interactor>();
		_handAnimator = _handAnimator.GetComponent<Animator>();
	}
	void staffCheck()
    {
		showController = true;
		spawnedController.SetActive(true);
	}
    // Update is called once per frame
    void Update()
    {
		if(_getStaff.targetItemHeld==2)
        {
			staffCheck();
		}

		if (showController)
        {
			_controller.inputDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggetValue);
			_controller.inputDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue);
			_controller.inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxisTouch, out var padAxisTouch);
			_controller.inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out var padAxisClick);
			_controller.inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 primary2DAxisValue);
			_controller.inputDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out bool secondaryButtonValue);
			if (triggetValue >= 0)
			{
				_handAnimator.SetFloat("Trigger", triggetValue);
			}
			if (gripValue >= 0)
			{
				_handAnimator.SetFloat("Grip", gripValue);
			}
			if (!padAxisTouch)
			{

			}
			if (padAxisClick)
			{

			}
			if (primary2DAxisValue.magnitude >= 0)
			{
				_handAnimator.SetFloat("yAxis", primary2DAxisValue.y);
				_handAnimator.SetFloat("xAxis", primary2DAxisValue.x);
			}
			if (secondaryButtonValue)
			{
				_handAnimator.SetBool("SecondBtn", secondaryButtonValue);
			}
			else
			{
				_handAnimator.SetBool("SecondBtn", false);
			}
		}

	}
}
