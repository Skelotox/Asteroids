using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField] private List<Sprite> asteroidSprites = new List<Sprite>();
    [SerializeField] private int maxAsteroidSpawnFromDestroy = 3;
    [SerializeField] private Asteroid asteroidPrefab;
    [SerializeField] private float asteroidBaseSpeedMultiplier = 3f;
    [SerializeField] private float maxLifeTime = 25f;
    [SerializeField] private float trajectoryVariance = 180f;

    public AsteroidType type = AsteroidType.NONE;

    private bool isFromDestroy;

    #region Start/Updates Method
    private void Awake()
    {
        this.gameObject.GetComponent<SpriteRenderer>().sprite = asteroidSprites[UnityEngine.Random.Range(0, asteroidSprites.Count-1)]; //Randomize Asteroid sprite (on 4 different models)
    }

    private void Start()
    {
        if(type == AsteroidType.NONE)
        {
            type = AsteroidType.BIG;

        }
        Destroy(this.gameObject, maxLifeTime);
    }
    #endregion




    public void SetTrajectory(Vector3 direction)
    {
        if(!isFromDestroy)
        {
            this.GetComponent<Rigidbody2D>().AddForce(direction * asteroidBaseSpeedMultiplier);

        }
        else
        {
            this.GetComponent<Rigidbody2D>().AddForce(direction * asteroidBaseSpeedMultiplier * 3f);
        }
    }




    public void SpawnAsteroids()
    {
        if(!isFromDestroy)
        {
            for (int i = 0; i < maxAsteroidSpawnFromDestroy; i++)
            {
                Vector3 spawnPoint = this.transform.position;
                float variance = Random.Range(-trajectoryVariance, trajectoryVariance);
                Quaternion asteroidRotation = Quaternion.AngleAxis(variance, Vector3.forward);

                Asteroid asteroid = Instantiate(asteroidPrefab, spawnPoint, asteroidRotation);
                asteroid.type = AsteroidType.SMALL;
                asteroid.transform.localScale = asteroid.transform.localScale / 2;
                asteroid.isFromDestroy = true;
                asteroid.SetTrajectory(asteroidRotation * -spawnPoint);
                

            }

        }
    }




    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player" || collision.tag == "Bullet")
        {
            if(!isFromDestroy)
            {

                SpawnAsteroids();
                Destroy(this.gameObject);

            }
            else
            {
                Destroy(this.gameObject);
            }
        }
    }
}




public enum AsteroidType
{
    NONE,
    BIG = 3,
    SMALL = 5
}
