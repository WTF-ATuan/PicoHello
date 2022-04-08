using System;
using HelloPico2.Hand.Scripts.Event;
using Project;
using UnityEngine;
using UnityEngine.Events;

namespace HelloPico2.Menu.Interactable{
	public class MenuPresenter : MonoBehaviour{
		[SerializeField] private GameObject enterLevelTarget;

		[SerializeField] private UnityEvent enterLevelViewEvent;


		private void Start(){
			EventBus.Subscribe<HandSelected>(OnHandSelected);
		}

		private void OnHandSelected(HandSelected selected){
			var instanceID = selected.SelectedInstanceID;
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