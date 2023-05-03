using Gtec.UnityInterface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static Gtec.UnityInterface.BCIManager;

public class GameHandler : MonoBehaviour
{
    [Header("Enemy Spawning")]
    public List<Npc> spawnables;
    public float distanceX;
    public float distanceZ;

    [Header("Enemies & Civilians")]
    public List<Npc> currentlySpawned;

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

    [Header("EEG")]
    public ERPFlashController3D eRPFlashController3D;
    public bool useEEG;
    private uint _selectedClass = 0;
    private bool _update = false;

    private static GameHandler instance;

    public static GameHandler Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            if (useEEG)
            {
                // detach from class selection available event
                BCIManager.Instance.ClassSelectionAvailable -= OnClassSelectionAvailable;
            }

            Destroy(this.gameObject);
        }
        else
        {
            if (useEEG)
            {
                // attach to class selection available event
                BCIManager.Instance.ClassSelectionAvailable += OnClassSelectionAvailable;
            }

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

                if (useEEG && _update && _selectedClass >= 3 && _selectedClass - 3 < currentlySpawned.Count)
                {
                    // if using eeg && there is a new eeg signal && it is not detecting the dummy objects && it is a valid object
                    currentlySpawned[(int)(_selectedClass) - 3].focus();
                    _update = false;
                }

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

        // Reset EEG detector
        _selectedClass = 0;
        _update = false;
    }

    private void SetUpTargets()
    {
        Spawn(spawnables[0], true); // Spawn 1 guaranteed enemy

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

    private void CleanUpTargets()
    {
        for (int i = currentlySpawned.Count - 1; i >= 0; i--)
        {
            if (currentlySpawned[i] != null)
            {
                currentlySpawned[i].destroyMe();
            }
            currentlySpawned.RemoveAt(i);

            if (useEEG)
            {
                eRPFlashController3D.ApplicationObjects.RemoveAt(i + 3);
            }
        }
    }

    private void GameOver()
    {
        hud.SetActive(false);
        finalScoreText.text = "Final Score: " + score.ToString();
        gameOverScreen.SetActive(true);
    }

    private void Spawn(Npc target, bool isEnemy)
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

        Npc npc = Instantiate<Npc>(target, spawnPos, spawnRot);
        currentlySpawned.Add(npc);

        if (useEEG)
        {
            ERPFlashObject3D newFlashObj = new ERPFlashObject3D();
            newFlashObj.ClassId = currentlySpawned.Count;
            newFlashObj.DarkMaterial = npc.darkMaterial;
            newFlashObj.FlashMaterial = npc.flashMaterial;
            newFlashObj.GameObject = npc.gameObject;

            eRPFlashController3D.ApplicationObjects.Add(newFlashObj);
        }
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

    /// <summary>
    /// This event is called whenever a new class selection is available. Th
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnClassSelectionAvailable(object sender, System.EventArgs e)
    {
        ClassSelectionAvailableEventArgs ea = (ClassSelectionAvailableEventArgs)e;
        _selectedClass = ea.Class;
        _update = true;
        Debug.Log(string.Format("Selected class: {0}", ea.Class));
    }
}
