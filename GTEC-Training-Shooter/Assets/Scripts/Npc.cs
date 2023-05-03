using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Npc : MonoBehaviour
{
    private GameHandler gameHandler;
    public bool isEnemy;

    private void Start()
    {
        gameHandler = GameHandler.Instance;
    }

    private void OnMouseDown()
    {
        interact();
    }

    public void focus()
    {
        interact();
    }

    private void interact()
    {
        if (isEnemy)
        {
            Debug.Log("Enemy Shot!");
            gameHandler.IncreaseScore(100);
            gameHandler.currentEnemies--;
        } 
        else
        {
            Debug.Log("Civilian Shot!");
            gameHandler.IncreaseScore(-100);
        }

        // Deactivate instead of destroy so that EEG handler does not lose track of the object
        // Game object will be destroyed when GameHandler runs CleanUpTargets()
        gameObject.SetActive(false);
    }

    public void destroyMe()
    {
        Destroy(gameObject);
    }
}
