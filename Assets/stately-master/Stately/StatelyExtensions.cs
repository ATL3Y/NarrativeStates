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

using System;

namespace Stately
{
    public static class StatelyExtensions
    {
        public static Func<T, bool> And<T>(
            this Func<T, bool> predicate1, 
            Func<T, bool> predicate2) 
        {
            return arg => predicate1(arg) && predicate2(arg);
        }

        public static Func<T, bool> Or<T>(
            this Func<T, bool> predicate1, 
            Func<T, bool> predicate2) 
        {
            return arg => predicate1(arg) || predicate2(arg);
        }
    }
}