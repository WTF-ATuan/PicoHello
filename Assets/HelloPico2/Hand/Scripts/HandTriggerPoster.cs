using System;
using System.Collections;
using UnityEngine;

namespace HelloPico2.Hand.Scripts{
	[RequireComponent(typeof(Hand))]
	//TODO need refactor event poster;
	public class HandTriggerPoster : MonoBehaviour{
		private Hand _hand;

		//Todo roughTest
		private Coroutine _createCoroutine;

		private void Start(){
			_hand = GetComponent<Hand>();
		}

		//TODO need refactor 
		private void OnTriggerEnter(Collider other){
			if(other.tag.Equals("TriggerPoint")){
				var particle = _hand.palmTransform.GetComponent<ParticleSystem>();
				particle.Play();
				_createCoroutine = StartCoroutine(DelayCreating(2f));
			}
		}

		private void OnTriggerExit(Collider other){
			if(other.tag.Equals("TriggerPoint")){
				var particle = _hand.palmTransform.GetComponent<ParticleSystem>();
				particle.Stop();
				if(_createCoroutine != null){
					StopCoroutine(_createCoroutine);
				}
			}
		}

		private IEnumerator DelayCreating(float delayTime){
			yield return new WaitForSeconds(delayTime);
			var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			sphere.transform.position = _hand.palmPoint;
			sphere.transform.localScale = Vector3.one * 0.2f;
			sphere.GetComponent<Collider>().enabled = false;
		}
	}
}