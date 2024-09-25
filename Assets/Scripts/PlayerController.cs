using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {

    private Vector2 _input;
    private CharacterController _controller;
    public void Move(InputAction.CallbackContext context) { 
        _input = context.ReadValue<Vector2>();
        Debug.Log(_input);
    }
}
