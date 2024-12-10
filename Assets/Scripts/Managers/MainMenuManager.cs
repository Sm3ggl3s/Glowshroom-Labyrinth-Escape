using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour{

    [Header("UI Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject creditsPanel;
    [SerializeField] private GameObject instructionsPanel;

    private void Start() {
        // Show the main menu panel
        mainMenuPanel.SetActive(true);
        creditsPanel.SetActive(false);
        instructionsPanel.SetActive(false);
    }

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

    // Show credits
    public void ShowCredits() {
        mainMenuPanel.SetActive(false);
        creditsPanel.SetActive(true);
    }

    // Show instructions
    public void ShowInstructions() {
        mainMenuPanel.SetActive(false);
        instructionsPanel.SetActive(true);
    }

    // Return to main menu
    public void ReturnToMainMenu() {
        mainMenuPanel.SetActive(true);
        creditsPanel.SetActive(false);
        instructionsPanel.SetActive(false);
    }

}
