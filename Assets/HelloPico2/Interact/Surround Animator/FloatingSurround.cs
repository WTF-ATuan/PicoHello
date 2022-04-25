using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace HelloPico2.Interact.SpineCreator{
	[SuppressMessage("ReSharper", "PossibleLossOfFraction")]
	public class FloatingSurround : MonoBehaviour{
		[SerializeField] private GameObject bigDotPrefab;
		[SerializeField] private GameObject smallDotPrefab;


		private Transform _bigDotsContainer;
		private Transform _smallDotsContainer;

		private List<GameObject> _bigDotsGameObjects;
		private List<GameObject> _smallDotsGameObjects;

		[FormerlySerializedAs("bigDots")] [Tooltip("Big dots count which each is animated separately")] [Range(0, 24)]
		public int bigDotCount = 12;

		[FormerlySerializedAs("smallDots")]
		[Tooltip("Small dots count which are only rotated in one transform")]
		[Range(0, 24)]
		public int smallDotCount = 8;

		[Header("Placement Parameters")] public float rotationSpeed = 50f;
		public float bigDotsRadius = 50.0f;
		public float smallDotsRadius = 10.0f;

		[Header("Animation Parameters")] public float animationSpeed = 1f;
		public float bigRadiusVariation = 10f;
		public float iterationOffset = 1f;
		public float sinusMultiplier = 1f;
		public float cosinusMultiplier = 1f;

		[Header("Additional Parameters")] public float waverSpeed = 1f;
		public float bigDotsRadiusWaver = 0f;
		private WaveCreator _bigDotsWaver;

		public float smallDotsRadiusWaver = 0f;
		private WaveCreator _smallDotsWaver;

		public float bigRadiusVariationWaver = 0f;
		private WaveCreator _bigRadiusWaver;

		public float sinusMultiplierWaver = 0f;
		private WaveCreator _sinusWaver;

		public float cosinusMultiplierWaver = 0f;
		private WaveCreator _cosinusWaver;


		private float _time;

		private void Start(){
			SetupContainer();
			_bigDotsGameObjects = new List<GameObject>();
			_smallDotsGameObjects = new List<GameObject>();
			SetupWaveCreator();
			ClearPreviousObjects();
			CreateDotObjects();
		}

		[BoxGroup]
		[Button(ButtonSizes.Large)]
		public void ModifyBigDotCount(int amount){
			if(!bigDotPrefab) return;
			bigDotCount = amount;
			ClearPreviousObjects();
			CreateDotObjects();
		}

		[BoxGroup]
		[Button(ButtonSizes.Large)]
		public void ModifySmallDotCount(int amount){
			if(!smallDotPrefab) return;
			smallDotCount = amount;
			ClearPreviousObjects();
			CreateDotObjects();
		}

		private void CreateDotObjects(){
			if(bigDotPrefab){
				if(bigDotCount > 0){
					float step = 360 / bigDotCount;

					for(var i = 0; i < bigDotCount; i++){
						var o = Instantiate(bigDotPrefab, _bigDotsContainer);
						o.transform.rotation = Quaternion.Euler(0f, 0f, i * step);
						o.transform.localPosition = Vector3.zero;
						_bigDotsGameObjects.Add(o.transform.GetChild(0).gameObject);
					}
				}
			}

			if(smallDotPrefab){
				if(smallDotCount > 0){
					float step = 360 / smallDotCount;

					for(var i = 0; i < bigDotCount; i++){
						var o = Instantiate(smallDotPrefab, _smallDotsContainer);
						o.transform.rotation = Quaternion.Euler(0f, 0f, i * step);
						o.transform.localPosition = Vector3.zero;
						_smallDotsGameObjects.Add(o.transform.GetChild(0).gameObject);
					}
				}
			}

			foreach(var smallDot in _smallDotsGameObjects)
				smallDot.transform.localPosition = new Vector2(0f, smallDotsRadius);


			_time = Random.Range(-Mathf.PI, Mathf.PI);
		}

		private void ClearPreviousObjects(){
			_bigDotsGameObjects.ForEach(x => Destroy(x.gameObject));
			_bigDotsGameObjects.Clear();
			_smallDotsGameObjects.ForEach(x => Destroy(x.gameObject));
			_smallDotsGameObjects.Clear();
		}

		private void SetupWaveCreator(){
			_bigDotsWaver = new WaveCreator();
			_smallDotsWaver = new WaveCreator();
			_bigRadiusWaver = new WaveCreator();
			_sinusWaver = new WaveCreator();
			_cosinusWaver = new WaveCreator();
		}

		private void SetupContainer(){
			if(bigDotPrefab){
				_bigDotsContainer = new GameObject(name + "-BigDots").transform;
				_bigDotsContainer.SetParent(transform);
				_bigDotsContainer.localScale = transform.localScale;
				_bigDotsContainer.localPosition = Vector3.zero;
			}

			if(smallDotPrefab){
				_smallDotsContainer = new GameObject(name + "-SmallDots").transform;
				_smallDotsContainer.SetParent(transform);
				_smallDotsContainer.localScale = transform.localScale;
				_smallDotsContainer.localPosition = Vector3.zero;
			}
		}

		private void Update(){
			_time += Time.unscaledDeltaTime * animationSpeed;
			if(!Application.isPlaying)
				if(Time.unscaledDeltaTime == 0f)
					_time += 0.015625f * animationSpeed;

			// Additional options calculations if used
			float bigD = 0f, smallD = 0f, bigRadD = 0f, sinD = 0f, cosD = 0f;

			if(bigDotsRadiusWaver != 0f){
				_bigDotsWaver.timeSpeed = waverSpeed;
				bigD = _bigDotsWaver.GetValue() * bigDotsRadiusWaver;
			}

			if(smallDotsRadiusWaver != 0f){
				_smallDotsWaver.timeSpeed = waverSpeed;
				smallD = _smallDotsWaver.GetValue() * smallDotsRadiusWaver;
			}

			if(bigRadiusVariationWaver != 0f){
				_bigRadiusWaver.timeSpeed = waverSpeed;
				bigRadD = _bigRadiusWaver.GetValue() * bigRadiusVariationWaver;
			}

			if(sinusMultiplierWaver != 0f){
				_sinusWaver.timeSpeed = waverSpeed;
				sinD = _sinusWaver.GetValue() * sinusMultiplierWaver;
			}

			if(cosinusMultiplierWaver != 0f){
				_cosinusWaver.timeSpeed = waverSpeed;
				cosD = _cosinusWaver.GetValue() * cosinusMultiplierWaver;
			}

			RotatingDots(smallD);
			AnimatingDots(cosD, bigD, bigRadD, sinD);
		}

		private void AnimatingDots(float cosD, float bigD, float bigRadD, float sinD){
			for(var i = 0; i < _bigDotsGameObjects.Count; i++){
				var cos = Mathf.Cos(_time * 5.5f + i * iterationOffset) * (cosinusMultiplier + cosD);
				var sin = (bigDotsRadius + bigD + Mathf.Sin(_time * 5.5f + i * bigDotsRadius) *
					(bigRadiusVariation + bigRadD)) * (sinusMultiplier + sinD);
				_bigDotsGameObjects[i].transform.localPosition = new Vector3(cos, sin, 0);
			}
		}

		private void RotatingDots(float smallD){
			if(_bigDotsContainer) _bigDotsContainer.Rotate(0f, 0f, Time.unscaledDeltaTime * rotationSpeed);

			if(!_smallDotsContainer) return;
			_smallDotsContainer.Rotate(0f, 0f, Time.unscaledDeltaTime * -rotationSpeed * 1.5f);
			if(smallDotsRadiusWaver == 0f) return;
			foreach(var smallDot in _smallDotsGameObjects){
				smallDot.transform.localPosition = new Vector3(0, smallDotsRadius + smallD);
			}
		}
	}
}