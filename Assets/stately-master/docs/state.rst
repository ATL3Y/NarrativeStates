.. _state:

State
=====

A Stately state machine consists of a top-level state (known as the "root") and
various substates which are defined using Transitions. You will never reference
Transition except through methods provided by State.

A State consists of a name, various conditions for transitions between states,
and callback functions to execute when states are entered, updated, or exited.

Methods
-------

* :func:`Start() <Start>`
* :func:`StartAt(State otherState) <StartAt>`
* :func:`Reset() <Reset>`
* :func:`Update(float deltaTime) <Update>`
* :func:`FixedUpdate() <FixedUpdate>`
* :func:`ChangeTo(State otherState) <ChangeTo>`
* :func:`ChangeToSubState(State otherState) <ChangeToSubState>`
* :func:`SendSignal(string signal) <SendSignal>`
* :func:`OnTransitionTo(State otherState) <OnTransitionTo>`
* :func:`ReplaceTransitionCondition(State otherState) <ReplaceTransitionCondition`

Method Reference
------------------

.. function:: Start()

The final step in initializing the state machine. Should be called once and only
once on the root state before any updates are called.

.. function:: StartAt(State subState)

    :param State subState: The substate to begin at.

Sets the starting sub-state of a state. Essentially, you can think of this method
as creating a sub-level of the state.

.. function:: Reset()

Resets a state to its starting sub-state.

.. function:: Update(float deltaTime)

    :param float deltaTime: The change in time since the last call to Update.

Updates the state machine, checking for state transitions and updating timers.
Should only be called on the root state.

.. function:: FixedUpdate()

A convenience method intended for Unity. Allows for the OnFixedUpdate callback
to be used. Should only be called on the root state.

.. function:: ChangeTo(State otherState)

    :param State otherState: The state to create a `Transition` to.
    :returns: The `Transition` object between the two states.

The bread and butter of Stately. Creates a `Transition` object from the calling
state to the given state. `Transition` objects are never created or
manipulated directly. Instead, use expressions on states
to define transition conditions and callbacks.

**Examples**

.. code-block:: csharp

    idleState.ChangeTo(runningState).If(() => Input.GetButton("Run"));

The state will change from idleState to runningState if the Run button is down.

.. code-block:: csharp

    idleState.ChangeTo(jumpingState).If(() => Input.GetButton("Jump")).ThenDo(delegate {
      dustParticles.Emit(100);
    });

The state will change from idleState to jumpingState if the Jump button is down,
and then it will emit dust particles.

For a complete list of transition conditions and callback methods please refer
to <transition>

.. function:: ChangeToSubState(State otherState)

    :param State otherState: The state to create an `AnyStateTransition` to.
    :returns: The `AnyStateTransition` to the substate.

Called from a higher-level state using a substate as a parameter. When the
condition is met, it will transition from any substate into the given `otherState`.
Useful when you have many substates that can change between each other.

States will not self-transition with this method.

**Example**

.. code-block:: csharp

    rootState = new State("root");

    redState = new State("red");
    blueState = new State("blue");
    greenState = new State("green");
    yellowState = new State("yellow");

    rootState.ChangeToSubState(redState).IfSignalCaught("red");
    rootState.ChangeToSubState(blueState).IfSignalCaught("blue");
    rootState.ChangeToSubState(greenState).IfSignalCaught("green");
    rootState.ChangeToSubState(yellowState).IfSignalCaught("yellow");

    rootState.StartAt(redState);

Now, `redState`, `blueState`, `greenState`, and `yellowState` can all
transition between each other,
but we've narrowed this down to 4 transitions instead of 12. Nice!

.. function:: SendSignal(string signal)

    :param string signal: The signal to send to the state.

For use with the `IfSignalCaught` transition condition.
This signal propagates recursively until a state with no substate is reached.

.. function:: OnTransitionTo(State otherState)

    :param State otherState: The other state.
    :returns: The already existing `Transition` object between these two states.

Used to redefine transition callbacks in conjunction with the `InsteadDo` and
`AlsoDo` methods. Useful for extending or modifying transition
behavior when inheriting classes that implement a state machine.

**Example**

.. code-block:: csharp

    idleState.OnTransitionTo(jumpingState).InsteadDo(delegate {
      dustParticles.Emit(500);
    });

Jumping now emits 500 dust particles instead of 100!

.. function:: ReplaceTransitionCondition(State otherState)

    :param State otherState: The other state.
    :returns: The already existing `Transition` object between these two states.

This function is similar to `OnTransitionTo`, except that it erases the
transition condition so it can be replaced. Useful for extending or modifying
transition behavior when inheriting classes that implement a state machine.

**Example**

.. code-block:: csharp

    idleState.ReplaceTransitionCondition(jumpingState).With.After(2f);

Now the idle state will transition to the jumping state after 2 seconds instead
of waiting for a button press.

Callback Reference
------------------

``OnEnter``
  Runs when the state is entered.

``OnUpdate``
  Runs on each update tick.

``OnFixedUpdate``
  Runs on each FixedUpdate tick (convenience method for Unity).

``OnExit``
  Runs when the state is exited.

Property Reference
------------------

.. function:: string Name

    :returns: The name of the ``State``.

.. function:: State CurrentState

    :returns: The current substate of the ``State``.

.. function:: State CurrentStateRecursive

    :returns: The deepest substate of the ``State``.

.. function:: string CurrentStatePath

    :returns: A string concatenating the names of each active substate.

Useful for debug purposes.

**Example**

.. code-block:: csharp

    rootState = new State("root");
    idleState = new State("idle");
    animatingState = new State("animating");

    rootState.StartAt(idleState);
    idleState.StartAt(animatingState);

In this case, ``rootState.CurrentStatePath`` would return ``"root.idle.animating"``.
