Quickstart Guide
================

So you want to make a state machine. You've come to the right place.

Let's get started.

We are going to use Unity as our example here, but these concepts should
be easily adaptable to any other game engine that uses C#.

First, let's include Stately and define a simple ``GameObject`` script.
We'll need a top-level state.
Let's call it ``rootState``.

.. code-block:: csharp

  using Stately;

  public class Cube : MonoBehaviour
  {
    State rootState = new State("root");

    void Awake();
    {
      DefineStateMachine();
    }

    void Start()
    {
      rootState.Start();
    }

    public void DefineStateMachine()
    {

    }

    void Update()
    {
      rootState.Update(Time.deltaTime);
    }
  }

The designer says she wants the cube to jump when the player is on the
ground and presses the jump button. A simple task, with the power of state machines!

.. code-block:: csharp

  State idleState = new State("idle");
  State jumpState = new State("jump");

  Transform transform;
  Vector3 velocity = Vector3.Zero;

  void Awake()
  {
    transform = GetComponent<Transform>();
  }

  public void DefineStateMachine()
  {
    rootState.StartAt(idleState);

    idleState.ChangeTo(jumpState).If(() => Input.GetButtonDown("Jump"));

    jumpState.OnEnter = delegate
    {
      velocity = new Vector3(0f, 5f, 0f);
    };

    jumpState.OnUpdate = (deltaTime) =>
    {
      velocity.y -= 0.5f * deltaTime;
    };

    jumpState.ChangeTo(idleState).If(() => transform.position.y <= 0f);
  }

  void Update()
  {
    rootState.Update(Time.deltaTime);
    transform.position += velocity * Time.deltaTime;
  }

Let's break down what's going on here.

State Behavior
--------------

.. code-block:: csharp

  rootState.StartAt(idleState);

`StartAt` tells a state to use a state as its starting substate. This sets up
the state hierarchy. In this case, we wish ``idleState`` to be the starting state
of the object.

.. code-block:: csharp

  jumpState.OnEnter = delegate
  {
    velocity = new Vector3(0f, 5f, 0f);
  };

`OnEnter` is a callback which is called when the state is entered. Simple enough.
In this case, when ``jumpState`` is entered, we want to set the object's ``velocity``
to ``{0f, 5f, 0f}``.

.. code-block:: csharp

  jumpState.OnUpdate = (deltaTime) =>
  {
    velocity.y -= 0.5f * deltaTime;
  };

`OnUpdate` is, as you might expect, a callback which is called when the state
is updated. It takes the time step as the sole argument.
In this case, to simulate the effects of gravity, we want to
decrease the object's ``y`` velocity by a factor of half the time step on each
update tick.

For more information on defining State behavior, check the :ref:`State documentation <state>`.

Transitions
-----------

.. code-block:: csharp

  idleState.ChangeTo(jumpState).If(() => Input.GetButtonDown("Jump"));

`ChangeTo` designates a transition between two states.
In this case, ``idleState.ChangeTo(jumpState)`` sets up a transition from ``idleState`` to ``jumpState``.
Now, what's all that business past the `ChangeTo` call?

Each transition requires a condition that will trigger the transition.
There are a few different condition methods that we can use,
but the most basic one is `If`.

`If` takes a method which has zero arguments and returns a boolean. We can
use C#'s convenient anonymous function syntax for readability.

So this snippet means that the transition will be executed when the jump button is pressed,
changing the active substate of ``rootState`` from ``idleState`` to ``jumpState``.

Now the designer wants to give the visuals a little oomph. She wants dust
particles to appear when the cube jumps. No problem! We can accomplish
this with a transition callback.

.. code-block:: csharp

  ParticleSystem dustParticleSystem;

  void Awake()
  {
    // ...

    dustParticleSystem = GetComponent<ParticleSystem>();
  }

  void DefineStateMachine()
  {
    // ...

    idleState.ChangeTo(jumpState).If(() => Input.GetButtonDown("Jump")).ThenDo(() =>
    {
      dustParticleSystem.Emit(100);
    });

    // ...
  }

`ThenDo` is used to specify a method which should be called when the transition is
executed. It is executed after `OnExit` of the previous state and before `OnEnter` of the new state.

Why would you use `ThenDo` instead of `OnExit`? Simply, if you have one state that
branches into two other states, you can define transition-specific behavior
depending on which transition is executed.

For more information on defining Transitions, check the :ref:`Transition documentation <transition>`.

Inheritance
-----------

The designer wants a new type of Cube that has slightly different behavior.
This cube should emit lots more dust when it jumps. No problem!
Stately has functions to redefine state behavior so you can avoid
duplicating code.

.. code-block:: csharp

  public class DustyCube : Cube
  {
    override void DefineStateMachine()
    {
      base.DefineStateMachine();

      idleState.OnTransitionTo(jumpState).InsteadDo(() =>
      {
        dustParticleSystem.Emit(1000);
      });
    }
  }

Now 10 times more dust particles will be emitted by the cube when it jumps!
Wow!

The designer wants a different kind of cube now. This one should automatically
jump two seconds after it touches the ground.

.. code-block:: csharp

  public class AutoJumpCube : Cube
  {
    override void DefineStateMachine()
    {
      base.DefineStateMachine();

      idleState.ReplaceTransitionCondition(jumpState).With.After(2f);
    }
  }

Now the cube will jump two seconds after entering the idle state! Easy!

This concludes the quickstart guide. You should have a good overview of the concepts
you'll need to build state machines with Stately.

Please reference the class-specific documentation if you are
in need of further clarification. I hope you enjoy building software with Stately!
