using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{

    public static PlayerInput playerInput;
    private InputAction move;
    private InputAction shoot;

    public static bool isMouseLeftButtonPressed;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        move = playerInput.actions["Move"];
        shoot = playerInput.actions["Shoot"];
    }

    private void Update()
    {
        isMouseLeftButtonPressed = shoot.IsPressed();
    }
}
