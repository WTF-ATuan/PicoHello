using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace HelloPico2.TutorialSystem{
	[CreateAssetMenu(fileName = "ProcessDataOverview", menuName = "HelloPico2/ScriptableObject/ ProcessDataOverview")]
	public class ProcessDataOverview : ScriptableObject{
		[Required]
		[LabelText("Condition: ")]
		[BoxGroup("BehaviorCondition")]
		[PropertyOrder(1)]
		[TypeFilter("GetBehaviorBase")]
		[SerializeReference]
		[HideReferenceObjectPicker]
		[OdinSerialize]
		public ConditionBase condition;
		

		[BoxGroup("BehaviorCondition")]
		[PropertyOrder(1)]
		[TypeFilter("GetBehaviorBase")]
		[SerializeReference]
		[HideReferenceObjectPicker]
		[OdinSerialize]
		public List<ConditionBase> conditions;

		[UsedImplicitly]
		private IEnumerable GetBehaviorBase(){
			var b = typeof(ConditionBase).Assembly
					.GetTypes()
					.Where(x => x.IsAbstract == false)
					.Where(x => x.IsGenericTypeDefinition == false)
					.Where(x => typeof(ConditionBase).IsAssignableFrom(x))
					.Select(x => new ValueDropdownItem(x.Name, x));
			return b;
		}
	}
}