using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    private Vector3 newLocation;
    private Animator actorAnimator;
    public bool fadeIn = false;

    private void Start()
    {
        actorAnimator = gameObject.GetComponent<Animator>();
    }

    public void MoveActor(Vector3 nLoc)
    {
        Debug.Log(name + "  Moveedd");
        newLocation = nLoc;

        if (!fadeIn)
        {
            //Move actor
            gameObject.transform.localPosition = newLocation;
            //Fade actor in
            actorAnimator.SetBool("FadeIn", true);
            actorAnimator.SetBool("FadeOutComplete", false); //Get ready to fade out again
            fadeIn = true;
        }
        else
        {
            Debug.Log(name + "FALLSEEE");
            //Fade out the actor, actor will move usin an animation event at the end
            actorAnimator.SetBool("FadeIn", false);
            fadeIn = false;
        }
    }

    //Called from animator
    private void MoveAfterFade()
    {
        gameObject.transform.localPosition = newLocation;
        actorAnimator.SetBool("FadeOutComplete", true);
    }
}
