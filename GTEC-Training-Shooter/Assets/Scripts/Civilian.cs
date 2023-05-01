using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Civilian : MonoBehaviour
{
    private GameHandler gameHandler;

    private void Start()
    {
        gameHandler = GameHandler.Instance;
    }

    private void OnMouseDown()
    {
        Debug.Log("Civilian Shot!");
        gameHandler.IncreaseScore(-100);
        Destroy(gameObject);
    }
}
