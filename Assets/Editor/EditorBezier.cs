using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using PathCreation;
using PathCreationEditor;

[CustomEditor(typeof(IAFollowing))]
public class EditorBezier : Editor
{
    
    private void OnSceneGUI()
    {
        IAFollowing iAFollowing = (IAFollowing)target;

        Handles.color = Color.red;

        //Initialiser point 0
        DrawOnScene(true);

      
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        //Afficher un bouton / toggle qui permet d'activer des cubes à la places des gizmos
        //bezierPath.controlPoints[i] = Handles.FreeMoveHandle(bezierPath.controlPoints[i], Quaternion.identity, 0.5f, Vector3.zero, Handles.CubeCap);
        //delta = Handles.FreeMoveHandle(bezierPath.controlPoints[i], Quaternion.identity, 0.5f, Vector3.zero, Handles.CubeCap);

        //D'autres fonctions/paramètres à sérialiser?
        //Moyen de Save Path sur un ScriptableObject
        //Choisir si bésier ou pas
        if (GUILayout.Button("GeneratePath"))
        {
            GeneratePath();
        } 
        
        if (GUILayout.Button("Add WaypointToPath"))
        {
            AddWaypointToPath();
            GeneratePath();
        }
        
        if (GUILayout.Toggle(false, "CubeGizmos"))
        {
            
        }

    }
    public void DrawOnScene(bool canRead)

    {
        IAFollowing iAFollowing = (IAFollowing)target;
        if (canRead)
        {
            Vector3 newPos = new Vector3();

            for (int i = 0; i < iAFollowing.pointsPath.Count; ++i)
            {


                //rajouter un point entre le transform du personnage ainsi que le point 0 du tableau

                //DrawLines

                //Set Line Color
                Handles.color = Color.red;

                foreach (Vector3 vec in iAFollowing.pointsPath)
                {
                    //Pour ne pas dépassé de la liste
                    if (i < iAFollowing.pointsPath.Count -1)
                    {
                        Handles.DrawLine(iAFollowing.pointsPath[i], iAFollowing.pointsPath[i + 1]);
                       
                    }
                }

                if(i == 0)
                {

                    //float sinA = Mathf.Sin(angle);
                    //float cosA = Mathf.Cos(angle);

                    //float tangent = sinA / cosA;

                    //float degree = angle * Mathf.Rad2Deg;
                    //float tangent = Mathf.Tan(degree);

                    //tangent d'un point tan(a) équivaut à a/b ou ration entre sinus et cosinus tan(a) = sin(a)/cos(a)


                    //Handles.DoPositionHandle(new Vector3(tangent, iAFollowing.pointsPath[i].y, 0f), Quaternion.identity);
                    //Handles.DoPositionHandle(new Vector3(0f, iAFollowing.pointsPath[i].y, tangent), Quaternion.identity);
                    if (iAFollowing.toggleSphereHandle)
                    {
                        Handles.color = Color.blue;
                        newPos = Handles.FreeMoveHandle(iAFollowing.pointsPath[i], Quaternion.identity, 0.5f, Vector3.zero, Handles.SphereHandleCap);
                    }
                    else
                    {
                        Handles.DrawLine(iAFollowing.transform.position, iAFollowing.pointsPath[0]);

                        //calculer la distance entre A et B puis B et C
                        newPos = Handles.PositionHandle(iAFollowing.pointsPath[i], Quaternion.identity);

                        
                    }
                }
                else
                {
                  
                    if (iAFollowing.toggleSphereHandle)
                    {
                        Handles.color = Color.blue;
                        newPos = Handles.FreeMoveHandle(iAFollowing.pointsPath[i], Quaternion.identity, 0.5f, Vector3.zero, Handles.SphereHandleCap);
                    }
                    else
                    {
                        //Récupérer la position du point et y coller un Gizmo
                        newPos = Handles.PositionHandle(iAFollowing.pointsPath[i], Quaternion.identity);
                    }
                }

                //Soustraire la position au point que l'on est en train de bouger
                newPos -= iAFollowing.pointsPath[i];
                
                //Chercher le point que l'on bouge et ajouter sa nouvelle position
                iAFollowing.pointsPath[i] += newPos;

                //Fonction pour que la position du point déplacé s'update tout seul
                UpdateWaypointPosition(i, iAFollowing.pointsPath[i]);
            }
        }
    }
    public void TangentLines()
    {
    }

    /// <summary>
    /// Quand le joueur appuie sur le boutton Générer path ça va lui créer un truc Smooth
    /// Faire en sorte que déplace les gameobject ça update Tout
    /// </summary>
    /// <returns> la nouvelle Liste de GameObject généré </returns>
    public List<GameObject> GeneratePath()
    {
        IAFollowing iAFollowing = (IAFollowing)target;
        
        //Quand on recrée le path on le détruit par la copie de la liste allPointPath
        List<GameObject> oldPointPath = iAFollowing.allPointPath;

        //Si allPointPath a des waypoints
        if (oldPointPath.Count > 0)
        {
            foreach (GameObject go in iAFollowing.allPointPath)
            {
                DestroyImmediate(go);
            }
        }

        //Recréation de la liste de GameObject
        iAFollowing.allPointPath = new List<GameObject>();

        //Set l'index à 0
        int index = 0;

        //Attribuer la position des nouveau Waypoints en fonction de ceux placés par l'éditeur
        foreach (Vector3 Wp in iAFollowing.pointsPath)
        {
            //Créer le point
            GameObject newPoint = new GameObject();

            //Set le transform du nouvel object crée
            newPoint.transform.position = Wp;

            //Le renommer
            newPoint.name = "Waypoints: " + index;

            //Lui donner un Parent
            newPoint.transform.SetParent(iAFollowing.ParentWaypoints);

            //Incrémenter l'index pour le nom
            index++;

            //Ajouter le point à la liste
            iAFollowing.allPointPath.Add(newPoint);
        }

        return iAFollowing.allPointPath;
    }

    /// <summary>
    /// Fonction pour update la position du poinr déplacé
    /// paramètre le point déplacé à sa position depuis l'éditeur à l'espace 3D
    /// </summary>
    /// <param name="i"></param>
    /// <param name="currentPostition"></param>
    public void UpdateWaypointPosition(int i, Vector3 currentPostition)
    {
        IAFollowing iAFollowing = (IAFollowing)target;

        iAFollowing.allPointPath[i].transform.position = currentPostition;
    }

    public Vector3 AddWaypointToPath()
    {
        IAFollowing iAFollowing = (IAFollowing)target;

        Vector3 newPoint = Vector3.zero;

        iAFollowing.pointsPath.Add(newPoint);

        return newPoint;
    }
}
