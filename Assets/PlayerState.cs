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
    State idleState = new Stately.State("idle"); // Intro
    State storyState = new State("story");
    State exploreState = new State("explore");
    State backwardsState = new State("backwards");

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
        rootState.ChangeToSubState ( idleState ).If ( ( ) => Input.GetButtonDown ( "Reset" ) ).ThenDo ( delegate
        {
            transform.position = startPos;
            SetColor ( this.gameObject, Color.yellow );
            SetVisible ( ghostMother, true );
            SetMusic ( null );
            particles.Stop ( );
        } );

        // Press "enter" to start the story.
        idleState.ChangeTo ( storyState ).If ( ( ) => Input.GetButtonDown ( "Submit" ) ).ThenDo ( delegate
        {
            SetColor ( this.gameObject, Color.green );
            SetVisible ( ghostMother, false );
            SetMusic ( storyClip );
        } );

        storyState.OnUpdate = delegate
        {
            transform.position += Vector3.forward * Time.deltaTime;
            ghostMother.transform.position += Vector3.forward * Time.deltaTime;
        };

        storyState.ChangeTo ( exploreState ).If ( ( ) => InputFirstDown ( ) ).ThenDo ( delegate
        {
            SetColor ( this.gameObject, Color.red );
            SetVisible ( ghostMother, true );
            SetMusic ( exploreClip );
            particles.Play ( );
        } ) ;

        exploreState.ChangeTo ( backwardsState ).If ( ( ) => InputSecondDown ( ) ).ThenDo ( delegate
        {
            SetColor ( this.gameObject, Color.blue );
            SetMusic ( backwardsClip );
            particles.Stop ( );
        } );

        backwardsState.OnUpdate = delegate
        {
            transform.position -= Vector3.forward * Time.deltaTime;
            ghostMother.transform.position -= Vector3.forward * Time.deltaTime;
        };

        backwardsState.ChangeTo ( exploreState ).If ( ( ) => InputFirstUp ( ) ).ThenDo ( delegate
        {
            SetColor ( this.gameObject, Color.red );
            SetMusic ( exploreClip );
            particles.Play ( );
        } );

        exploreState.ChangeTo ( storyState ).If ( ( ) => InputSecondUp ( ) ).ThenDo ( delegate
        {
            SetColor ( this.gameObject, Color.green );
            SetVisible ( ghostMother, false );
            SetMusic ( storyClip );
            particles.Stop ( );
        } );

        
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
