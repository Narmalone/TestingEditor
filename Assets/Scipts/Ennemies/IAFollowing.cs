using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class IAFollowing : MonoBehaviour
{

    //Liste de path dans un ScriptableObject

    //Des choses pour débugs elles ne sont pas utilisées dans quoique ce soit dans ce script
    //Les points dans le mode 3D par l'éditorBezier
    public List<Vector3> pointsPath;
    [HideInInspector] public Vector3 pointSelected;

    [HideInInspector] public int indexPath = 0;
    [HideInInspector] public int currentIndexMove = 0;
    public bool toggleSphereHandle = false;

    float radius = 1f;
    [SerializeField] private float agentSpeed = 5f;

    [SerializeField, Tooltip("Mettre le Gameobject qui accueilleras tout le path de l'IA ")] public Transform ParentWaypoints;

    /// <summary>
    /// Liste des différents waypoints que l'entité va suivre
    /// </summary>
    [HideInInspector] public List<Transform> waypoints = new List<Transform>();

    /// <summary>
    /// Liste de tous les GameObjects générés par la fonction GeneratePath from editorBerizer
    /// </summary>
    [HideInInspector] public List<GameObject> allPointPath = new List<GameObject>();
    private void Start()
    {
        currentIndexMove = 0;
    }
    private void Update()
    {
        if (Vector3.Distance(transform.position, pointsPath[currentIndexMove]) < radius)
        {
            currentIndexMove++;

            if(currentIndexMove >= pointsPath.Count)
            {
                currentIndexMove = 0;
            }
        }
        transform.position = Vector3.MoveTowards(transform.position, pointsPath[currentIndexMove], agentSpeed * Time.deltaTime);

        //attention les childs dans allpoint path bougent aussi donc trouver une alternative
        transform.LookAt(allPointPath[currentIndexMove].transform);
    }

    public virtual void UpdateWaypoint(int i)
    {
        if (waypoints != null)
        {
            Debug.Log(waypoints[i]);
        }
    }

}
