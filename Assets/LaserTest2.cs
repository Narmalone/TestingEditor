using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]  
public class LaserTest2 : MonoBehaviour
{

    LineRenderer lineRenderer;

    [SerializeField] List<Collider> colliders = new List<Collider>();

    [SerializeField] private float maxRange = 15f;
    [SerializeField] private float maxLargeur = 2f;

    public MeshFilter filterChildren;
    private Mesh mesh;
    Vector3[] newVertices;
    Vector2[] newUV;
    int[] newTriangles;

    private Vector3 endRaycast;
    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();

        mesh = new Mesh();
        mesh.name = "LineRendererMesh";
        filterChildren.mesh = mesh;
    }

    //Le rayon part du milieu de l'objet si raycast ça va au milieu de l'objet vu que ce sont des statues
    private void Update()
    {
        Ray ray = new Ray(transform.position, new Vector3(transform.forward.x * maxRange, 0f, transform.forward.z * maxRange));

        Ray[] allRay = new Ray[colliders.Count];
        allRay[0] = ray;

        //Marche mais faire gaffe quand c'est à la fin
        //endRaycast = transform.position + ray.direction * maxRange;

        RaycastHit firstHit;
       
        //Si raycast du premier objet
        if (Physics.Raycast(allRay[0], out firstHit, maxRange))
        {
            Debug.DrawRay(transform.position, new Vector3(firstHit.point.x, 0f, firstHit.point.z));

            float dist = firstHit.distance;

            allRay[1] = new Ray(firstHit.point, new Vector3(firstHit.transform.forward.x * maxRange, 0f, firstHit.transform.forward.z * maxRange));


            Debug.DrawRay(firstHit.transform.position, new Vector3(firstHit.transform.forward.x * maxRange, 0f, firstHit.transform.forward.z * maxRange));

            RaycastHit secondHit;

            if (Physics.Raycast(allRay[1], out secondHit, maxRange))
            {
                Debug.Log(secondHit.collider.name);
                allRay[2] = new Ray(secondHit.point, new Vector3(secondHit.transform.forward.x * maxRange, 0f, secondHit.transform.forward.z * maxRange));
                RaycastHit thirdHit;
                Debug.DrawRay(secondHit.transform.position, new Vector3(secondHit.transform.forward.x * maxRange, 0f, secondHit.transform.forward.z * maxRange));


                if (Physics.Raycast(allRay[2], out thirdHit, maxRange))
                {
                    //allRay[3] = new Ray(thirdHit.point, new Vector3(thirdHit.transform.forward.x * maxRange, 0f, thirdHit.transform.forward.z * maxRange));
                    Debug.DrawRay(thirdHit.transform.position, new Vector3(thirdHit.transform.forward.x * maxRange, 0f, thirdHit.transform.forward.z * maxRange));
                    Debug.Log(thirdHit.collider.name);
                }

            }


            #region mesh
            //We need two arrays one to hold the vertices and one to hold the triangles
            Vector3[] VerteicesArray = new Vector3[4];
            int[] trianglesArray = new int[6];

            //lets add 3 vertices in the 3d space

            // Origine - Bas à droite
            VerteicesArray[0] = new Vector3(0 - maxLargeur, 0, 0);

            //haut à gauche
            VerteicesArray[1] = new Vector3(0 - maxLargeur, 0, 1 * dist);

            //bas à droite
            VerteicesArray[2] = new Vector3(maxLargeur, 0, 0);


            // --------------- TRIANGLE 2 --------------- \\

            //haut à droite
            VerteicesArray[3] = new Vector3(maxLargeur, 0, 1 * dist);


            //Triangles 2

            trianglesArray[3] = 3;
            trianglesArray[4] = 2;
            trianglesArray[5] = 1;


            //define the order in which the vertices in the VerteicesArray shoudl be used to draw the triangle
            //Triangle 1
            trianglesArray[0] = 0;
            trianglesArray[1] = 1;
            trianglesArray[2] = 2;
            /**/
            //add these two triangles to the mesh
            mesh.vertices = VerteicesArray;
            mesh.triangles = trianglesArray;

            mesh.RecalculateBounds();

            #endregion
        }
        else
        {
            //Rien n'est raycast
            Debug.DrawRay(transform.position, new Vector3(transform.forward.x * maxRange, 0f, transform.forward.z * maxRange));
        }

        GenerateMesh();
    }

    public void GenerateMesh()
    {
        
    }

    private Vector3 ReflectedVector(Vector3 fromDirection, Vector3 Normal)
    {
        Vector3 vecReflected = Vector3.Reflect(fromDirection, Normal);
        return vecReflected;
    }
}
