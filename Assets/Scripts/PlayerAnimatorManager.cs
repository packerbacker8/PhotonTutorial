using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorManager : Photon.MonoBehaviour
{
    public float DirectionDampTime = 0.25f;

    private Animator animator;

    // Use this for initialization
    void Start()
    {
        animator = this.GetComponent<Animator>();
        if (!animator)
        {
            Debug.Log("No animator present on this player.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        //check if player is current user
        if(!photonView.isMine && PhotonNetwork.connected)
        {
            return;
        }
        if (!animator)
        {
            return;
        }

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0); //0 = current animation
        //jump only if running
        if(animator.GetFloat("Forward") > 0)
        {
            //when using space button
            if (Input.GetKeyDown(KeyCode.Space))
            {
                //no jumping right now because i am lazy
                //animator.SetBool("OnGround",false);
            }
        }

        float horiz = Input.GetAxis("Horizontal");
        float vert = Input.GetAxis("Vertical");

        if (vert < 0)
        {
            vert = 0;
        }

        animator.SetFloat("Forward", horiz * horiz + vert * vert);
        animator.SetFloat("Turn", horiz, DirectionDampTime, Time.deltaTime);
    }
}
