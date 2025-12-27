using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class droneEnemy : MonoBehaviour
{
    // Start is called before the first frame update
    public float speed=10f;
    public float life = 50;
    Rigidbody2D rb;
    public float MoveArea;
    private bool canMove=true;
    private bool canFlip = true;
    private Vector3 start;
    public CapsuleCollider2D col;
    public Animator animator;
    Vector2 save;
    public float n;
    private bool isDead = false;
    

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        start = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(canMove)
        {            
            if (Mathf.Abs(transform.position.x - start.x) > MoveArea&&canFlip)
            {
                Flip();
                canFlip = false;
            }
            if(Mathf.Abs(transform.position.x - start.x) <= MoveArea )
            {
                canFlip = true;
            }
            rb.velocity = new Vector2(transform.localScale.x * speed*-1, 0);
        }
        if (isDead)
        {
            rb.velocity = Vector2.zero;
            canMove = false;
            StartCoroutine(Die());
        }
    }
    void ApplyDamage(Vector2 direction)
    {
        life -= 25f;
        if (life <= 0)
        {
            isDead = true;
        }
        else
        {
            save = rb.velocity;
            canMove = false;
            rb.velocity = Vector2.zero;
            rb.velocity = direction * n;
            StartCoroutine(HittedCoolDown());
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            save = rb.velocity;
            rb.velocity = Vector2.zero; // ‘¬“x‚ð0‚ÉÝ’è
        }
    }

    void OnCollisionExit(Collision collision)
    {
        // Õ“Ë‚©‚ç—£‚ê‚½ŒãA•Û‘¶‚µ‚Ä‚¢‚½‘¬“x‚ðŒ³‚É–ß‚·
        if (collision.gameObject.tag == "Player")
        {
            rb.velocity = save;
        }
    }

    IEnumerator HittedCoolDown()
    {
        yield return new WaitForSeconds(0.1f);
        rb.velocity = save;
        canMove = true;
    }
    IEnumerator Die()
    {
        col.enabled = false;
        animator.SetBool("Died", true);
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
    void Flip()
    {
        Vector3 size = transform.localScale;
        size.x *= -1;
        transform.localScale = size;
    }
}
