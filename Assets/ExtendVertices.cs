using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using UnityEngine.Rendering;

public class ExtendVertices : MonoBehaviour
{
    [SerializeField] private MeshFilter m_thisMeshFilter;
    private Mesh mesh;

    [SerializeField] float x;
    [SerializeField] float y;
    [SerializeField] float z;

    [SerializeField] private int verticeCount = 0;
    Vector3[] vertices;

    [SerializeField] private bool mustResetOnNextStart = false;
    [SerializeField] private bool makeVerticesMove = false;
    private Vector3 vecPos;

    private void Awake()
    {
        m_thisMeshFilter = GetComponent<MeshFilter>();
        mesh = m_thisMeshFilter.sharedMesh;
        vertices = mesh.vertices;
    }
    private void Start()
    {

        if (mustResetOnNextStart)
        {
            ResetVertices();
        }
    }
    public void ResetVertices()
    {

        Vector3[] vertices = {
            new Vector3 (0, 0, 0),
            new Vector3 (1, 0, 0),
            new Vector3 (1, 1, 0),
            new Vector3 (0, 1, 0),
            new Vector3 (0, 1, 1),
            new Vector3 (1, 1, 1),
            new Vector3 (1, 0, 1),
            new Vector3 (0, 0, 1),
        };

        int[] triangles = {
            0, 2, 1, //face front
			0, 3, 2,
            2, 3, 4, //face top
			2, 4, 5,
            1, 2, 5, //face right
			1, 5, 6,
            0, 7, 4, //face left
			0, 4, 3,
            5, 4, 7, //face back
			5, 7, 6,
            0, 6, 7, //face bottom
			0, 1, 6
        };

        Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.Optimize();
        mesh.RecalculateNormals();


    }
    private void Update()
    {
        if (mustResetOnNextStart) return;

        if (makeVerticesMove)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                //Bouger les vertices
                vertices[i] += Vector3.up * Time.deltaTime;
                Debug.Log("update");
            }
            mesh.vertices = vertices;
            mesh.RecalculateBounds();
        }
    }

    private void OnDrawGizmosSelected()
    {
        m_thisMeshFilter = GetComponent<MeshFilter>();
        mesh = m_thisMeshFilter.sharedMesh;

        if (m_thisMeshFilter == null) return;
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawCube(mesh.vertices[0], new Vector3(x, y, z));
        Debug.Log("on draw gizmo");
    }
}
