using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameHandler : MonoBehaviour
{
    [Header("Enemy Spawning")]
    public List<GameObject> spawnables;
    public float distanceX;
    public float distanceZ;

    [Header("Enemies & Civilians")]
    public List<GameObject> currentlySpawned;
    public int currentEnemies;

    [Header("Menus & UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI finalScoreText;
    private int score = 0;

    public GameObject menuScreen;
    public GameObject hud;
    public GameObject gameOverScreen;

    [Header ("Timer")]
    public TextMeshProUGUI timerText;
    public float startTime;
    private float timeRemaining;
    private bool timerIsRunning = false;

    private static GameHandler instance;

    public static GameHandler Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Timer Code
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                timerText.text = "Time: " + Mathf.FloorToInt(timeRemaining).ToString();

                if (currentEnemies == 0)
                {
                    CleanUpTargets();
                    SetUpTargets();
                }
            }
            else
            {
                timeRemaining = 0;
                timerIsRunning = false;
                CleanUpTargets();
                GameOver();
            }
        }
    }

    // Screens
    public void StartGame()
    {
        // Reset Game
        timeRemaining = startTime;
        timerIsRunning = true;
        score = 0;
        scoreText.text = "Score: " + score.ToString();
    }

    public void SetUpTargets()
    {
        Spawn(spawnables[0], true); // Spawn 1 garaunteed enemy

        int targetAmount = Random.Range(0, 3); // Randomly Spawn More

        for (int i = 0; i < targetAmount; i++)
        {
            int enemyRange = Random.Range(0, 4);
            if (enemyRange < 2) // Spawn Enemy
            {
                Spawn(spawnables[enemyRange], true);
            }
            else // Spawn Civilian
            {
                Spawn(spawnables[enemyRange], false);
            }
        }
    }

    public void CleanUpTargets()
    {
        for (int i = currentlySpawned.Count - 1; i >= 0; i--)
        {
            if (currentlySpawned[i] != null)
            {
                Destroy(currentlySpawned[i]);
            }
            currentlySpawned.RemoveAt(i);
        }
    }

    public void GameOver()
    {
        hud.SetActive(false);
        finalScoreText.text = "Final Score: " + score.ToString();
        gameOverScreen.SetActive(true);
    }

    public void Spawn(GameObject target, bool isEnemy)
    {
        // choose a random position within the rectangular area defined by distanceX and distanceZ
        float xPos = Random.Range(-distanceX / 2, distanceX / 2) + transform.position.x;
        float zPos = Random.Range(-distanceZ / 2, distanceZ / 2) + transform.position.z;

        // create the object at the chosen position and rotation
        Vector3 spawnPos = new Vector3(xPos, 1.2f, zPos);
        Quaternion spawnRot = Quaternion.Euler(90f, 0f, 180f);

        if (isEnemy)
        {
            currentEnemies++;
        }

        currentlySpawned.Add(Instantiate(target, spawnPos, spawnRot));
    }

    public void IncreaseScore(int amount)
    {
        score += amount;
        scoreText.text = "Score: " + score.ToString();
    }

    private void OnDrawGizmosSelected()
    {
        // Draw a semitransparent red cube at the transforms position
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawCube(transform.position, new Vector3(distanceX, 1, distanceZ));
    }
}
