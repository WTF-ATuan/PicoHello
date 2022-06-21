using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HelloPico2.TutorialSystem{
	[CreateAssetMenu(fileName = "TutorialDataOverview", menuName = "HelloPico2/ScriptableObject/ TutorialDataOverview")]
	public class TutorialDataOverview : ScriptableObject{
		public List<TutorialDataWrapper> tutorialDataList = new List<TutorialDataWrapper>();

		public T FindCondition<T>(Type trackedType) where T : AbstractCondition{
			var dataWrapper = tutorialDataList.Find(x => x.trackedComponent.GetType() == trackedType);
			var condition = dataWrapper.condition;
			return (T)condition;
		}

		public AbstractCondition FindCondition(Type trackedType){
			var dataWrapper = tutorialDataList.Find(x => x.trackedComponent.GetType() == trackedType);
			var condition = dataWrapper.condition;
			return condition;
		}
	}

	[Serializable]
	public class TutorialDataWrapper{
		[BoxGroup("Tracked Area")] public GameObject trackedPrefab;

		[BoxGroup("Tracked Area")] [ShowIf("trackedPrefab")] [ValueDropdown("GetComponentTypes")] [SerializeReference]
		public Component trackedComponent;

		[BoxGroup("Tracked Area")]
		[ShowIf("trackedPrefab")]
		[TypeFilter("GetConditionTypes")]
		[SerializeReference]
		[HideReferenceObjectPicker]
		public AbstractCondition condition;

		public void ResetData(){
			condition.Reset();
		}

		private IEnumerable GetComponentTypes(){
			var components = trackedPrefab?.GetComponents(typeof(Component)).ToList();
			if(components == null || components.Count < 1) return components;
			var valueDropdownItems = components.Select(x => new ValueDropdownItem(x.GetType().Name, x)).ToList();
			valueDropdownItems.Insert(0, new ValueDropdownItem("None", new Component()));
			return valueDropdownItems;
		}

		private IEnumerable<Type> GetConditionTypes(){
			var conditionType = typeof(AbstractCondition).Assembly
					.GetTypes()
					.Where(x => !x.IsAbstract)
					.Where(x => !x.IsGenericTypeDefinition)
					.Where(x => typeof(AbstractCondition).IsAssignableFrom(x));
			return conditionType;
		}
	}
}