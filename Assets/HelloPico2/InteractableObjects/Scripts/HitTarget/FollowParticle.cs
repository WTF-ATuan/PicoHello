using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace HelloPico2.InteractableObjects{
	public class FollowParticle : MonoBehaviour{
		public bool Activate;
		public bool UseWorldPosition = false;
		public bool SpawnFollowerWhenBirth = false;
		[ShowIf("SpawnFollowerWhenBirth")] public int SpawnAmount = 10;
		public bool DisableFollowerOnAwake = true;
		public GameObject[] m_Follower;
		public ParticleSystem m_FollowThis;
		public bool m_DeactivateAfterParticleDie = true;

		[ShowIf("m_DeactivateAfterParticleDie")]
		public float m_DelayDeactiveTime = .5f;

		ParticleSystem.Particle[] m_Particles;
		public System.Action<GameObject> WhenParticleDies;

		public void ActivateFollowParticle(){
			Activate = true;
		}

		private void OnValidate(){
			if(m_Follower.Length != 0){
				for(int i = 0; i < m_Follower.Length; i++){
					if(m_Follower[i] == gameObject)
						throw new System.Exception("Can't assign self to m_Follower.");
				}
			}
		}

		private void Awake(){
			for(int i = 0; i < m_Follower.Length; i++){
				m_Follower[i].SetActive(!DisableFollowerOnAwake);
			}
		}

		private void OnEnable(){
			if(SpawnFollowerWhenBirth){
				List<GameObject> cloneList = new List<GameObject>();

				var obj = m_Follower[0];
				cloneList.Add(obj);

				for(int i = 0; i < SpawnAmount; i++){
					var clone = Instantiate(obj);
					cloneList.Add(clone);
					clone.transform.SetParent(transform);
					clone.SetActive(!DisableFollowerOnAwake);
				}

				m_Follower = cloneList.ToArray();
			}
		}

		public void FixedUpdate(){
			if(!Activate) return;

			m_Particles = new ParticleSystem.Particle[m_FollowThis.main.maxParticles];
			int particlesAliveAmount = m_FollowThis.GetParticles(m_Particles);

			for(int i = 0; i < particlesAliveAmount; i++){
				if(i < m_Follower.Length){
					if(m_Follower[i] == null) continue;

					if(UseWorldPosition)
						m_Follower[i].transform.position = m_Particles[i].position;
					else
						m_Follower[i].transform.position = m_Particles[i].position + m_FollowThis.transform.position;

					m_Follower[i].transform.forward = m_Particles[i].animatedVelocity.normalized;

					if(m_Particles[i].remainingLifetime > 0.3f){
						m_Follower[i].SetActive(true);
					}
					else{
						WhenParticleDies?.Invoke(m_Follower[i]);

						if(m_DeactivateAfterParticleDie)
							StartCoroutine(Delayer(m_Follower[i]));

						m_Follower[i].SetActive(false);
						m_Follower[i].transform.position = m_FollowThis.transform.position;
					}
				}
			}

			m_FollowThis.SetParticles(m_Particles, particlesAliveAmount);
		}

		private IEnumerator Delayer(GameObject follower){
			yield return new WaitForSeconds(m_DelayDeactiveTime);
			follower.SetActive(false);
			follower.transform.position = m_FollowThis.transform.position;
		}
	}
}