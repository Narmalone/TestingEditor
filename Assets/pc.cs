using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class pc : MonoBehaviour
{
    private InputActionAsset inputAsset;
    private InputActionMap player;
    private InputAction move;
    private CharacterController characterController;
    private Vector2 position;
    public float speed = 5f;
    void Awake()
    {
        inputAsset = GetComponent<PlayerInput>().actions;
        player = inputAsset.FindActionMap("Player");
        characterController = GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        player.FindAction("Move").started += DoMove;
        move = player.FindAction("Move");

        player.Enable();
    }

    private void OnDisable()
    {
        player.FindAction("Move").started -= DoMove;

        player.Disable();
    }
    private void DoMove(InputAction.CallbackContext obj)
    {
        position = obj.ReadValue<Vector2>();
        Debug.Log("do move");
    }
    private void Update()
    {
        transform.Translate(new Vector3(position.x, 0f, position.y) * speed * Time.deltaTime, Space.World);
    }
}
