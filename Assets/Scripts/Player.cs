using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{

    public static Player instance;

    [Header("GameObject Element")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Bullet bulletPrefab;

    [Header("Movement variables")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float rotationSpeed = 30f;

    [Header("Shoot variables")]
    [SerializeField] private float timeBeforeShootAgain = 0.5f;
    [SerializeField] private float resetTimer = 0.5f;

    [Header("Effects")]
    [SerializeField] private float blinkTime = 3f;
    [SerializeField] private float blinkFreq = 0.2f;

    

    private Vector2 smoothMvtInput;
    private Vector2 mvtInput;
    private Vector2 velocity;

    public bool isUpgraded;


    #region Start/Updates Methods
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        isUpgraded= false;
    }

    private void FixedUpdate()
    {
        SetPlayerVelocity();

        PlayerRotation();

        
    }

    private void Update()
    {
        timeBeforeShootAgain -= Time.deltaTime;
    }
    #endregion



    #region Player's Movement
    //Set a smooth movement for the player that is called in the 'Call to action" Move function
    private void SetPlayerVelocity()
    {
        smoothMvtInput = Vector2.SmoothDamp(
            smoothMvtInput,
            mvtInput,
            ref velocity,
            0.1f);
        rb.velocity = smoothMvtInput * moveSpeed;
    }

    private void PlayerRotation()
    {
        if(mvtInput != Vector2.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(transform.forward, smoothMvtInput);
            Quaternion rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            rb.MoveRotation(rotation);
        }
        
    }

    public void Move(InputAction.CallbackContext context)
    {
        mvtInput = context.ReadValue<Vector2>();
    }
    #endregion




    public void Shoot(InputAction.CallbackContext context)
    {
        if (timeBeforeShootAgain <= 0)
        {
            Bullet bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
            bullet.Project(transform.up);

            if (isUpgraded)
            {
                Quaternion targetRotation = Quaternion.Euler(30, 30, 160);
                //Debug.Log(this.gameObject.transform.localEulerAngles.z);
                if(transform.localEulerAngles.z > 180)
                {
                    bullet = Instantiate(bulletPrefab, transform.position, Quaternion.AngleAxis(-Vector3.Angle(new Vector3(0, 1, 0), transform.up) + 30, Vector3.forward));
                    bullet.Project(bullet.transform.up);

                    bullet = Instantiate(bulletPrefab, transform.position, Quaternion.AngleAxis(-Vector3.Angle(new Vector3(0, 1, 0), transform.up) - 30, Vector3.forward));
                    bullet.Project(bullet.transform.up);
                }
                else
                {
                    bullet = Instantiate(bulletPrefab, transform.position, Quaternion.AngleAxis(Vector3.Angle(new Vector3(0, 1, 0), transform.up) + 30, Vector3.forward));
                    bullet.Project(bullet.transform.up);

                    bullet = Instantiate(bulletPrefab, transform.position, Quaternion.AngleAxis(Vector3.Angle(new Vector3(0, 1, 0), transform.up) - 30, Vector3.forward));
                    bullet.Project(bullet.transform.up);
                }
                
                
            }
            timeBeforeShootAgain = resetTimer;
        }

    }




    private void PlayerCollision()
    {
        if(GameManager.instance.lives > 0)
        {
            this.gameObject.GetComponent<CapsuleCollider2D>().enabled = false;
            StartCoroutine(Blink(blinkTime));
            isUpgraded = false;
            GameManager.instance.LoseLife();
        }
        else
        {
            GameManager.instance.LoseGame();
        }
    }




    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Asteroid" || collision.tag == "Deathball" || collision.tag == "DeathBullet")
        {
            PlayerCollision();
        }
    }




    IEnumerator Blink(float waitTime)
    {
        while(waitTime> 0)
        {
            this.gameObject.GetComponent<Renderer>().enabled = false;
            yield return new WaitForSeconds(blinkFreq);
            waitTime -= blinkFreq;
            this.gameObject.GetComponent<Renderer>().enabled = true;
            yield return new WaitForSeconds(blinkFreq);
            waitTime -= blinkFreq;
        }
        this.gameObject.GetComponent<Renderer>().enabled = true;
        this.gameObject.GetComponent<CapsuleCollider2D>().enabled = true;
        yield return null;

        
    }

}
