using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GUIManager : MonoBehaviour {

    public static GUIManager Instance { get; private set; }

    [Header("UI Screens")]
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Slider progressSlider;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject deathScreen;

    [Header("UI Elements")]
    [SerializeField] private GameObject healthBarUI;
    [SerializeField] private Slider healthBarSlider;
    [SerializeField] private TMP_Text healthBarText;

    private bool isGamePaused = false;

    // Public property to access the pause state
    public bool IsGamePaused => isGamePaused;   

    private void Awake() {
        // Ensure there's only one GUIManager in the scene
        if (Instance != null && Instance != this) {
            Destroy(gameObject); 
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // Persist between scenes

        // Initially hide the health bar and Pause Screen
        if (healthBarUI != null)
            healthBarUI.SetActive(false);
        if (pauseMenu != null)
            pauseMenu.SetActive(false);
    }

    private void Update() {
        // Toggle pause state when the "P" key is pressed
        if (Input.GetKeyDown(KeyCode.P)) {
            TogglePause();
        }
    }

    // Toggles the pause state and UI
    public void TogglePause() {
        isGamePaused = !isGamePaused;

        if (isGamePaused) {
            // Pause the game
            Time.timeScale = 0;
            if (pauseMenu != null) pauseMenu.SetActive(true);
            ShowCursor();
        } else {
            // Resume the game
            Time.timeScale = 1;
            if (pauseMenu != null) pauseMenu.SetActive(false);
            HideCursor();
        }
    }

    // Called by the Resume button in the pause menu
    public void ResumeGame() {
        isGamePaused = false;
        Time.timeScale = 1;
        if (pauseMenu != null) pauseMenu.SetActive(false);
        HideCursor();
    }
    // Called by the Quit button in the pause menu
    public void QuitToMainMenu() {
        ShowCursor();
        Time.timeScale = 1;
        isGamePaused = false;
        SceneManager.LoadScene("Main Menu");
    }

    // Show and unlock the cursor when paused
    private void ShowCursor() {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None; // Unlock the cursor
    }

    // Hide and lock the cursor when resumed
    private void HideCursor() {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor
    }

    // Show the loading screen
    public void ShowLoadingScreen() {
        if (loadingScreen != null) {
            HideCursor(); // Hide the cursor when loading
            loadingScreen.SetActive(true);
            if (progressSlider != null) {
                progressSlider.value = 0; // Reset the progress slider
            }
        }
    }

    // Update the loading screen progress
    public void UpdateLoadingProgress(float progress) {
        if (progressSlider != null) {
            progressSlider.value = Mathf.Clamp01(progress);
        }
    }

    // Hide the loading screen
    public void HideLoadingScreen() {
        if (loadingScreen != null) {
            loadingScreen.SetActive(false);
        }
    }

    // Show the health bar after the loading screen is hidden
    public void ShowHealthBarAfterLoading() {
        StartCoroutine(ShowHealthBarCoroutine());
    }

    private IEnumerator ShowHealthBarCoroutine() {
        // Wait until the loading screen is fully hidden
        while (loadingScreen.activeSelf) {
            yield return null; // Wait one frame
        }

        // Show the health bar
        ShowHealthBar();
    }

    // Show the health bar
    public void ShowHealthBar() {
        if (healthBarUI != null) {
            healthBarUI.SetActive(true);
        } else {
            Debug.LogError("Health bar reference is missing in GUIManager.");
        }
    }

    // Update the health bar
    public void UpdateHealthBar(float currentHealth, float maxHealth) {
        if (healthBarSlider != null) {
            healthBarSlider.maxValue = maxHealth;
            healthBarSlider.value = currentHealth;
        }

        if (healthBarText != null){
            healthBarText.text = $"{Mathf.Ceil(currentHealth)}";
        }
    }

    // Show the win screen and transition back to Main Menu
    public void ShowWinScreen() {
        if (winScreen != null) {
            winScreen.SetActive(true);
            StartCoroutine(ShowScreenAndReturnToMenu(winScreen));
        } else {
            Debug.LogError("Win screen reference is missing in GUIManager.");
        }
    }

    // Show the death screen and transition back to Main Menu
    public void ShowDeathScreen() {
        if (deathScreen != null) {
            deathScreen.SetActive(true);
            StartCoroutine(ShowScreenAndReturnToMenu(deathScreen));
        } else {
            Debug.LogError("Death screen reference is missing in GUIManager.");
        }
    }

    private IEnumerator ShowScreenAndReturnToMenu(GameObject screen) {
        yield return new WaitForSecondsRealtime(5f); // Wait for 5 seconds
        SceneManager.LoadScene("Main Menu");
        screen.SetActive(false);
    }



}
