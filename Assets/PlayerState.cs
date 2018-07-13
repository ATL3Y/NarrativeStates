using System.Collections.Generic;
using UnityEngine;
using Stately;

public class PlayerState : MonoBehaviour {

    [SerializeField]
    GameObject ghostMother;
    [SerializeField]
    AudioSource audiosource;
    [SerializeField]
    ParticleSystem particles;
    [SerializeField]
    AudioClip storyClip;
    [SerializeField]
    AudioClip exploreClip;
    [SerializeField]
    AudioClip backwardsClip;

    State rootState = new State("root");
    State introState = new Stately.State("intro");
    State forwardState = new State("forward");
    State stoppedState = new State("stopped");
    State backwardState = new State("backward");

    Vector3 startPos;

	void Awake ()
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
            Debug.Log ( rootState.CurrentStatePath );
            UpdateInputState ( );
        };

        // Debug: reset to origin if press "r".
        rootState.ChangeToSubState ( introState ).If ( ( ) => Input.GetButtonDown ( "Reset" ) ).ThenDo ( delegate
        {
            transform.position = startPos;
            SetColor ( this.gameObject, Color.yellow );
            SetVisible ( ghostMother, true );
            SetMusic ( null );
            particles.Stop ( );
        } );

        // Press "enter" to start the story.
        introState.ChangeTo ( forwardState ).If ( ( ) => Input.GetButtonDown ( "Submit" ) ).ThenDo ( delegate
        {
            SetColor ( this.gameObject, Color.green );
            SetVisible ( ghostMother, false );
            SetMusic ( storyClip );

        } );

        forwardState.OnUpdate = delegate
        {
            transform.position += Vector3.forward * Time.deltaTime;
            ghostMother.transform.position += Vector3.forward * Time.deltaTime;
        };

        forwardState.ChangeTo ( stoppedState ).If ( ( ) => InputStatic( ) ).ThenDo ( delegate
        {
            SetColor ( this.gameObject, Color.red );
            SetVisible ( ghostMother, true );
            SetMusic ( exploreClip );
            particles.Play ( );
        } ) ;

        forwardState.ChangeTo ( backwardState ).If ( ( ) => InputBackward( ) ).ThenDo ( delegate
        {
            SetColor ( this.gameObject, Color.blue );
            SetMusic ( backwardsClip );
            particles.Stop ( );
        } );


        stoppedState.ChangeTo ( backwardState ).If ( ( ) => InputBackward ( ) ).ThenDo ( delegate
        {
            SetColor ( this.gameObject, Color.blue );
            SetMusic ( backwardsClip );
            particles.Stop ( );
        } );

        stoppedState.ChangeTo ( forwardState ).If ( ( ) => InputForward( ) ).ThenDo ( delegate
        {
            SetColor ( this.gameObject, Color.green );
            SetVisible ( ghostMother, false );
            SetMusic ( storyClip );
            particles.Stop ( );
        } );

        backwardState.ChangeTo ( forwardState ).If ( ( ) => InputForward ( ) ).ThenDo ( delegate
        {
        } );


        backwardState.ChangeTo ( stoppedState ).If ( ( ) => InputStatic( ) ).ThenDo ( delegate
        {
        } );

        backwardState.OnUpdate = delegate
        {
            transform.position -= Vector3.forward * Time.deltaTime;
            ghostMother.transform.position -= Vector3.forward * Time.deltaTime;
        };

        //backwardState.ChangeTo ( stoppedState ).If ( ( ) => InputFirstUp ( ) ).ThenDo ( delegate
        //{
        //    SetColor ( this.gameObject, Color.red );
        //    SetMusic ( exploreClip );
        //    particles.Play ( );
        //} );

       
        
    }

    // "Fire1" = Mouse 0 || L Ctrl.
    // "Fire2" = Mouse 1 || L Alt.

    private string leftButton = "Fire1";
    private string rightButton = "Fire2";

    private bool noneLastFrame = false;
    private bool oneLastFrame = false;
    private bool bothLastFrame = false;

    void UpdateInputState ( )
    {
        print ( "UpdateInputState" );
        if ( !Input.GetButton ( leftButton ) && !Input.GetButton ( rightButton ) )
        {
            noneLastFrame = true;
        }
        else
        {
            noneLastFrame = false;
        }
        if ( ( Input.GetButton ( leftButton ) && !Input.GetButton ( rightButton ) )
            || ( !Input.GetButton ( leftButton ) && Input.GetButton ( rightButton ) ) )
        {
            oneLastFrame = true;
        }
        else
        {
            oneLastFrame = false;
        }
        if ( Input.GetButton ( leftButton ) && Input.GetButton ( rightButton ) )
        {
            bothLastFrame = true;
        }
        else
        {
            bothLastFrame = false;
        }
    }


    bool InputForward ( )
    {
        print ( "InputForward" );
        if ( ( !Input.GetButton ( leftButton ) && !Input.GetButton ( rightButton ) ) )
        {
            return true;
        }
        return false;
    }


    bool InputBackward ( )
    {
        print ( "InputForward" );
        if ( ( Input.GetButton ( leftButton ) && Input.GetButton ( rightButton )  || ( Input.GetButtonDown ( leftButton ) && Input.GetButtonDown ( rightButton ) ) ) )
        {
            return true;
        }
        return false;
    }

    bool InputStatic ( )
    {
        print ( "InputForward" );
        if ( ( Input.GetButtonUp ( leftButton ) && Input.GetButtonUp ( rightButton ) ) )
        {
            return true;
        }
        return false;
    }



    // Return true on the frame the first button is pressed down.
    bool InputFirstDown ( )
    {
        print ( "InputFirstDown check" );
        if( noneLastFrame && ( Input.GetButtonDown ( leftButton ) || Input.GetButtonDown ( rightButton ) ) )
        {
            return true;
        }
        return false;
    }

    // Return true on the frame the second button is pressed down.
    
    bool InputSecondDown ( )
    {
        print ( "InputSecondDown check" );
        if ( oneLastFrame && ( Input.GetButtonDown ( leftButton ) || Input.GetButtonDown ( rightButton ) ) )
        {
            return true;
        }
        return false;
    }

    
    bool InputFirstUp ( )
    {
        print ( "InputFirstUp check" );
        if ( bothLastFrame && ( Input.GetButtonUp ( leftButton ) || Input.GetButtonUp ( rightButton ) ) )
        {
            return true;
        }
        return false;
    }

    // Return true on the frame the second button is lifted up. 
    bool InputSecondUp ( )
    {
        print ( "InputSecondUp check" );
        if ( oneLastFrame && ( Input.GetButtonUp ( leftButton ) || Input.GetButtonUp ( rightButton ) ) )
        {
            return true;
        }
        return false;
    }

    void Update ()
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
        audiosource.Play ( ); // what if clip == null?
    }
}
