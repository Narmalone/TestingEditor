using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Quaternion toRightRotation;
    [SerializeField] private Quaternion toLeftRotation;
    public bool rotationSens = false;

    private void Update()
    {
        if(rotationSens == false)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRightRotation, 0.1f);
            if (transform.rotation == toRightRotation) rotationSens = true;
        }
        else
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toLeftRotation, 0.1f);
            if (transform.rotation == toLeftRotation) rotationSens = false;
        }
    }
}
