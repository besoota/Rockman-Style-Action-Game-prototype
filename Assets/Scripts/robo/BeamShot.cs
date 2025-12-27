using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamShot : MonoBehaviour
{
    public Vector2 direction;
    public bool hasHit = false;
    public float speed = 10f;
    private Vector3 startPoint;
    private Vector3 endPoint;
    Animator animator;
    public CapsuleCollider2D col;
    void Start()
    {
        startPoint = transform.position;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!hasHit)
        {
            GetComponent<Rigidbody2D>().velocity = direction * speed;
            transform.localScale = new Vector3(direction.x, 1, 1);
            if (Mathf.Abs(transform.position.x - startPoint.x) > 20.0)
            {
                Destroy(gameObject);
            }
        }
        if (hasHit )
        {
            StartCoroutine(Hit());
        }

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        hasHit = true;
        ContactPoint2D contact = collision.contacts[0];
        endPoint = contact.point;
        if (collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.SendMessage("ApplyDamage", direction);
        }
        
    }
    IEnumerator Hit()
    {
        col.enabled = false;
        transform.position = endPoint;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        animator.SetBool("IsHit", true);
        yield return new WaitForSeconds(0.583f);
        Destroy(gameObject);
    }
}
