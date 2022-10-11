using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class TestCombineMeshes : MonoBehaviour
{
    /*
     void Start()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        int i = 0;
        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);

            i++;
        }
        transform.GetComponent<MeshFilter>().mesh = new Mesh();
        transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
        transform.gameObject.SetActive(true);
    }
    */

    void Start() {
        Debug.Log("Starting...");

        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
        Mesh mesh = meshFilter.mesh;

        Vector3[] vertices = new Vector3[mesh.vertices.Length];
        System.Array.Copy(mesh.vertices, vertices, vertices.Length);

        for (int i = 0; i < vertices.Length; i++) {
            Vector3 rayDirection = -mesh.normals[i];

            RaycastHit hit;
            if ( Physics.Raycast( vertices[i], rayDirection, out hit, 100f ) ) {
                vertices[i] = hit.point * 2f;
            }
            else {
                vertices[i] = Vector3.zero;
            }
        }

        mesh.vertices = vertices;

        Debug.Log("Done. Vertices count " + vertices.Length);

        // mesh.RecalculateBounds();
        // mesh.RecalculateNormals();
        // mesh.RecalculateTangents();
    }

    
}
