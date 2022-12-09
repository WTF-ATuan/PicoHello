using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace HelloPico2.LevelTool{
	public class ChildActiveEvent : MonoBehaviour{
		public bool destroyWhenFinish;
		public bool selectChild;
		[ShowIf("selectChild")] public List<GameObject> selectedList;

		[ReadOnly] public List<GameObject> childList = new List<GameObject>();
		public UnityEvent onChildAllActive;

		[Button]
		private void GetChild(){
			childList.Clear();
			if(selectChild){
				childList.AddRange(selectedList);
			}
			else{
				for(var i = 0; i < transform.childCount; i++){
					var child = transform.GetChild(i);
					childList.Add(child.gameObject);
				}
			}
		}

		private void Start(){
			Application.backgroundLoadingPriority = ThreadPriority.Low;
		}

		private void LateUpdate(){
			var anyFalse = childList.Any(x => x.activeSelf == false);
			if(anyFalse) return;
			onChildAllActive?.Invoke();
			if(destroyWhenFinish) Destroy(this);
		}
	}
}