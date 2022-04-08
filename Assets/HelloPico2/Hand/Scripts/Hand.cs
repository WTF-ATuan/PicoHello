using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;

public class Hand : MonoBehaviour{
	[SerializeField] private InputDeviceCharacteristics characteristics;

	private InputDevice _currentDevice;

	private void Start(){
		var inputDevices = new List<InputDevice>();
		InputDevices.GetDevicesWithCharacteristics(characteristics, inputDevices);
		_currentDevice = inputDevices.First();
	}
}