using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using log4net.Util;

[CustomEditor(typeof(GenerateCone))]  
public class EditorConeTest : Editor
{
    /*  private void OnSceneGUI()
      {
          GenerateCone cone = (GenerateCone)target;

          Handles.color = Color.white;

          //Set le vecteur 3 d'ou se trouve le point final de notre hauteur
          cone.hauteurPoint = new Vector3(cone.transform.position.x, cone.hauteur, cone.transform.position.z);

          Handles.DrawLine(cone.transform.position, new Vector3(cone.transform.position.x, cone.transform.position.y + cone.hauteur, cone.transform.position.z));


          Handles.color = Color.yellow;

          //Créer un arc en fonction de notre point
          //Prob au vector 3 new Vector3
          Handles.DrawWireArc(cone.hauteurPoint, Vector3.up, new Vector3(0,0,1f), 360f, cone.radius);

          Handles.color = Color.red;
          //Draw une ligne du point d'origine vers l'extrémité du rayon gauche ou droit
          //Attribuer les points d'origines
          Vector3 calculateMinPoint = cone.hauteurPoint;

          //On veut les extrémités du cercle par rapport au radius
          cone.minPoint = calculateMinPoint.x - cone.radius;
          cone.maxPoint = calculateMinPoint.x + cone.radius;

          Handles.DrawLine(cone.transform.position, new Vector3(cone.minPoint, calculateMinPoint.y, calculateMinPoint.z));
          Handles.DrawLine(cone.transform.position, new Vector3(cone.maxPoint, calculateMinPoint.y, calculateMinPoint.z));

          //Set les points de distances équivalentes basées sur le minPoint et maxPoint
          // Une face en gros est un triangle, point d'origine à point 1,2, point d'origine à point 2,3...
          //
      }
    */

    /*private void OnSceneGUI()
    {
        GenerateCone cone = (GenerateCone)target;

        Handles.color = Color.white;

        Handles.DrawWireArc(cone.transform.position, Vector3.up, Vector3.forward, 360f, cone.radius);

        float rayon = cone.radius / 2;

        cone.hauteurPoint = new Vector3(cone.transform.position.x, cone.hauteur, cone.transform.position.z);
        Handles.DrawLine(cone.transform.position, cone.hauteurPoint);

        Handles.color = Color.red;
        Vector3 calculateMinPoint = cone.hauteurPoint;

        //On veut les extrémités du cercle par rapport au radius
        cone.minPoint = calculateMinPoint.x - cone.radius;
        cone.maxPoint = calculateMinPoint.x + cone.radius;

        Handles.DrawLine(cone.hauteurPoint, new Vector3(cone.minPoint, cone.transform.position.y, cone.transform.position.z));
        Handles.DrawLine(cone.hauteurPoint, new Vector3(cone.maxPoint, cone.transform.position.y, cone.transform.position.z));


        #region MathTest
        //calculer la longueur de l'hypothénus
        //BC² = AB² + AC²
        float hypothenus = Mathf.Pow(cone.hauteur, 2) + Mathf.Pow(cone.radius / 2, 2);
        //Debug.Log(hypothenus / 2);

        //calculer les angles du triangle
        //A = points d'origine, B = point hauteur, C point du côté
        //B = AC / AB -> long côté / l'angle du côté qu'on veut

        float bRadiant = rayon / cone.hauteur;

        //on multiplie le résultat par le max d'angle dans un triangle, et on divise par Mathf.PI
        float bAngle = Mathf.Atan(bRadiant) * 180 / Mathf.PI;

        //par exemple le cos(b) = b/c
        //cos(c) = c/
        float bCos = Mathf.Cos(bAngle);
        float bSin = Mathf.Sin(bAngle);
        //result 14° -> 90 + 17 = 104° donc l'angle C mesure 180 - 104 = 76°

        #endregion
    }
    */
}
