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
        //Edit this value to change how long befre stop is evaluate (times the deltaTime)
        int inputDelayMultiplier = 30;

        rootState.OnUpdate = delegate 
        {
            Debug.Log ( rootState.CurrentStatePath );
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
        introState.ChangeTo ( forwardState ).If ( ( ) => Input.GetButtonDown ( "Submit" ) );

        forwardState.OnUpdate = delegate
        {
            transform.position += Vector3.forward * Time.deltaTime;
            ghostMother.transform.position += Vector3.forward * Time.deltaTime;
        };


        //I got rid of duplicate code by setting certain properties on OnEnter
        forwardState.OnEnter = delegate
        {
            SetColor(this.gameObject, Color.green);
            SetVisible(ghostMother, false);
            SetMusic(storyClip);
            particles.Stop();

        };
        forwardState.ChangeTo(backwardState).If(() => InputBackward());
        //So my solution to making sure stop didn't happen immediately is to add in a delay the "AndAfter" check
        //It is possible to jump right to backwardState if both buttons are pressed, but it all depends on the number multiplying Time.deltaTime in AndAfter()
        forwardState.ChangeTo(stoppedState).If(() => InputStopped()).AndAfter(Time.deltaTime * inputDelayMultiplier);

        stoppedState.OnEnter = delegate
        {
            SetColor(this.gameObject, Color.red);
            SetVisible(ghostMother, true);
            SetMusic(exploreClip);
            particles.Play();
        };

        stoppedState.ChangeTo(forwardState).If(() => InputForward());
        stoppedState.ChangeTo( backwardState).If(() => InputBackward());

        backwardState.OnUpdate = delegate
        {
            transform.position -= Vector3.forward * Time.deltaTime;
            ghostMother.transform.position -= Vector3.forward * Time.deltaTime;
        };

        backwardState.OnEnter = delegate
        {
            SetColor(this.gameObject, Color.blue);
            SetMusic(backwardsClip);
            particles.Stop();
        };

        backwardState.ChangeTo(forwardState).If(() => InputForward());
        backwardState.ChangeTo(stoppedState).If(() => InputStopped() ).AndAfter(Time.deltaTime * inputDelayMultiplier);
    }

    // "Fire1" = Mouse 0 || L Ctrl.
    // "Fire2" = Mouse 1 || L Alt.

    private string leftButton = "Fire1";
    private string rightButton = "Fire2";

    private bool noneLastFrame = false;
    private bool oneLastFrame = false;
    private bool bothLastFrame = false;

    //enum TransitionType {
    //    Forward,
    //    Backward,
    //    Stopped,
    //};

    bool InputForward ( )
    {
        //Test if no buttons
        if (
          (!Input.GetButton(leftButton) && !Input.GetButton(rightButton))
        )
        {
            return true;
        }
        return false;
    }


    bool InputBackward ( )
    {
        //Test if both buttons down
        if (
            (Input.GetButton(leftButton) && Input.GetButton(rightButton) ) 
        )
        {
            return true;
        }
        return false;
    }

    //float delayTime = 0;

    bool InputStopped ( )
    {
        //Test if only one button
        if (
              (Input.GetButton(leftButton) && !Input.GetButton(rightButton)) ||
              (Input.GetButton(rightButton) && !Input.GetButton(leftButton))
          )
        {
            return true;
        }
        return false;

        //If instead you want to delay with more specific logic than "AndAfter", you can store the time and evaluate how how time passed
        //Commenting out code

        //bool check = (Input.GetButton(leftButton) && !Input.GetButton(rightButton)) || (Input.GetButton(rightButton) && !Input.GetButton(leftButton));
        ////Test if no buttons
        //if (
        //    check &&
        //    (Time.time >= delayTime)
        //    )
        //{
        //    delayTime = 0;
        //    return true;
        //}
        //if (check && delayTime == 0)
        //{
        //    delayTime = Time.time + 1;
        //}

        //return false;


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
