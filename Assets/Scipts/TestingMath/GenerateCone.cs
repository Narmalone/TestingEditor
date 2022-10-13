using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]  
[RequireComponent(typeof(MeshFilter))]  
public class GenerateCone : MonoBehaviour
{
    //Mesh
    private MeshFilter meshFilter;
    private Mesh mesh;

    //D�termin�e par la position de l'objet en Y -> donc depuis le vector3 orine point
    public float hauteur = 10f;

    //Prendre le point hauteur et le stocker dans une variable pour qu'on puisse le Get
    public float radius = 5f;

    public int segmentsNumber;

    //tableaux pour la cr�ation du mesh
    private Vector3[] vertices;
    private int[] triangles;
    private Vector2[] uv;

    [SerializeField] private Transform parentToRotate;
    [SerializeField] private bool mustUpdateMesh = false;

    private void Awake()
    {
        if (mustUpdateMesh) GetComponent<MeshFilter>().sharedMesh = GenerateConeMesh(segmentsNumber, radius, hauteur, vertices, triangles);

        parentToRotate.position = new Vector3(0, hauteur, 0);
        gameObject.transform.SetParent(parentToRotate);

    }

    private void Update()
    {
        if(!mustUpdateMesh) GetComponent<MeshFilter>().sharedMesh = GenerateConeMesh(segmentsNumber, radius, hauteur, vertices, triangles);

        //Merge les 2 codes FOW
    }
    private Mesh GenerateConeMesh(int subdivisions, float radius, float hauteur, Vector3[] vertices, int[] triangles)
    {
        //Cr�er le mesh
        Mesh mesh = new Mesh();
        mesh.name = "ConeMeshGenerated";

        //Vertex = numbre de segments + 2 car un segments � plusieurs Vertexs
        vertices = new Vector3[subdivisions + 2];

        //Set les uv si jamais texture
        uv = new Vector2[vertices.Length];
        uv[0] = new Vector2(0.5f, 0f);

        //Triangles pareils chacuns des triangles ont 3 vertices
        triangles = new int[(subdivisions * 2) * 3];

        //Le point d'origine ne bouge jamais donc vector0
        vertices[0] = Vector3.zero;

        //G�n�rer vertexs en fonctions des angles et du nombre de segments � cr�er
        for (int i = 0, n = subdivisions - 1; i < subdivisions; i++)
        {
            //Ratio correspond � l'avanc�e de i / par le nombre de subdivision -1
            float ratio = (float)i / n;

            //Utiliser ratio qui est une �quation qui �value a et b �cris a / b ou b n'�quivaut pas � 0
            float r = ratio * (Mathf.PI * 2f);

            //En calculant le ratio on va pouvoir s�parer chaques edges de mani�res �quivalentes (l� le placement des points)
            //� l'aide du cosinus et du sinus
            float x = Mathf.Cos(r) * radius;
            float z = Mathf.Sin(r) * radius;

            //ensuite on set la nouvelle position du point en fonction x et z
            vertices[i + 1] = new Vector3(x, 0f, z);
            
            //Pareil pour l'uv ou on l'update
            uv[i + 1] = new Vector2(ratio, 0f);
        }

        //On cr�er en gros une nouvelle edge qui repart du point d'origine jusqu'a hauteur
        vertices[subdivisions + 1] = new Vector3(0f, hauteur, 0f);

        //Pareil pour l'Uv on set nouvelle face
        uv[subdivisions + 1] = new Vector2(0.5f, 1f);

        //Construction de la face du bas

        for (int i = 0, n = subdivisions - 1; i < n; i++)
        {
            //Chaques triangles � 3 vertexs donc on multiplie par 3 et on vient cr�er le triangle de mani�re � faire 0,1,2 � chaque fois
            int offset = i * 3;
            triangles[offset] = 0;
            triangles[offset + 1] = i + 1;
            triangles[offset + 2] = i + 2;
        }

        //Construction des c�t�s
        //M�me chose que le dessus
        int bottomOffset = subdivisions * 3;
        for (int i = 0, n = subdivisions - 1; i < n; i++)
        {
            //Le changement est surtout ici
            int offset = i * 3 + bottomOffset;
            triangles[offset] = i + 1;
            triangles[offset + 1] = subdivisions + 1;
            triangles[offset + 2] = i + 2;
            Debug.Log(offset);
        }

        //R�atribution des donn�es du mesh
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        //Puis return vu que Mesh function
        return mesh;
    }

}
