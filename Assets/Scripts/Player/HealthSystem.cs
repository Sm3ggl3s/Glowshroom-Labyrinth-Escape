using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour {

    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f; // Maximum health
    [SerializeField] private float healthDecayDurationMinutes = 5f; // Time to fully deplete health in minutes
    private float currentHealth;
    private float healthDecayRate; // Decay per second (calculated)
    private bool isHealthDecaying = false;

    private void Start() {
        // Initialize health and decay rate
        currentHealth = maxHealth;
        float healthDecayDurationSeconds = healthDecayDurationMinutes * 60f;
        healthDecayRate = maxHealth / healthDecayDurationSeconds;

        // Update UI with initial values
        GUIManager.Instance.UpdateHealthBar(currentHealth, maxHealth);
    }

    public void StartHealthDecay() {
        if (!isHealthDecaying) {
            isHealthDecaying = true;
            StartCoroutine(DecreaseHealthOverTime());
        }
    }

    private IEnumerator DecreaseHealthOverTime() {
        while (currentHealth > 0) {
            // Check if the game is paused
            if (GUIManager.Instance != null && GUIManager.Instance.IsGamePaused) {
                yield return null; // Wait for the next frame without decreasing health
                continue; // Skip the rest of this iteration
            }

            currentHealth -= healthDecayRate * Time.deltaTime;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

            // Update the UI
            GUIManager.Instance.UpdateHealthBar(currentHealth, maxHealth);

            // Handle death condition
            if (currentHealth <= 0) {
                Debug.Log("Player has died.");
                HandlePlayerDeath();
                yield break;
            }

            yield return null;
        }
    }

    private void HandlePlayerDeath() {
        Debug.Log("Player has died.");
        GUIManager.Instance.ShowDeathScreen();
    }

    public void RestoreHealth(float amount) {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        GUIManager.Instance.UpdateHealthBar(currentHealth, maxHealth);
    }
}
