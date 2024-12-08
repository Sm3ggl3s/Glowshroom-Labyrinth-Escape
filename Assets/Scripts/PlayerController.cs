using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

    private void Awake() {
        _controller = GetComponent<CharacterController>();
    }

    private void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update() {
        PlayerGravity();

        if (_input.sqrMagnitude == 0) {
            _controller.Move(Vector3.zero);
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

        // Calculate movement direction based on input and camera orientation
        _direction = (forward * _input.y + right * _input.x).normalized * moveSpeed;

        // Apply gravity to the direction
        _direction.y = _velocity;

        // Move the player
        _controller.Move(_direction * Time.deltaTime);

        // Face the player in the direction of movement
        if (_direction.sqrMagnitude > 0.01f) {
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(_direction.x, 0, _direction.z));
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }
    }

    private void PlayerGravity() {
        if (_controller.isGrounded && _velocity < 0.0f) {
            _velocity = -1.0f;
        } else {
            _velocity += _gravity * gravityMultiplier * Time.deltaTime;
        }
    }

    public void Move(InputAction.CallbackContext context) { 
        _input = context.ReadValue<Vector2>();
    }
}
