using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TakeObject : MonoBehaviour
{

    private bool objectHasHand = false;
    private GameObject objecttaken;
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log("ontriggerstay");
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (objectHasHand) return;
            Debug.Log("à un gameobject");
            objectHasHand = true;
            other.gameObject.transform.parent = transform;
            objecttaken = other.gameObject;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (objectHasHand)
            {
                objectHasHand = false;
                objecttaken.transform.parent = null;
            }
        }
    }
}
