using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour{

    // Play game
    public void PlayGame() {
        // Load the next scene in the build index
        SceneManager.LoadScene("Main Level");
    }

    // Quit game
    public void QuitGame() {
        // For the editor
        Debug.Log("Game Quit");

        // For the build
        Application.Quit();
    }

}
