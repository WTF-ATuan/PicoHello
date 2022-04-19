using UnityEngine;

namespace HelloPico2.Interact.Grip_Gun{
	public class PowerCarrier : MonoBehaviour{
		[SerializeField] private int powerAmount;
		public bool isEmpty => powerAmount <= 0;
		public int currentPowerAmount => powerAmount;

		public void ModifyPower(int amount){
			powerAmount += amount;
		}
	}
}