using System;
using System.Collections.Generic;

namespace HelloPico2.StateMachine{
	[Serializable]
	public abstract class AbstractState : IState{
		public List<IState> ChangeableState = new List<IState>();
		public abstract IState NextState();
		public abstract void Begin();
		public abstract void Executing();
		public abstract void End();
	}

	public interface IState{
		IState NextState();
		void Begin();
		void Executing();
		void End();
	}
}