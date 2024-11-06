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

    private void Awake() {
        _controller = GetComponent<CharacterController>();
    }

    private void Update() {
        PlayerGravity();

        if (_input.sqrMagnitude == 0) {
            _controller.Move(Vector3.zero);
            return;
        }

        // Normalize input vector to standardize movement speed
        _direction.Normalize();
        _direction *= moveSpeed;

        // Move the player
        _controller.Move(_direction * Time.deltaTime);

        // Face player along movement vector
        Quaternion targetRotation = Quaternion.LookRotation(_direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
    }

    private void PlayerGravity() {
        if (_controller.isGrounded && _velocity < 0.0f)
        {
            _velocity = -1.0f;
        }
        else
        {
            _velocity += _gravity * gravityMultiplier * Time.deltaTime;
        }
        
        _direction.y = _velocity;
    }

    public void Move(InputAction.CallbackContext context) { 
        _input = context.ReadValue<Vector2>();
        _direction = new Vector3(_input.x, 0, _input.y);
    }
}
