using System;
using UnityEngine;

namespace HelloPico2.Interact.Grip_Gun{
	public class PowerCarrier : MonoBehaviour{
		[SerializeField] private int powerAmount;
		public bool isEmpty => powerAmount <= 0;
		public int currentPowerAmount => powerAmount;
		public int maxPowerAmount{ get; private set; }

		public void ModifyPower(int amount){
			powerAmount += amount;
		}

		private void Start(){
			maxPowerAmount = powerAmount;
		}
	}
}