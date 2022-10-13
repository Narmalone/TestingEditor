using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using log4net.Util;

[CustomEditor(typeof(IAFollowing))]
public class EditorBezier : Editor
{
    LineRenderer lineRenderer;
    private void OnSceneGUI()
    {
        IAFollowing iAFollowing = (IAFollowing)target;

        Handles.color = Color.red;

        //Initialiser point 0
        Handles.DrawBezier(iAFollowing.transform.position, iAFollowing.pointsPath[0], Vector3.zero, Vector3.zero, Color.red, null, 2f);

        for(int i = 1; i < iAFollowing.pointsPath.Length; i++)
        {
            Handles.DrawBezier(iAFollowing.pointsPath[0], iAFollowing.pointsPath[iAFollowing.indexPath], Vector3.zero, Vector3.zero, Color.white, null, 2f);

            //Cr�er un GameObject qui va permettre de controller les b�ziers en gros
            //Faire s'incr�menter les Rayons �mis des b�ziers trouver un moyen d'incr�menter indexpath et cr�ation de Gameobject sans �tre en scene GUI
            GameObject go = new GameObject();
            go.transform.position = iAFollowing.pointsPath[iAFollowing.indexPath];
        }
    }

    public void GetPoint(Vector3 pointPosition)
    {
        
    }
}
