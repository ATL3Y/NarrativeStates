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

namespace Stately {
    public class Timer {
        float duration;
        float timeElapsed;
        bool complete;

        public bool IsComplete {
            get { return complete; }
        }

        public Timer(float duration)
        {
            this.duration = duration;
            timeElapsed = 0f;
            complete = false;
        }

        public void Reset() {
            timeElapsed = 0f;
            complete = false;
        }

        public void Update(float deltaTime) {
            timeElapsed += deltaTime;

            if (timeElapsed >= duration) {
                complete = true;
            }
        }
    }
}