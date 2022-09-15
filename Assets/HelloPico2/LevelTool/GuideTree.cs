using System;
using System.Collections.Generic;
using HelloPico2.Rating_System;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace HelloPico2.LevelTool{
	public class GuideTree : MonoBehaviour{
		[SerializeField] private Transform treeRoot;
		[SerializeField] private string selectName;
		[SerializeField] private string ignoreName;
		[SerializeField] private List<GameObject> guideList;
		[FormerlySerializedAs("RatingSetting")] public RatingSetting ratingSetting;
		private void Start(){
			GetAllGuideMesh();
			SetGuideAmount();
		}

		private void SetGuideAmount(){
			var randomAmount = Random.Range(20,50);
			var closeGuide = guideList.GetRange(randomAmount , guideList.Count - randomAmount);
			closeGuide.ForEach(x => x.gameObject.SetActive(false));
		}

		[Button]
		private void GetAllGuideMesh(){
			guideList.Clear();
			GetChildRecursive(treeRoot.gameObject);
		}


		private void GetChildRecursive(GameObject obj){
			if(null == obj)
				return;
			foreach(Transform child in obj.transform){
				if(null == child)
					continue;
				var childGameObject = child.gameObject;
				var match = childGameObject.name.Contains(selectName);
				var ignore = childGameObject.name.Contains(ignoreName);
				if(match && !ignore){
					guideList.Add(childGameObject);
				}

				GetChildRecursive(childGameObject);
			}
		}

		public static void GetAllChild(Transform origin, List<GameObject> childList, [NotNull] Predicate<Transform> match){
			if(null == origin) return;
			foreach(Transform child in origin.transform){
				if(null == child)
					continue;
				var childGameObject = child.gameObject;
				if(match.Invoke(childGameObject.transform)){
					childList.Add(origin.gameObject);
				}

				GetAllChild(childGameObject.transform, childList, match);
			}
		}
	}
}