using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DeathBall : MonoBehaviour
{

    [SerializeField] private float deathBallBaseSpeedMultiplier = 3f;
    [SerializeField] private float maxLifeTime = 25f;
    [SerializeField] private DeathballBullet DeathballBulletPrefab;
    [SerializeField] private float shootChance = 0.1f;

    private void Start()
    {
        
        Destroy(this.gameObject, maxLifeTime);
    }




    private void Update()
    {
        float shoot = Random.Range(0f, 100f);
        if(shoot < shootChance && GameManager.instance.lives > 0)
        {
            Shoot();
        }
    }




    public void SetTrajectory(Vector3 direction)
    {
        this.GetComponent<Rigidbody2D>().AddForce(direction * deathBallBaseSpeedMultiplier);
        
    }




    public void Shoot()
    {
         DeathballBullet bullet = Instantiate(DeathballBulletPrefab, transform.position, transform.rotation);
         bullet.Project(Player.instance.transform.position);

    }




    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log(isFromDestroy);
        if (collision.tag == "Player")
        {
            
            Destroy(this.gameObject);
        }
        if (collision.tag == "Bullet")
        {
            Player.instance.isUpgraded = true;

            Destroy(this.gameObject);
        }
    }
}
