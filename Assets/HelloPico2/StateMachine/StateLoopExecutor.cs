using UnityEngine;

namespace HelloPico2.StateMachine{
	public class StateLoopExecutor : MonoBehaviour{
		[SerializeReference] public AbstractState beginState;
		private IState _currentState;

		private void Start(){
			_currentState = beginState;
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