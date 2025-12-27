using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public LayerMask groundLayer;
    public Transform GroundCheck;
    public Transform WallCheck;
    private Rigidbody2D rb;
    private Animator animator;
    Vector3 velocity = Vector3.zero;
    Vector3 startVelocity = Vector3.zero;
    

    public float JumpForce = 200f;
    public float DashSpeed = 15f;
    public float MaxFallSpeed = 25f;
    public float AirResist = 0.5f;
    public float runSpeed;
    //private float life = 100f;
    private float Smoothing = 0.5f;//一定速度になるまでの時間
    private float GroundRadios = 0.18f;
    //private float preVelocityX = 0f;

    private bool isGround;
    private bool isWall = false;

    private bool canMove = true;
    private bool canDash=true;
    private bool canDoubleJump = true;
    //private bool invinvcible = false;
    private bool FaceRight = true;


    private bool isDashing = false;
    private bool isWallSlidding = false;
    //private bool oldWallSlidding = false;
    //private bool canCheck = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //接地判定
        bool wasGround = isGround;

        isGround = false;
        Collider2D[] collider = Physics2D.OverlapCircleAll(GroundCheck.position, GroundRadios, groundLayer);
        for(int i = 0; i < collider.Length; i++)
        {
            if (collider[i] != gameObject)
            {
                isGround = true;
                /*if (!wasGround&& isGround && (animator.GetBool("IsJump") || animator.GetBool("IsDoubleJump")) && (rb.velocity.y>0))
                {
                    animator.SetBool("IsJump", false); // ジャンプ中かつ接地したらジャンプアニメーションを終了
                    animator.SetBool("IsDoubleJump", false);
                }*/

            }
        }
        

        if (isGround)
        {
            canDoubleJump = true;
        }
        isWall = false;
        

        //空中のときの処理
        if (!isGround)
        {
            Collider2D[] collider2 = Physics2D.OverlapCircleAll(WallCheck.position, GroundRadios, groundLayer);
            for(int i = 0;i < collider2.Length; i++)
            {
                if (collider2[i] != gameObject)
                {
                    isWall = true;
                    isDashing = false;//壁にぶつかるとダッシュを中断
                }
            }
        }
        

    }
    
    public void Move(float move, bool jump, bool dash)//他のスクリプトに渡して実行
    {
        if (canMove)
        {
            
            Vector3 targetVelocity = new Vector2(move * runSpeed, rb.velocity.y);
            //ダッシュの処理
            if (dash && canDash && !isWall)
            {
                StartCoroutine(Dash());
            }
            if (isDashing)
            {
                rb.velocity = new Vector2(transform.localScale.x * DashSpeed, 0);
            }
            else if (isGround)
            {
                
                rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, Smoothing - 0.4f);
                                
                
            }
            else if (!isGround)
            {
                if(rb.velocity.y < -MaxFallSpeed)
                {
                    rb.velocity = new Vector2(rb.velocity.x, -MaxFallSpeed);
                }
                rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, Smoothing+AirResist);

            }
            //向きを変える
            if (move > 0 && !FaceRight && !isWallSlidding)
            {
                Flip();
            }else if (move < 0 && FaceRight && !isWallSlidding)
            {
                Flip();
            }
            //ジャンプの処理
            if (isGround && jump)
            {
                rb.AddForce(new Vector2(0, JumpForce));
                isGround = false;
                animator.SetBool("IsJump", true);
                canDoubleJump = true;
            }
            
            //二段ジャンプの処理
            else if(!isGround&& jump && canDoubleJump)
            {
                animator.SetBool("IsDoubleJump", true);
                canDoubleJump= false;
                rb.velocity = new Vector2(rb.velocity.x, 0);
                rb.AddForce(new Vector2(0, JumpForce/1.2f));
            }
            //壁スライド

        }
        
        if (!isGround && rb.velocity.y < 0)
        {
            animator.SetBool("IsFall", true);
            animator.SetBool("IsJump", false);
            animator.SetBool("IsDoubleJump", false);
        }
        else if(isGround)
        {
            animator.SetBool("IsFall", false);
        }
        if(!isGround && rb.velocity.y > 0)
        {
            animator.SetBool("IsJump", true);
        }
        if (isGround)
        {
            animator.SetBool("IsJump", false);
            animator.SetBool("IsDoubleJump", false);
        }
        
    }
    /*public void BeHitted(float damage,Vector3 position)
    {

    }*/
    void Flip()
    {
        FaceRight = !FaceRight;
        Vector3 size=transform.localScale;
        size.x *= -1;
        transform.localScale = size;
    }
    IEnumerator Dash()
    {
        //animator
        canDash = false;
        isDashing = true;
        yield return new WaitForSeconds(0.1f);
        isDashing = false;
        yield return new WaitForSeconds(0.5f);
        canDash = true;
    }
}
