 /*
    stately, a state machine system for C#
    Copyright (C) 2018 Evan Hemsley

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System.Collections;
using System.Collections.Generic;

namespace Stately {
	public class State {
		public string Name { get; protected set; }

		State currentSubState = null;
		State startingSubState = null;

		List<Transition> transitions = new List<Transition>();

		HashSet<State> transitionsTo = new HashSet<State>();
		Dictionary<State, Transition> stateTransitionMap = new Dictionary<State, Transition>();

		List<string> signals = new List<string>();

		public System.Action OnEnter = delegate { };
		public System.Action<float> OnUpdate = delegate { };
		public System.Action OnFixedUpdate = delegate { };
		public System.Action OnExit = delegate { };

		public State CurrentState
		{
			get { return currentSubState; }
		}

		public State CurrentStateRecursive
		{
			get 
			{ 
				return currentSubState == null ? this : currentSubState.CurrentState; 
			}
		}

		public string CurrentStatePath
		{
			get
			{
				return Name + (currentSubState == null ? "" : "." + currentSubState.CurrentStatePath);
			}
		}

		public State(string name = "")
		{
			Name = name;
		}

		public void StartAt(State startingSubState)
		{
			this.startingSubState = startingSubState;
			currentSubState = this.startingSubState;
		}
		
		public void Reset()
		{
			Enter();
		}

		public void Start()
		{
			Enter();
		}

		void Enter()
		{
			ClearSignals();
			ResetTransitions();
			OnEnter();
			if (startingSubState != null)
			{
				currentSubState = startingSubState;
				currentSubState.Enter();
			}
		}

		void Exit()
		{
			OnExit();
			if (currentSubState != null)
			{
				currentSubState.Exit();
			}
		}

		public void Update(float deltaTime)
		{
			UpdateTransitions(deltaTime);
			CheckTransitions();

			ClearSignals();

			OnUpdate(deltaTime);
			if (currentSubState != null)
			{
				currentSubState.Update(deltaTime);
			}
		}

		public void FixedUpdate()
		{
			OnFixedUpdate();
			if (currentSubState != null)
			{
				currentSubState.FixedUpdate();
			}
		}

		public Transition ChangeTo(State state)
		{
			Transition transition = new Transition(this, state);
			if (transitionsTo.Contains(state))
			{
				transitions.Remove(stateTransitionMap[state]);
				transitionsTo.Remove(state);
				stateTransitionMap.Remove(state);
			}
			transitions.Add(transition);
			transitionsTo.Add(state);
			stateTransitionMap[state] = transition;
			return transition;
		}

		public Transition ChangeToSubState(State state)
		{
			AnyStateTransition transition = new AnyStateTransition(this, state);
			if (transitionsTo.Contains(state))
			{
				transitions.Remove(stateTransitionMap[state]);
				transitionsTo.Remove(state);
				stateTransitionMap.Remove(state);
			}
			transitions.Add(transition);
			transitionsTo.Add(state);
			stateTransitionMap[state] = transition;
			return transition;
		}

		public void SendSignal(string signal)
		{
			signals.Add(signal);
			if (currentSubState != null)
			{
				currentSubState.SendSignal(signal);
			}
		}

		public Transition OnTransitionTo(State state)
		{
			return stateTransitionMap[state];
		}

		public Transition ReplaceTransitionCondition(State state)
		{
			Transition transition = stateTransitionMap[state];
			transition.ReplaceCondition();
			return stateTransitionMap[state];
		}

		void ClearSignals()
		{
			signals.Clear();
		}

		void ResetTransitions()
		{
			for (var i = 0; i < transitions.Count; i++)
			{
				var transition = transitions[i];
				transition.Reset();
			}
		}

		void UpdateTransitions(float deltaTime)
		{
			for (var i = 0; i < transitions.Count; i++)
			{
				var transition = transitions[i];
				transition.Update(deltaTime);
			}
		}

		void CheckTransitions()
		{
			// TODO: use a dictionary to represent possible transitions instead of looping
			if (currentSubState != null)
			{
				for (var i = 0; i < currentSubState.transitions.Count; i++)
				{
					var transition = currentSubState.transitions[i];
					if (transition.From == currentSubState)
					{
						if (transition.Check(signals))
						{
							TransitionTo(transition);
						}
					}
				}
			}

			for (var i = 0; i < transitions.Count; i++)
			{
				var transition = transitions[i];
				if (transition is AnyStateTransition)
				{
					if (CurrentState != transition.To && transition.Check(signals))
					{
						TransitionTo(transition);
					}
				}
			}
		}

		void TransitionTo(Transition transition)
		{
			if (currentSubState != null)
			{
				currentSubState.Exit();
			}

			transition.OnTransition();
			currentSubState = transition.To;
			currentSubState.Enter();
		}
	}
}