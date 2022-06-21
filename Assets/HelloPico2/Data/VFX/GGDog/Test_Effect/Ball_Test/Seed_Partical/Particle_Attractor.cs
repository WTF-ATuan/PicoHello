using System.Collections;
using UnityEditor;
using UnityEngine;
[RequireComponent(typeof(ParticleSystem))]
public class Particle_Attractor : MonoBehaviour {
	ParticleSystem ps;
	ParticleSystem.Particle[] m_Particles;
	public Transform target;
    public float RemainLifeTime = 1;
    public float speed = 5f;
    public Material TrailMaterial;

    public AnimationCurve SpeedCurve;


    int numParticlesAlive;

    Particle_Attractor _Particle_Attractor;

    ParticleSystemRenderer render;
    ParticleSystem.MainModule main;
    ParticleSystem.NoiseModule noise;
    ParticleSystem.VelocityOverLifetimeModule velocityOverLifetime;
    ParticleSystem.TrailModule trails;
    void Awake ()
    {
		ps = GetComponent<ParticleSystem>();
		if (!GetComponent<Transform>()){
			GetComponent<Transform>();
		}
        _Particle_Attractor = GetComponent<Particle_Attractor>();


        main = ps.main;
        noise = ps.noise;
        velocityOverLifetime = ps.velocityOverLifetime;
        trails = ps.trails;

        render = GetComponent<ParticleSystemRenderer>();
    }
    
    void OnEnable()
    {
            main.simulationSpace = ParticleSystemSimulationSpace.World;

            main.startLifetime = RemainLifeTime;

            m_Particles = new ParticleSystem.Particle[ps.main.maxParticles];
            numParticlesAlive = ps.GetParticles(m_Particles);
            for (int i = 0; i < numParticlesAlive; i++)
            {
                m_Particles[i].remainingLifetime = RemainLifeTime;
            }
            ps.SetParticles(m_Particles, numParticlesAlive);

            noise.enabled = false;

            velocityOverLifetime.enabled = true;
            
            trails.enabled = true;

            render.trailMaterial = TrailMaterial;
            
            SpeedCurve_Time = 0;

            ps.Simulate(0);
            ps.Play();
    }
    

    void PS_Status_Reset()
    {
        main.simulationSpace = ParticleSystemSimulationSpace.Local;

        main.startLifetime = 999;
        
        noise.enabled = true;
        
        velocityOverLifetime.enabled = false;
        
        trails.enabled = false;

        _Particle_Attractor.enabled = false;
    }



    float SpeedCurve_Time = 0;
    void Update () {
		m_Particles = new ParticleSystem.Particle[ps.main.maxParticles];
		numParticlesAlive = ps.GetParticles(m_Particles);
        
        for (int i = 0; i < numParticlesAlive; i++)
        {
			m_Particles[i].position = Vector3.Lerp(m_Particles[i].position, target.position, speed* SpeedCurve.Evaluate(SpeedCurve_Time) / 10);
        }
		ps.SetParticles(m_Particles, numParticlesAlive);

        SpeedCurve_Time += Time.deltaTime;

        //粒子系統狀態重置
        if(SpeedCurve_Time> RemainLifeTime)
        {
            PS_Status_Reset();
        }

    }
}
