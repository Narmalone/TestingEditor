using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    private Rigidbody rb;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Vector3 moveDir = new Vector3(0, 0, 0);
        if (Input.GetKey(KeyCode.Z))
        {
            moveDir.z = +1f;
        }

        if (Input.GetKey(KeyCode.S))
        {
            moveDir.z = -1f;
        }
        
        if (Input.GetKey(KeyCode.Q))
        {
            moveDir.x = -1f;
        }
        
        if (Input.GetKey(KeyCode.D))
        {
            moveDir.x = +1f;
        }

      
        transform.position += moveDir * 10f * Time.deltaTime;
    }
}
