using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Npc : MonoBehaviour
{
    private GameHandler gameHandler;
    public bool isEnemy;

    public Material flashMaterial;
    public Material darkMaterial;

    public ParticleSystem explosionEffect;

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

        // Play Explosion Effect
        ParticleSystem newHitEffect = Instantiate(explosionEffect, transform.position, transform.rotation);
        newHitEffect.Play();
        Destroy(newHitEffect.gameObject, 1.5f);

        // Deactivate instead of destroy so that EEG handler does not lose track of the object
        // Game object will be destroyed when GameHandler runs CleanUpTargets()
        gameObject.SetActive(false);
    }

    public void SwapMaterial()
    {
        // swap the material of the object to flashMaterial or darkMaterial, whichever it is not currently using
        if (GetComponent<Renderer>().sharedMaterial.name.Equals(flashMaterial.name))
        {
            GetComponent<Renderer>().material = darkMaterial;
        }
        else
        {
            GetComponent<Renderer>().material = flashMaterial;
        }
    }
}
