using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

namespace HelloPico2.InputDevice.Scripts{
	public class InputDeviceTrigger : MonoBehaviour
	{
		private XRController _xrController;
		private UnityEngine.XR.InputDevice _inputDevice;
		[SerializeField] Transform[] ItemEffect;
		private int getType=0;//null 0 ball 1,acball 2
		private int rangeNum;

		private void Start()
		{
			_xrController = GetComponent<XRController>();
			_inputDevice = _xrController.inputDevice;

		}

		private void Update()
		{
			_inputDevice.TryGetFeatureValue(CommonUsages.trigger, out var triggerValue);
			_inputDevice.TryGetFeatureValue(CommonUsages.grip, out var gripValue);
			_inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out var touchPadAxis);
			_inputDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryButtonValue);
			_inputDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out bool secondaryButtonValue);

			
			if (triggerValue > 0.1f)
			{	
				OnTrigger(triggerValue);
			}

			if (gripValue > 0.1f)
			{
				OnGrip(gripValue);
			}

			if (touchPadAxis.magnitude > 0.1f)
			{
				OnTouchPad(touchPadAxis);
			}

			if (primaryButtonValue == true)
			{
				OnPrimayBtn(primaryButtonValue);
			}

			if (secondaryButtonValue == true)
			{
				OnSecondBtn(secondaryButtonValue);
			}
			
		}

		private void OnTrigger(float triggerValue)
		{
			ShowEffect();
		}

		private void OnGrip(float gripVale){ }

		private void OnTouchPad(Vector2 axis) { }

		private void OnPrimayBtn(bool primaryButtonValue)
		{
			ShowEffect();
		}
		private void OnSecondBtn(bool secondaryButtonValue)
		{
			ShowEffect();
		}
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "ball")
            {
				getType = 1;
			}
        }
        private void OnTriggerExit(Collider other)
        {
			getType = 0;
		}
		private void  ShowEffect()
        {
			if (getType == 0)
			{
				var particle = ItemEffect[0].GetComponent<ParticleSystem>();
				particle.Play();
			}
			else if (getType == 1)
			{
				var particle = ItemEffect[2].GetComponent<ParticleSystem>();
				particle.Play();
			}
		}
		
		}
		
	}