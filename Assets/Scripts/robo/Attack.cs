using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public GameObject Beam;
    public Transform attackPoint;
    private Rigidbody2D rb;
    private Animator animator;
    public bool canAttack = true;
    public float ShotCoolDown = 0.25f;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C) && canAttack)
        {
            canAttack = false;
            animator.SetBool("IsShot", true);
            GameObject Spark = Instantiate(Beam, attackPoint.position, Quaternion.identity);
            Vector2 direction = new Vector2(transform.localScale.x, 0);
            Spark.GetComponent<BeamShot>().direction = direction;
            StartCoroutine(AttackCooldown());

        }

    }
    IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(ShotCoolDown);
        canAttack = true;
    }
}
