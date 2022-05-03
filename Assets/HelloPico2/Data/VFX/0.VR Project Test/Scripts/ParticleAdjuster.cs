using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class ParticleAdjuster : MonoBehaviour{
	private List<ParticleSystem> _particleSystems;

	private float _currentVelocity;
	private Color _currentColor;

	//get text
	public GameObject speedTextObj;
	public GameObject colorTextObj;

	private void Start(){
		_particleSystems = GetComponentsInChildren<ParticleSystem>().ToList();
		_particleSystems.RemoveAt(0);

		speedTextObj = GameObject.Find("speedText");
		colorTextObj = GameObject.Find("colorText");
	}

	[Button]
	public void ModifyVelocity(float velocity){
		foreach(var particle in _particleSystems){
			var mainModule = particle.main;
			mainModule.simulationSpeed += velocity;
			_currentVelocity = mainModule.simulationSpeed;
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
			var tintColorModified = tintColor + new Color(r, g, b, 0);
			var shadowColorModified = shadowColor + new Color(r, g, b, 0);
			_currentColor = tintColorModified;
			renderMaterial.SetColor($"_Color", tintColorModified);
			renderMaterial.SetColor($"_ShadowColor", shadowColorModified);
		}
	}

	private void OnGUI(){
		GUI.Label(new Rect(10, 10, 100, 20), _currentVelocity.ToString(CultureInfo.InvariantCulture));
		GUI.Label(new Rect(10, 30, 200, 20), _currentColor.ToString());
	}
    public void Update()
    {
		colorTextObj.GetComponent<Text>().text = System.String.Format("Color: {0:N3}", _currentColor);
		speedTextObj.GetComponent<Text>().text = System.String.Format("Speed: {0:N1}", _currentVelocity);
	}
}