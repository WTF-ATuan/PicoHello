using UnityEngine;
using UnityEngine.Events;

namespace HelloPico2.Menu.Interactable{
	public class MenuPresenter : MonoBehaviour{
		[SerializeField] private GameObject enterLevelTarget;

		[SerializeField] private UnityEvent enterLevelViewEvent;


		private void OnHandSelected(GameObject selectObject){
			var instanceID = selectObject.gameObject.GetInstanceID();
			var isEquals = enterLevelTarget.GetInstanceID().Equals(instanceID);
			if(isEquals){
				EnterLevel();
			}
		}

		private void EnterLevel(){
			enterLevelViewEvent?.Invoke();
		}
	}
}