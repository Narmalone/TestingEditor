using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class GenerateCone : MonoBehaviour
{
    [SerializeField] private bool generateSmoothCube = false;
    #region trying easy cube
    Vector3[] vertices;
    int[] triangless;

    #endregion

    #region Variable SmoothCube
    private Vector3[] baseVertices = new Vector3[8];
    private Vector3[] finalVertices = new Vector3[24];

    private Mesh mesh;

    private int[] triangles = new int[36];

    private int[][] allFaces = new int[6][];

    private int faceNumber = 6;

    #endregion

    void Start()
    {
        if (!generateSmoothCube)
        {
            CreateNotSmoothCube();
        }
        else
        {
            mesh = GetComponent<MeshFilter>().mesh;
            GenerateSmoothCube();
        }
    }
    #region GenerateSmoothCube
    public void GenerateSmoothCube()
    {
        //Génération des sommets du cube
        baseVertices[0] = new Vector3(1, 1, 1);
        baseVertices[1] = new Vector3(-1, 1, 1);
        baseVertices[2] = new Vector3(-1, -1, 1);
        baseVertices[3] = new Vector3(1, -1, 1);
        baseVertices[4] = new Vector3(-1, 1, -1);
        baseVertices[5] = new Vector3(1, 1, -1);
        baseVertices[6] = new Vector3(1, -1, -1);
        baseVertices[7] = new Vector3(-1, -1, -1);

        //Stocker les indices de baseVertices par faces
        allFaces[0] = new int[4] { 0, 1, 2, 3 }; // Face arrière
        allFaces[1] = new int[4] { 5, 0, 3, 6 }; // Face droite
        allFaces[2] = new int[4] { 4, 5, 6, 7 }; // Face Avant
        allFaces[3] = new int[4] { 1, 4, 7, 2 }; // Face Gauche
        allFaces[4] = new int[4] { 5, 4, 1, 0 }; // Face Du dessus
        allFaces[5] = new int[4] { 3, 2, 7, 6 }; // Face du dessous


        int verticesByFace = 4;
        int verticesCount = 0;
        int trianglesCount = 0;

        for(int face = 0; face < faceNumber; face++)
        {
            triangles[trianglesCount + 0] = verticesCount;
            triangles[trianglesCount + 1] = verticesCount + 1;
            triangles[trianglesCount + 2] = verticesCount + 2;
            triangles[trianglesCount + 3] = verticesCount;
            triangles[trianglesCount + 4] = verticesCount + 2;
            triangles[trianglesCount + 5] = verticesCount + 3;

            trianglesCount += faceNumber;

            for(int vertex = 0; vertex < verticesByFace; vertex++)
            {
                Vector3 currPoint = baseVertices[allFaces[face][vertex]] * 0.5f;
                finalVertices[verticesCount] = currPoint;
                verticesCount++;
            }
        }

        UpdateMesh();

    }
    public void UpdateMesh()
    {
        mesh.vertices = finalVertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
    }

    #endregion

    private void CreateNotSmoothCube()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        Mesh mesh = meshFilter.mesh;

        //les 8 sommets d'une cube
        vertices = new[]{
            new Vector3 (0, 0, 0),
            new Vector3 (0, 1, 0),
            new Vector3 (1, 0, 0),
            new Vector3 (1, 1, 0),

            new Vector3 (1, 0, 1),
            new Vector3 (1, 1, 1),
            new Vector3 (0, 0, 1),
            new Vector3 (0, 1, 1),
        };

        mesh.vertices = vertices;
        //Cubes d'unity constitués de 12 triangles par pack de 3 car chaques triangles contient 3 vertices
        triangles = new[]{
            0,1,2,
            2,1,3,
            2,3,4,

            4,3,5,
            4,5,6,
            6,5,7,

            6,7,0,
            0,7,1,
            1,5,3,

            5,1,7,
            0,2,4,
            4,6,0
        };
        mesh.triangles = triangles;

    }

}
