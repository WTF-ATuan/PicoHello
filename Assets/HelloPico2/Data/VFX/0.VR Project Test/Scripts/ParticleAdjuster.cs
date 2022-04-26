using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class ParticleAdjuster : MonoBehaviour{
	private List<ParticleSystem> _particleSystems;

	private void Start(){
		_particleSystems = GetComponentsInChildren<ParticleSystem>().ToList();
		_particleSystems.RemoveAt(0);
	}

	[Button]
	public void ModifyVelocity(float velocity){
		foreach(var particle in _particleSystems){
			var mainModule = particle.main;
			mainModule.simulationSpeed += velocity;
			particle.Play();
		}
	}

	[Button]
	public void ModifyColor(float r, float g, float b){
		foreach(var particle in _particleSystems){
			var render = particle.GetComponent<Renderer>();
			var renderMaterial = render.material;
			var tintColor = renderMaterial.GetColor($"_Color");
			var shadowColor = renderMaterial.GetColor($"_ShadowColor");
			var tintColorModified = tintColor + new Color(r, g, b);
			var shadowColorModified = shadowColor + new Color(r, g, b);
			renderMaterial.SetColor($"_Color", tintColorModified);
			renderMaterial.SetColor($"_ShadowColor", shadowColorModified);
		}
	}
}