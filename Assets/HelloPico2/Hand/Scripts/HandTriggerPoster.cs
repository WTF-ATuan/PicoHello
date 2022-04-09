using System;
using UnityEngine;

namespace HelloPico2.Hand.Scripts{
	[RequireComponent(typeof(Hand))]
	//TODO need refactor event poster;
	public class HandTriggerPoster : MonoBehaviour{
		private Hand _hand;

		private void Start(){
			_hand = GetComponent<Hand>();
		}

		//TODO need refactor 
		private void OnTriggerEnter(Collider other){
			if(other.tag.Equals("TriggerPoint")){
				var particle = _hand.palmTransform.GetComponent<ParticleSystem>();
				particle.Play();
			}
		}

		private void OnTriggerExit(Collider other){
			if(other.tag.Equals("TriggerPoint")){
				var particle = _hand.palmTransform.GetComponent<ParticleSystem>();
				particle.Stop();
			}
		}
	}
}