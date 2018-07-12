.. _transition:

Transition
==========

Transitions should always be referenced through methods provided by State.

All Transition methods return the Transition object itself so that its
methods can be chained.

Methods
-------

* :func:`If(System.Func\<bool\> condition) <If>`
* :func:`AndIf(System.Func\<bool\> condition) <AndIf>`
* :func:`OrIf(System.Func\<bool\> condition) <OrIf>`
* :func:`IfSignalCaught(string signal) <IfSignalCaught>`
* :func:`AndIfSignalCaught(string signal) <AndIfSignalCaught>`
* :func:`OrIfSignalCaught(string signal) <OrIfSignalCaught>`
* :func:`After(float duration) <After>`
* :func:`AndAfter(float duration) <AndAfter>`
* :func:`OrAfter(float duration) <OrAfter>`
* :func:`AfterOneFrame() <AfterOneFrame>`
* :func:`AfterNFrames(int n) <AfterNFrames>`
* :func:`ThenDo(System.Action onTransition) <ThenDo>`
* :func:`AlsoDo(System.Action onTransition) <AlsoDo>`
* :func:`InsteadDo(System.Action onTransition) <InsteadDo>`

Method Reference
------------------

.. function:: If(System.Func<bool> condition)

Defines an `If` transition between states. Performs the transition if the
function given as argument returns true.

.. function:: AndIf(System.Func<bool> condition)

Adds an additional condition to a transition.
Performs the transition if the previous condition and the
given function are both true.

.. function:: OrIf(System.Func<bool> condition)

Adds an additional condition to a transition.
Performs the transition if the previous condition or the given function
returns true.

.. function:: IfSignalCaught(string signal)

Defines an `IfSignalCaught` transition between states. Performs the transition
if a matching signal is sent through the state machine.

.. function:: AndIfSignalCaught(string signal)

Adds an additional condition to a transition. Performs the transition
if the previous condition is true and a matching signal is sent through
the state machine.

.. function:: OrIfSignalCaught(string signal)

Adds an additional condition to a transition. Performs the transition
if the previous condition is true or a matching signal is sent through
the state machine.

.. function:: After(float duration)

Defines an `After` transition between states. Performs the transition after the
given amount of time has passed.

.. function:: AndAfter(float duration)

Adds an additional condition to a transition. Performs the transition if
the previous condition is true and the given amount of time has passed.

.. function:: OrAfter(float duration)

Adds an additional condition to a transition. Performs the transition if the
previous condition is true or the given amount of time has passed.

.. function:: AfterOneFrame()

Defines an `AfterOneFrame` transition. Performs the transition after a single
Update has been completed.

.. function:: AfterNFrames(int n)

Defines an `AfterNFrames` transition. Performs the transition after ``n`` updates
have been completed.

.. function:: ThenDo(System.Action onTransition)

Defines a `ThenDo` callback on the transition. Executes the given callback
when the transition is performed. Executed between `OnExit` of the previous
state and `OnEnter` of the new state.

.. function:: AlsoDo(System.Action onTransition)

Adds an additional callback on the transition. Will be executed after the
originally defined callback given by `ThenDo`.

.. function:: InsteadDo(System.Action onTransition)

Overwrites the callback on the transition. This callback will be executed
instead of the originally defined callback.

Property Reference
------------------

.. function:: Transition With

  :returns: The Transition itself.

Used as part of the `ReplaceTransitionCondition` idiom.
