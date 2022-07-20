using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HelloPico2.StateMachine{
	public class StateLoopExecutor : MonoBehaviour{
		[Required] public StateOverview stateOverview;
		private IState _currentState;

		private void Start(){
			var allSetupState = stateOverview.GetAllSetupState();
			_currentState = allSetupState.First();
			_currentState.Begin();
		}

		private void FixedUpdate(){
			_currentState.Executing();
			var nextState = _currentState.NextState();
			if(!_currentState.Equals(nextState)) ChangeState(nextState);
		}

		private void ChangeState(IState nextState){
			_currentState.End();
			_currentState = nextState;
			_currentState.Begin();
		}
	}
}