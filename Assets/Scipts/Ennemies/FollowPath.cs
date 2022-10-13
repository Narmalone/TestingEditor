using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class FollowPath : MonoBehaviour
{
    public PathCreator creator;

    public float agentSpeed = 5f;

    float distanceTraveled;

    private void Update()
    {
        distanceTraveled += agentSpeed * Time.deltaTime;
        transform.position = creator.path.GetPointAtDistance(distanceTraveled);
        transform.rotation = creator.path.GetRotationAtDistance(distanceTraveled);
    }
}
