using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace HelloPico2.InteractableObjects
{
    public class FollowParticle : MonoBehaviour
    {
        public bool Activate;
        public GameObject[] m_Follower;
        public ParticleSystem m_FollowThis;
        public bool m_DeactivateAfterParticleDie = true;
        [ShowIf("m_DeactivateAfterParticleDie")] public float m_DelayDeactiveTime = .5f;
        ParticleSystem.Particle[] m_Particles;
        private void Awake()
        {
            for (int i = 0; i < m_Follower.Length; i++)
            {
                m_Follower[i].SetActive(false);
            }
        }
        public void Update()
        {
            if (!Activate) return;

            m_Particles = new ParticleSystem.Particle[m_FollowThis.main.maxParticles];
            int particlesAliveAmount = m_FollowThis.GetParticles(m_Particles);

            for (int i = 0; i < particlesAliveAmount; i++)
            {
                if (i < m_Follower.Length)
                {
                    if (m_Follower[i] == null) continue;                    

                    m_Follower[i].transform.position = m_Particles[i].position + m_FollowThis.transform.position;
                    m_Follower[i].transform.forward = m_Particles[i].animatedVelocity.normalized;

                    if (m_Particles[i].remainingLifetime > 0.1f)
                    {
                        m_Follower[i].SetActive(true);
                    }
                    else
                    {
                        if(m_DeactivateAfterParticleDie)
                            StartCoroutine(Delayer(m_Follower[i]));
                    }

                }
            }
        }
        private IEnumerator Delayer(GameObject follower)
        {
            yield return new WaitForSeconds(m_DelayDeactiveTime);
            follower.SetActive(false);
        }
    }
}
