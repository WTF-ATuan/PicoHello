using System.Collections;
using System.Linq;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace HelloPico2.Process_System{
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