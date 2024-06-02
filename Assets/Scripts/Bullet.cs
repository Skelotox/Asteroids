using UnityEditor.Rendering;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    [SerializeField] private float maxLifeTime = 3.0f;
    [SerializeField] private float BulletSpeed = 250.0f;

    private Rigidbody2D rb;
    

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }




    public void Project(Vector3 direction)
    {

        rb.AddForce(direction * BulletSpeed);
        Destroy(this.gameObject, maxLifeTime);
    }




    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Asteroid")
        {
            GameManager.instance.increaseScore((int)collision.gameObject.GetComponent<Asteroid>().type);
            Destroy(this.gameObject);
        }

        if (collision.tag == "Border")
        {
            
            Destroy(this.gameObject);
        }

    }
}
