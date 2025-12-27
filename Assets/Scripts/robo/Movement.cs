using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    // Start is called before the first frame update
    public PlayerController controller;
    public Animator animator;
    float horizontal = 0f;
    bool jump = false;
    bool dash = false;


    

    // Update is called once per frame
    void Update()
    {
        horizontal=Input.GetAxisRaw("Horizontal");
        animator.SetFloat("Speed",Mathf.Abs(horizontal));
        if (Input.GetKeyDown(KeyCode.Z))
        {
            jump = true;
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            dash = true;
        }

    }
    private void FixedUpdate()
    {
        
        controller.Move(horizontal, jump, dash);
        jump = false;
        dash = false;
    }
}
