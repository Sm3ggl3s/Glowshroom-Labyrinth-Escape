using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour {

    public static GUIManager Instance { get; private set; }

    [Header("UI Screens")]
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Slider progressSlider;

    private void Awake() {
        // Ensure there's only one GUIManager in the scene
        if (Instance != null && Instance != this) {
            Destroy(gameObject); 
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // Persist between scenes
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



}
