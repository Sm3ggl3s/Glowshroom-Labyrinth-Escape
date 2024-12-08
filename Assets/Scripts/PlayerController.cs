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

    [Header("References")]
    [SerializeField] private Transform cameraTransform; // Reference to the camera transform

    private void Awake() {
        _controller = GetComponent<CharacterController>();
    }

    private void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private Vector3 _currentPosition;

    private void Update() {
        if (_input.sqrMagnitude > 0.01f) {
            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;

            forward.y = 0;
            right.y = 0;

            forward.Normalize();
            right.Normalize();

            // Calculate desired position
            Vector3 targetDirection = (forward * _input.y + right * _input.x).normalized * moveSpeed;

            // Smooth the movement direction
            _direction = Vector3.SmoothDamp(_direction, targetDirection, ref _currentPosition, 0.1f);

            // Move the player
            _controller.Move(_direction * Time.deltaTime);

            // Rotate the player
            Vector3 lookDirection = new Vector3(_direction.x, 0, _direction.z);
            if (lookDirection.sqrMagnitude > 0.01f) {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
            }
        }
    }



    public void Move(InputAction.CallbackContext context) { 
        _input = context.ReadValue<Vector2>();
    }
}
