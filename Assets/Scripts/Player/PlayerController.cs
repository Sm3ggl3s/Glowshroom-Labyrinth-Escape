using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour {

    [Header("Movement Settings")]
    private Vector2 _input;
    private CharacterController _controller;
    private Vector3 _direction;
    [SerializeField] private float moveSpeed = 5f;

    [Header("Rotation Settings")]
    [SerializeField] private float turnSpeed = 10f;

    [Header("Gravity Settings")]
    private float _gravity = -9.81f;
    [SerializeField] private float gravityMultiplier = 3.0f;
    private float _velocity;

    [Header("References")]
    [SerializeField] private Transform cameraTransform; // Reference to the camera transform


    [Header("Input Action Settings")]
    [SerializeField] private InputActionReference playerInputAction;
    
    private void Awake() {
        _controller = GetComponent<CharacterController>();

        // Automatically assign the main camera if not already set
        if (cameraTransform == null) {
            Camera mainCamera = Camera.main;
            if (mainCamera != null) {
                cameraTransform = mainCamera.transform;
            } else {
                Debug.LogWarning("No camera tagged as MainCamera found in the scene.");
            }
        }
    }

    private void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Ensure the player starts with a slight downward force for dropping
        if (!_controller.isGrounded) {
            _velocity = -2f; // A slight negative velocity to ensure initial descent
        }

        // Bind the Move input action from the PlayerInput component to the Move method
        if (playerInputAction != null && playerInputAction.action != null) {
            playerInputAction.action.performed += Move;
            playerInputAction.action.canceled += Move;
        }
    }

    private void Update() {
        // Check if the game is paused
        if (GUIManager.Instance != null && GUIManager.Instance.IsGamePaused) {
            // Stop movement input when the game is paused
            _input = Vector2.zero;  // Reset input to prevent continuous movement
            return;
        }

        PlayerGravity(); // Gravity is applied here

        if (_input.sqrMagnitude == 0) {
            _direction = Vector3.zero; // Stop horizontal movement completely
            _controller.Move(Vector3.zero); // Apply no movement when no input
            return;
        }

        // Adjust movement direction to align with camera's forward direction
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        // Ignore vertical component of camera direction
        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        // Calculate horizontal movement direction based on input and camera orientation
        _direction = (forward * _input.y + right * _input.x).normalized * moveSpeed;

        // Apply gravity to the vertical direction only
        _direction.y = _velocity;

        // Apply movement (only horizontal movement when input is present, and vertical movement due to gravity)
        _controller.Move(_direction * Time.deltaTime);

        // Face the player in the direction of movement
        if (_direction.sqrMagnitude > 0.01f) {
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(_direction.x, 0, _direction.z));
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }

        // Start health decay when the player moves
        if (_input.sqrMagnitude > 0) {
            var healthSystem = GetComponent<HealthSystem>();
            if (healthSystem != null) {
                healthSystem.StartHealthDecay();
            }
        }
    }

    private void PlayerGravity() {
        if (_controller.isGrounded && _velocity < 0.0f) {
            _velocity = -1.0f; // Reset to a small downward force when grounded
        } else {
            _velocity += _gravity * gravityMultiplier * Time.deltaTime; // Apply gravity force over time
        }

        // Apply gravity only on the y-axis, affecting vertical velocity
        _controller.Move(new Vector3(0, _velocity, 0) * Time.deltaTime);
    }


    public void Move(InputAction.CallbackContext context) { 
        _input = context.ReadValue<Vector2>();
    }

    private void OnDestroy() {
        // Unsubscribe from the input actions when this object is destroyed
        if (playerInputAction != null && playerInputAction.action != null) {
            playerInputAction.action.performed -= Move;
            playerInputAction.action.canceled -= Move;
        }
    }
}
