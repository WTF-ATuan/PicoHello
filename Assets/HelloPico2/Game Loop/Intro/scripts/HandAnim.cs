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

	public TargetItem_SO _getItme;
	public GameObject spawnedController;
	public int[] checkItemHeld;
	private bool showController = false;
	private bool isTip;
	public GameObject tipButtonAll;
	public GameObject[] tipButton;

	bool isGetGripTip;
	bool isGetTriggetpTip;
	
	int showArmTipCounter;
	public bool firstTouch;

	// Start is called before the first frame update
	private void Start()
    {
        controllerInteractor = _controller.GetComponent<Interactor>();
		_handAnimator = _handAnimator.GetComponent<Animator>();
		isTip = true;


	}
	private void staffCheck()
    {
		spawnedController.SetActive(true); //show firstAnim
		showController = true;
	}
	private void showAnim()
    {
		_handAnimator.SetTrigger("isGet");
	}
    // Update is called once per frame
    void Update()
    {
        if (isTip)
        {
			if (_getItme.targetItemHeld == checkItemHeld[0])
			{
				staffCheck();
			}
			else if((_getItme.targetItemHeld == checkItemHeld[1]))
            {
				showAnim();
			}
		}
		
		if (!showController) return;
        {
			_controller.inputDevice.TryGetFeatureValue(CommonUsages.triggerButton, out var isTrigger);
			_controller.inputDevice.TryGetFeatureValue(CommonUsages.gripButton, out var isGrip);
			_controller.inputDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggetValue);
			_controller.inputDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue);
			_controller.inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxisTouch, out var padAxisTouch);
			_controller.inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out var padAxisClick);
			_controller.inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 primary2DAxisValue);
			_controller.inputDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out bool secondaryButtonValue);
			_controller.inputDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryButtonValue);

			/* find right left Tip
			if (isTrigger && isGetGripTip && !isGetTriggetpTip)
			{
				if(_getItme.isTipL || _getItme.isTipR)
                {
					TipGet(1);
				}
				_getItme.targetItemHeld = 2;
			}

			if (isGrip && !isGetGripTip &&_getItme.isTipL && _getItme.isTipR  && gameObject.transform.GetChild(4).transform.localScale.x != 0)
			{

				if (_controller.name == "LeftHand Controller")
				{
					_getItme.isTipR = false;
				}
				else
				{
					_getItme.isTipL = false;
				}
				_getItme.targetItemHeld = 1;
				TipGet(0);
				isGetGripTip = true;
				tipButton[0].transform.GetChild(2).gameObject.SetActive(false);//playbadAudioClose
			}
		
			if (!isGrip && isGetGripTip && gameObject.transform.GetChild(4).transform.localScale.x == 0 && _getItme.targetItemHeld ==1)
            {		
				_getItme.isTipL = true;
				_getItme.isTipR = true;
				tipButton[0].transform.GetChild(2).gameObject.SetActive(true);//playNotGetAudio
				tipButton[0].SetActive(true);
				tipButton[1].SetActive(false);
				isGetGripTip = false;
				_getGuideSys.guidesType = 3;
				_getItme.targetItemHeld = 0;

				showArmTipCounter += 1;
                if (showArmTipCounter > 2)
                {
					_getItme.isTipArm = true;
				}

		} //¤¤Â_ÂI
			//
			if (isTrigger)//Tip Trigger
            {
				tipButton[1].SetActive(true);
			}
            else
            {
				tipButton[1].SetActive(false);
			}
            if (isGrip)
            {
				tipButton[0].SetActive(true);
			}
            else
            {
				tipButton[0].SetActive(false);
			}
			*/
			if (triggetValue >= 0)
            {
				_handAnimator.SetFloat("Trigger", triggetValue);
			}
			if (gripValue >= 0)
			{
				_handAnimator.SetFloat("Grip", gripValue);
			}
			//

			/*
			if (!padAxisTouch)
			{

			}
			if (padAxisClick)
			{

			}*/
			if (primary2DAxisValue.magnitude >= 0)
			{
				_handAnimator.SetFloat("yAxis", primary2DAxisValue.y);
				_handAnimator.SetFloat("xAxis", primary2DAxisValue.x);
			}

			if (secondaryButtonValue)
			{
				_handAnimator.SetBool("SecondBtn", secondaryButtonValue);
				//tipButton[3].SetActive(true);
			}
			else
			{
				_handAnimator.SetBool("SecondBtn", false);
				//tipButton[3].SetActive(false);
			}
            if (primaryButtonValue)
            {
				_handAnimator.SetBool("PrimaryBtn", primaryButtonValue);
				//tipButton[2].SetActive(true);
			}
            else
            {
				_handAnimator.SetBool("PrimaryBtn", false);
				//tipButton[2].SetActive(false);
			}
		}
		//ShowTip();
		
	}
	public void getfirst()
    {
		firstTouch = true;
	}
	public void touchFalse()
	{
		firstTouch = false;
	}

	void ShowTip()
    {
        if (_getItme.isTipOpen)
        {
			if (_getItme.targetItemHeld == 0 && _getItme.isTipR && _getItme.isTipL)
			{
				tipButtonAll.SetActive(true);
			}
			if (_getItme.targetItemHeld == 1 && !_getItme.isTipR && !isGetGripTip && !isGetTriggetpTip)
			{
				tipButtonAll.SetActive(false);
			}
			if (_getItme.targetItemHeld == 1 && !_getItme.isTipL && !isGetGripTip && !isGetTriggetpTip)
			{
				tipButtonAll.SetActive(false);
			}
		}
        else
        {
			tipButtonAll.SetActive(false);
		}
		

	}
	void TipGet(int num)
    {

		tipButton[num].transform.GetChild(0).GetComponent<Animator>().SetBool("isTip", true);//Anima
		tipButton[num].transform.GetChild(1).gameObject.SetActive(true); //Audio Get

		_getItme.isTipArm = false;
		

		StartCoroutine(waitClose(num));
		StopCoroutine(waitClose(0));

		
		//_getItme.isTipOpen = false;


		
	}
	void TipClose()
    {
		tipButtonAll.SetActive(false);
		_getItme.isTipL = false;
		_getItme.isTipR = false;
		isGetTriggetpTip = false;
		_getItme.targetItemHeld = 0;
		_getItme.isTipOpen = false;
		
	}
	IEnumerator waitClose(int num)
    {
		yield return new WaitForSeconds(1);

		tipButton[num].transform.GetChild(0).GetComponent<Animator>().SetBool("isTip", false);//Anima
		tipButton[num].transform.GetChild(1).gameObject.SetActive(false); //Audio close
		
		if(num + 1 < tipButton.Length && tipButton[num + 1] != null)//show Next Tip
        {
			tipButton[num].SetActive(false);
			tipButton[num + 1].SetActive(true);
		}
		if (_getItme.targetItemHeld == 2)
		{
			TipClose();
		}
		yield return null;
	}

}
