using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtendVertices : MonoBehaviour
{
    [SerializeField] private MeshFilter m_thisMeshFilter;
    private Mesh mesh;

    [SerializeField] float x;
    [SerializeField] float y;
    [SerializeField] float z;

    [SerializeField] private int verticeCount = 0;
    Vector3[] vertices;

    private Vector3 vecPos;

    private void Awake()
    {
        m_thisMeshFilter = GetComponent<MeshFilter>();
        mesh = m_thisMeshFilter.sharedMesh;
        vertices = mesh.vertices;
    }
    private void Start()
    {
        ExtendVertice();
    }
    public void ExtendVertice()
    {
    }

    private void Update()
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
