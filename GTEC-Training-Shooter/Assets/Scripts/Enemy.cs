using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private GameHandler gameHandler;

    private void Start()
    {
        gameHandler = GameHandler.Instance;
    }

    private void OnMouseDown()
    {
        Debug.Log("Enemy Shot!");
        gameHandler.IncreaseScore(100);
        gameHandler.currentEnemies--;
        Destroy(gameObject);
    }
}
