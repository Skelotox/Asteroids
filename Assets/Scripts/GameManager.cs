using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Prefabs References")]
    [SerializeField] private Player player;
    [SerializeField] private Asteroid asteroidPrefab;
    [SerializeField] private DeathBall deathBallPrefab;

    [Header("UI Score Panel References")]
    [SerializeField] private TextMeshProUGUI scoreText; 
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private GameObject scorePanel;

    [Header("UI Lose Panel References")]
    [SerializeField] private GameObject losePanel;
    [SerializeField] private TextMeshProUGUI losingScore;

    [Header("Default UI References")]
    [SerializeField] private Image playerLifePrefab;
    [SerializeField] private GameObject playerLives;
    [SerializeField] private GameObject startText;
    [SerializeField] private GameObject menu;
    [SerializeField] private GameObject shadowPanel;
    [SerializeField] private Button pauseButton;


    [Header("Asteroid spawner variables")]
    [SerializeField] private float timeBeforeFirstSpawn = 0.2f;
    [SerializeField] private float timeBetweenSpawn = 2f;
    [SerializeField] private float spawnDistance = 15f;
    [SerializeField] private float trajectoryVariance = 20.0f;
    [SerializeField] private int minAsteroidSpawn = 1;
    [SerializeField] private int maxAsteroidSpawn = 4;


    [Header("Other variables")]
    [SerializeField] public int lives = 3;
    [SerializeField] private float blinkFreq = 0.65f;
    [SerializeField] private float timeFreq;
    [SerializeField] private bool isStarted;
    [SerializeField] private int spawnChanceDeathBall = 2;

    private int score;

    #region Start/Updates Methods
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        losePanel.SetActive(false);
        menu.SetActive(false);
        scorePanel.SetActive(false);
        
        player.enabled = false;
        timeFreq = blinkFreq;

        string[] splitScore = PlayerPrefs.GetString("HighScore").Split(char.Parse(","));
        if (splitScore.Length > 0)
        {

            for (int i = 0; i < splitScore.Length; i++)
            {
                highScoreText.text += (i + 1) + ". " + splitScore[i] + "\n";
                if (i == 4)
                {
                    i = splitScore.Length;
                }
            }

        }

    }

    private void Update()
    {

        if (!isStarted)
        {
            if(timeFreq> 0)
            {
                startText.transform.localScale = Vector3.Lerp(startText.transform.localScale, Vector3.one * 1.5f, blinkFreq * Time.deltaTime);

            }
            else if (timeFreq < -blinkFreq)
            {
                timeFreq = blinkFreq;
            }
            else
            {
                startText.transform.localScale = Vector3.Lerp(startText.transform.localScale, Vector3.one * 0.75f, blinkFreq * Time.deltaTime);
            }
            timeFreq -= Time.deltaTime;

        }
    }
    #endregion




    #region Spawning Methods
    private void SpawnAsteroid()
    {
        int asteroidNumber = Random.Range(minAsteroidSpawn, maxAsteroidSpawn);

        for (int i = 0; i < asteroidNumber; i++)
        {
            Vector3 spawnPoint = Random.insideUnitCircle.normalized* spawnDistance;
            float variance = Random.Range(-trajectoryVariance, trajectoryVariance);
            Quaternion asteroidRotation = Quaternion.AngleAxis(variance, Vector3.forward);

            Asteroid asteroid = Instantiate(asteroidPrefab, spawnPoint, asteroidRotation);
            asteroid.SetTrajectory(asteroidRotation * -spawnPoint);

        }
    }

    private void SpawnDeathBall()
    {
        int spawnChance = Random.Range(0, 100);

        if (spawnChance< spawnChanceDeathBall)
        {
            Vector3 spawnPoint = Random.insideUnitCircle.normalized * spawnDistance;
            float variance = Random.Range(-trajectoryVariance, trajectoryVariance);
            Quaternion deathBallRotation = Quaternion.AngleAxis(variance, Vector3.forward);

            DeathBall deathBall = Instantiate(deathBallPrefab, spawnPoint, deathBallRotation);
            deathBall.SetTrajectory(deathBallRotation * -spawnPoint);

        }
    }
    #endregion




    public void increaseScore(int points)
    {
        score+= points;
        
        scoreText.text = score.ToString();
    }




    public void LoseLife()
    {
        lives--;
        Destroy(playerLives.transform.GetChild(playerLives.transform.childCount - 1).gameObject);
    }



    #region Call to Action
    public void Play()
    {
        Time.timeScale = 1.0f;
        shadowPanel.SetActive(false);
        menu.SetActive(false);
    }

    public void Pause()
    {
        Time.timeScale = 0.0f;
        shadowPanel.SetActive(true);
        scorePanel.SetActive(false);
        menu.SetActive(true);
    }

    public void Score()
    {
        scorePanel.SetActive(true);
        menu.SetActive(false);
        
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void NewGame()
    {
        Time.timeScale = 1.0f;
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
    #endregion




    #region Start/Lose Game
    public void StartGame()
    {
        if (!isStarted)
        {
            StopAllCoroutines();
            Destroy(startText);
            InvokeRepeating(nameof(SpawnAsteroid), timeBeforeFirstSpawn, timeBetweenSpawn);
            InvokeRepeating(nameof(SpawnDeathBall), timeBeforeFirstSpawn, timeBetweenSpawn);
            for (int i = 0; i < lives; i++)
            {
                Image playerLife = Instantiate(playerLifePrefab);
                playerLife.transform.SetParent(playerLives.transform);
            }
            isStarted= true;
            player.enabled = true;
        }
    }

    public void LoseGame()
    {
        string[] splitScore = PlayerPrefs.GetString("HighScore").Split(char.Parse(","));
        if(splitScore.Length > 0 )
        {
            PlayerPrefs.SetString("HighScore", PlayerPrefs.GetString("HighScore") + "," + score.ToString());
        }
        else
        {
            PlayerPrefs.SetString("HighScore", PlayerPrefs.GetString("HighScore") + score.ToString());

        }
        
        Destroy(player.gameObject);
        StopAllCoroutines();
        losePanel.SetActive(true);
        pauseButton.enabled = false;
        losingScore.SetText("Your score is : "+score);
        losingScore.enabled = true;
        player.enabled = false;
        
    }
    #endregion

}
