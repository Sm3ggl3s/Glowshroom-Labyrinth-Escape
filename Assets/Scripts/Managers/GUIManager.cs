using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GUIManager : MonoBehaviour {

    public static GUIManager Instance { get; private set; }

    [Header("UI Screens")]
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Slider progressSlider;

    [Header("UI Elements")]
    [SerializeField] private GameObject healthBarUI;
    [SerializeField] private Slider healthBarSlider;
    [SerializeField] private TMP_Text healthBarText;

    private void Awake() {
        // Ensure there's only one GUIManager in the scene
        if (Instance != null && Instance != this) {
            Destroy(gameObject); 
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // Persist between scenes

        // Initially hide the health bar
        if (healthBarUI != null)
            healthBarUI.SetActive(false);
    }

    // Show the loading screen
    public void ShowLoadingScreen() {
        if (loadingScreen != null) {
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



}
