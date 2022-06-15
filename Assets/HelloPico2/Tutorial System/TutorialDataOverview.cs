using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HelloPico2.TutorialSystem{
	[CreateAssetMenu(fileName = "ProcessDataOverview", menuName = "HelloPico2/ScriptableObject/ ProcessDataOverview")]
	public class ProcessDataOverview : ScriptableObject{
		[BoxGroup("Tracked Area")] public GameObject trackedPrefab;

		[BoxGroup("Tracked Area")] [ShowIf("trackedPrefab")] [ValueDropdown("GetComponentTypes")] [SerializeReference]
		public Component trackedComponent;

		[BoxGroup("Tracked Area")] [ShowIf("trackedPrefab")] [TypeFilter("GetConditionTypes")] [SerializeReference] [HideReferenceObjectPicker]
		public AbstractCondition condition;


		private IEnumerable GetComponentTypes(){
			var components = trackedPrefab.GetComponents(typeof(Component)).ToList();
			var valueDropdownItems = components.Select(x => new ValueDropdownItem(x.GetType().Name, x)).ToList();
			valueDropdownItems.Insert(0, new ValueDropdownItem("None", null));
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