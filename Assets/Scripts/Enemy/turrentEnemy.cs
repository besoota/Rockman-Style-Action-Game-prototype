using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class turrentEnemy : MonoBehaviour
{
    public GameObject miniBeam;
    Animator animator;
    public Transform attackPoint;
    public float life = 25;
    public float cooldown = 5.0f;
    private float time = 0f;
    private bool isDead = false;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (isDead)
        {
            Destroy(gameObject);
        }
        if (time == cooldown)
        {
            GameObject beam = Instantiate(miniBeam, attackPoint.position, Quaternion.identity);
            beam.transform.localScale=transform.localScale;
            time = 0f;
        }
    }
    void ApplyDamage(Vector2 direction)
    {
        life -= 25f;
        if (life <= 0)
        {
            isDead = true;
        }
    }
}
