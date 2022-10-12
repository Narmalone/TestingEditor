using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(FieldOfView))]
public class EditorFieldOfView : Editor
{
    private void OnSceneGUI()
    {
        FieldOfView fow = (FieldOfView)target;

        //Cr�er un arc qui repr�sente le radius maximale du FOW
        Handles.color = Color.white;
        Handles.DrawWireArc(fow.transform.position, Vector3.up , Vector3.forward, 360f, fow.viewRadius);

        //Cr�ations des angles ou est contenu le FOW
        Vector3 viewAngleA = fow.DirFromAngle(-fow.viewAngle / 2, false);
        Vector3 viewAngleB = fow.DirFromAngle(fow.viewAngle / 2, false);

        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleA * fow.viewRadius);
        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleB * fow.viewRadius);

        Handles.color = Color.red;
        foreach(Transform visibleTarget in fow.visibleTargets)
        {
            Handles.DrawLine(fow.transform.position, visibleTarget.transform.position);
        }

    }
}
