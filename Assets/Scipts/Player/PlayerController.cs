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
    [SerializeField] private Rigidbody hips;
    [SerializeField] private PhysicMaterial physicMat;

    [Header("Character's Variables")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpPower = 2f;

    [SerializeField] private Camera cam;
    //[SerializeField] private float strafeSpeed = 2f;

    private void Awake()
    {
        if(CharacterManager.instance != null)
        {
            if (CharacterManager.instance.EnableSplitScreen == false)
            {
                cam.gameObject.SetActive(false);
            }
        }
       
        playersInput = GetComponent<PlayerInput>();

        action = OnDeviceLost();
        allColiders = GetComponentsInChildren<Collider>();
    }
   
  
    private Action<PlayerInput> OnDeviceLost()
    {
        //Ui qui dit en gros ah le câble est débranché/connection perdue
        return action;
    }

    /// <summary>
    /// Récupérer le callback venant de l'inspecteur, fonction délegate appelée uniquement quand move est activée
    /// </summary>
    /// <param name="context"></param>
    public void Move(InputAction.CallbackContext context)
    {
        direction = context.ReadValue<Vector2>();
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (isGrounded)
        {
            hips.AddForce(0f, jumpPower, 0f, ForceMode.Impulse);
        }
    }

    /// <summary>
    /// Tout ce qui est physique dans cette update
    /// </summary>
    private void FixedUpdate()
    {
        //Pourquoi le forcemode force ne marche pas
        hips.AddForce(new Vector3(direction.x, 0f, direction.y) * speed, ForceMode.Impulse);
    }
}
