using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    private PlayerInput playersInput;
    private Action<PlayerInput> action;
    private Vector2 direction;

    private Collider[] allColiders;

    private bool isGrounded = true;

    [Header("References")]
    [SerializeField] private Rigidbody feet1;
    [SerializeField] private Rigidbody feet2;

    [Header("Character's Variables")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpPower = 2f;

    private Animator m_pc;

    //[SerializeField] private float strafeSpeed = 2f;

    private void Awake()
    {
        
        playersInput = GetComponent<PlayerInput>();
        m_pc = GetComponent<Animator>();
        action = OnDeviceLost();
        allColiders = GetComponentsInChildren<Collider>();
    }
   
  
    private Action<PlayerInput> OnDeviceLost()
    {
        //Ui qui dit en gros ah le c�ble est d�branch�/connection perdue
        return action;
    }

    /// <summary>
    /// R�cup�rer le callback venant de l'inspecteur, fonction d�legate appel�e uniquement quand move est activ�e
    /// </summary>
    /// <param name="context"></param>
    public void Move(InputAction.CallbackContext context)
    {
        direction = context.ReadValue<Vector2>();
        if (context.performed)
        {
        }
    }

    private void Update()
    {
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (isGrounded)
        {
            feet1.AddForce(0f, jumpPower, 0f, ForceMode.Force);
            feet2.AddForce(0f, jumpPower, 0f, ForceMode.Force);
        }
    }

    /// <summary>
    /// Tout ce qui est physique dans cette update
    /// </summary>
    private void FixedUpdate()
    {
        //Pourquoi le forcemode force ne marche pas
        //feet1.AddForce(new Vector3(direction.x * speed, 0f, direction.y * speed), ForceMode.VelocityChange);
        //feet2.AddForce(new Vector3(direction.x * speed, 0f, direction.y * speed), ForceMode.VelocityChange);
    }
}
