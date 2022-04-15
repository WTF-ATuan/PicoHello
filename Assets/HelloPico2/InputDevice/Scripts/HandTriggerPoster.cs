using System.Collections;
using UnityEngine;

namespace HelloPico2.InputDevice.Scripts{
	[RequireComponent(typeof(Hand))]
	//TODO need refactor event poster;
	public class HandTriggerPoster : MonoBehaviour{
		private Hand hand;

		public Transform[] InsElement;//Jeremy create Ins
		int insLength;
		//Todo roughTest
		private Coroutine createCoroutine;

		private void Start(){
			hand = GetComponent<Hand>();
			insLength = InsElement.Length;
		}

		//TODO need refactor 
		private void OnTriggerEnter(Collider other){
			if(other.tag.Equals("TriggerPoint")){
				var particle = hand.palmTransform.GetComponent<ParticleSystem>();
				particle.Play();
				createCoroutine = StartCoroutine(DelayCreating(2f));
			}
		}

		private void OnTriggerExit(Collider other){
			if(other.tag.Equals("TriggerPoint")){
				var particle = hand.palmTransform.GetComponent<ParticleSystem>();
				particle.Stop();
				if(createCoroutine != null){
					StopCoroutine(createCoroutine);
				}
			}
		}

		private IEnumerator DelayCreating(float delayTime){
			yield return new WaitForSeconds(delayTime);
			/*var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			sphere.transform.position = hand.palmPoint;
			sphere.transform.localScale = Vector3.one * 0.2f;
			sphere.GetComponent<Collider>().enabled = false;*/

			Vector3 insElepos = new Vector3(hand.palmPoint.x, hand.palmPoint.y +0.5f, hand.palmPoint.z);
			Instantiate(InsElement[Random.Range(0, insLength)], insElepos, Quaternion.identity);
			
			
		}
	}
}