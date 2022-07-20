using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HelloPico2.StateMachine{
	[CreateAssetMenu(fileName = "stateOverview", menuName = "HelloPico2/ScriptableObject/ State Overview", order = 0)]
	public class StateOverview : ScriptableObject{
		[SerializeField] private List<StateWrapper> stateWrapperList;

		public IEnumerable<AbstractState> GetAllSetupState(){
			var stateList = stateWrapperList.Select(x => x.settingState);
			return stateList;
		}
	}

	[Serializable]
	public class StateWrapper{
		[SerializeReference] public AbstractState settingState;

		[ShowIf("settingState")]
		[SerializeReference]
		[BoxGroup("Set Changeable State")]
		[InlineButton("AddState")]
		[InlineButton("RemoveState")]
		private AbstractState pickedState;

		[SerializeReference] [ShowIf("settingState")] [BoxGroup("Set Changeable State")] [ReadOnly]
		private List<string> pickedStateList = new List<string>();

		private void AddState(){
			var type = pickedState.GetType();
			if(type == typeof(AbstractState)) throw new Exception("Can,t be abstract type");
			var instance = Activator.CreateInstance(type);
			var abstractState = (AbstractState)instance;
			if(CheckContains(abstractState)){
				Debug.Log("Is Exist !!!");
				return;
			}

			var changeableState = settingState.ChangeableState;
			changeableState.Add(abstractState);
			pickedStateList = changeableState.Select(x => x.ToString()).ToList();
		}

		private void RemoveState(){
			var type = pickedState.GetType();
			if(type == typeof(AbstractState)) throw new Exception("Can,t be abstract type");
			var instance = Activator.CreateInstance(type);
			var abstractState = (AbstractState)instance;
			if(!CheckContains(abstractState)){
				Debug.Log("Is Not Exist !!!");
				return;
			}

			var changeableState = settingState.ChangeableState;
			var state = changeableState.Find(x => x.ToString().Equals(abstractState.ToString()));
			changeableState.Remove(state);
			pickedStateList = changeableState.Select(x => x.ToString()).ToList();
		}

		private bool CheckContains(IState abstractState){
			var stateName = abstractState.ToString();
			var contains = pickedStateList.Contains(stateName);
			return contains;
		}
	}
}