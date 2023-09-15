using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Enemy()
    { 

    }

    Animator animator;

    [SerializeField, Tooltip("How Much Health The Player Has")]
    public float health = 50f;

    [SerializeField, Tooltip("How long it takes to spawn this character")]
    public float spawnTime = 3f;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(float damage)
    {
        animator.SetTrigger("TakeDamage");
        health -= damage;
        Debug.Log(name + " Health: " + health);
        if(health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        GetComponent<Collider>().enabled = false;
        animator.SetTrigger("Die");
        Debug.Log(name + " Died!");
    }

    public void DestroyObject()
    {
        Destroy(gameObject);
    }

    public virtual void Attack()
    {
        // Add functionality to child scripts
    }
}
