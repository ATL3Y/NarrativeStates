using System.Collections.Generic;
using UnityEngine;
using Stately;

public class PlayerState : MonoBehaviour
{

    [SerializeField]
    GameObject friend;
    [SerializeField]
    AudioSource audiosource;
    [SerializeField]
    ParticleSystem particles;
    [SerializeField]
    AudioClip[] music;

    State rootState = new State("root");
    State introState = new Stately.State("intro");
    State forwardState = new State("forward");
    State stoppedState = new State("stopped");
    State backwardState = new State("backward");
    private State endState = new State("end");

    Vector3 startPos;

    void Awake ( )
    {
        audiosource.loop = true;
        audiosource.volume = 1.0f;
        audiosource.spatialBlend = 1.0f;

        startPos = transform.position;
        SetColor ( this.gameObject, Color.white );

        DefineStateMachine ( );
        rootState.Start ( );
    }

    void DefineStateMachine ( )
    {
        rootState.OnUpdate = delegate
        {
            // Debug.Log ( rootState.CurrentStatePath );
            UpdateInputs ( );
        };

        // Debug: reset to origin if press "r".
        rootState.ChangeToSubState ( introState ).If ( ( ) => Input.GetButtonDown ( "Reset" ) ).ThenDo ( delegate
        {
            transform.position = startPos;
            SetColor ( this.gameObject, Color.yellow );
            SetVisible ( friend, false );
            SetMusic ( null );
            particles.Stop ( );
        } );

        // Press "enter" to start the story.
        introState.ChangeTo ( forwardState ).If ( ( ) => Input.GetButtonDown ( "Submit" ) );

        forwardState.OnUpdate = delegate
        {
            transform.position += Vector3.forward * Time.deltaTime;
            friend.transform.position += Vector3.forward * Time.deltaTime;
        };

        // Press "E" to end. 
        forwardState.ChangeTo ( endState ).If ( ( ) => Input.GetKeyDown ( KeyCode.E ) ).ThenDo ( delegate
        {
            SetColor ( this.gameObject, Color.white );
            SetMusic ( null );
        } );

        System.Func<bool> shouldStopCheck = () => IsNumDown ( 1 );
        System.Func<bool> shouldForwardCheck = () => IsNumDown ( 0 );
        System.Func<bool> shouldBackwardCheck = () => IsNumDown ( 2 );

        forwardState.ChangeTo ( stoppedState ).If ( shouldStopCheck );
        forwardState.ChangeTo ( backwardState ).If ( shouldBackwardCheck );

        stoppedState.ChangeTo ( backwardState ).If ( shouldBackwardCheck );
        stoppedState.ChangeTo ( forwardState ).If ( shouldForwardCheck );

        backwardState.ChangeTo ( forwardState ).If ( shouldForwardCheck );
        backwardState.ChangeTo ( stoppedState ).If ( shouldStopCheck );

        forwardState.OnEnter = delegate
        {
            Debug.Log ( rootState.CurrentStatePath + ".OnEnter" );
            SetColor ( this.gameObject, Color.green );
            SetVisible ( friend, true );
            SetMusic ( music [ 0 ] );
            particles.Stop ( );
        };

        stoppedState.OnEnter = delegate
        {
            Debug.Log ( rootState.CurrentStatePath + ".OnEnter" );
            SetColor ( this.gameObject, Color.red );
            SetVisible ( friend, false );
            SetMusic ( music [ 1 ] );
            particles.Play ( );
        };

        backwardState.OnEnter = delegate
        {
            Debug.Log ( rootState.CurrentStatePath + ".OnEnter" );
            SetColor ( this.gameObject, Color.blue );
            SetVisible ( friend, false );
            SetMusic ( music [ 2 ] );
            particles.Stop ( );
        };

        forwardState.OnUpdate = delegate
        {
            transform.localPosition += transform.forward * Time.deltaTime;
        };

        backwardState.OnUpdate = delegate
        {
            transform.localPosition -= transform.forward * Time.deltaTime;
        };
    }

    int numDown = 0;
    private bool IsNumDown ( int num )
    {
        return ( num == numDown );
    }

    void UpdateInputs ( )
    {
        numDown = 0;
        if ( Input.GetMouseButton ( 0 ) )
        {
            numDown++;
        }
        if ( Input.GetMouseButton ( 1 ) )
        {
            numDown++;
        }
        return;
    }

    void Update ( )
    {
        rootState.Update ( Time.deltaTime );
    }

    void FixedUpdate ( )
    {
        rootState.FixedUpdate ( );
    }

    void SetColor ( GameObject g, Color col )
    {
        foreach ( Renderer r in g.GetComponentsInChildren<Renderer> ( ) )
        {
            r.material.color = col;
        }
    }

    void SetVisible ( GameObject g, bool visible )
    {
        foreach ( Renderer r in g.GetComponentsInChildren<Renderer> ( ) )
        {
            r.enabled = visible;
        }
    }

    void SetMusic ( AudioClip clip )
    {
        if ( audiosource.isPlaying )
        {
            audiosource.Stop ( );
        }
        audiosource.clip = clip;
        audiosource.Play ( );
    }
}
