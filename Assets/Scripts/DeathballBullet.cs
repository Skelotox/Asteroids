using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathballBullet : MonoBehaviour
{
    [SerializeField] private float maxLifeTime = 5.0f;
    [SerializeField] private float BulletSpeed = 250.0f;

    private Rigidbody2D rb;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }




    public void Project(Vector2 direction)
    {
        rb.AddForce(direction * BulletSpeed);
        Destroy(this.gameObject, maxLifeTime);
    }




    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            
            
            Destroy(this.gameObject);
        }
        
    }
}
