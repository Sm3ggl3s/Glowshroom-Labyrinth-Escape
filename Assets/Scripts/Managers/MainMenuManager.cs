using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour{

    // Play game
    public void PlayGame() {
        // Load the next scene in the build index
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex + 1);
    }

    // Quit game
    public void QuitGame() {
        // For the editor
        Debug.Log("Game Quit");

        // For the build
        Application.Quit();
    }

}
