using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButtonController : MonoBehaviour
{
    // Diese Methode wird aufgerufen, wenn der Play Button gedrückt wird
    public void OnPlayButtonPressed()
    {
        // Überprüfe, ob das Level 1 ist
        if (PlayerData.Instance.level == 1)
        {
            // Transition zur Szene "A"
            SceneManager.LoadScene("Tutorial - Portrait");
        }
        else
        {
            // Transition zur Szene "B"
            SceneManager.LoadScene("Old - Portrait");
        }
    }
}

