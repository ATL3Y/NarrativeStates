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
	public class Transition {
		System.Func<List<string>, bool> condition = delegate { return false; };

		Timer timer;

		int frameCount = 0;

		State from;
		State to;
		System.Action onTransition = delegate { };

		public Transition With { get { return this; } }

		public System.Action OnTransition
		{
			get { return onTransition; }
			protected set { onTransition = value; }
		}

		public State From
		{
			get { return from; }
			protected set { from = value; }
		}
		public State To
		{
			get { return to; }
			protected set { to = value; }
		}

		public Transition(State from, State to)
		{
			From = from;
			To = to;
		}

		public bool Check(List<string> signals)
		{
			return condition(signals);
		}

		bool CheckSignals(List<string> signals, string signal)
		{
			if (signals.Contains(signal))
			{
				signals.Remove(signal);
				return true;
			} else
			{
				return false;
			}
		}

		public void Reset()
		{
			if (timer != null) { timer.Reset(); }
			frameCount = 0;
		}

		public void Update(float deltaTime) 
		{
			if (timer != null) { timer.Update(deltaTime); }
			frameCount += 1;
		}

		public Transition If(System.Func<bool> condition)
		{
			this.condition = (signals) => condition();
			return this;
		}

		public Transition AndIf(System.Func<bool> condition)
		{
			this.condition = this.condition.And((signals) => condition());
			return this;
		}

		public Transition OrIf(System.Func<bool> condition)
		{
			this.condition = this.condition.Or((signals) => condition());
			return this;
		}

		public Transition IfSignalCaught(string signal)
		{
			this.condition = (signals) => CheckSignals(signals, signal);
			return this;
		}

		public Transition AndIfSignalCaught(string signal)
		{
			this.condition = this.condition.And((signals) => CheckSignals(signals, signal));
			return this;
		}

		public Transition OrIfSignalCaught(string signal)
		{
			this.condition = this.condition.Or((signals) => CheckSignals(signals, signal));
			return this;
		}

		public Transition After(float duration)
		{
			timer = new Timer(duration);
			this.condition = (signals) => timer.IsComplete;
			return this;
		}

		public Transition AndAfter(float duration)
		{
			timer = new Timer(duration);
			this.condition = this.condition.And((signals) => timer.IsComplete);
			return this;
		}

		public Transition OrAfter(float duration)
		{
			timer = new Timer(duration);
			this.condition = this.condition.Or((signals) => timer.IsComplete);
			return this;
		}

		public Transition AfterOneFrame()
		{
			this.condition = (signals) => frameCount == 1;
			return this;
		}

		public Transition AfterNFrames(int n)
		{
			this.condition = (signals) => frameCount == n;
			return this;
		}

		public void ThenDo(System.Action onTransition)
		{
			OnTransition = onTransition;
		}

		public void AlsoDo(System.Action onTransition)
		{
			OnTransition += onTransition;
		}

		public void InsteadDo(System.Action onTransition)
		{
			ThenDo(onTransition);
		}

		public void ReplaceCondition()
		{
			this.condition = delegate { return false; };
		}
	}
}