using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class PlaneGenerator : MonoBehaviour
{
    Mesh myMesh;
    MeshFilter meshFilter;

    [SerializeField] Vector2 planeSize = new Vector2(1, 1);
    [SerializeField] int planeResolution = 1;

    List<Vector3> vertices;
    List<int> triangles;

    void Awake()
    {
        myMesh = new Mesh();
        meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = myMesh;
    }

    void Update()
    {
        planeResolution = Mathf.Clamp(planeResolution, 1, 50);

        GeneratePlane(planeSize, planeResolution);
        AssignMesh();
    }

    [ContextMenu("Generate Mesh")]
    void DoTheThing()
    {
        myMesh = new Mesh();
        meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = myMesh;

        planeResolution = Mathf.Clamp(planeResolution, 1, 50);

        GeneratePlane(planeSize, planeResolution);
        AssignMesh();
    }

    void GeneratePlane(Vector2 size, int resolution)
    {
        vertices = new List<Vector3>();
        float xPerStep = size.x/resolution;
        float yPerStep = size.y/resolution;
        for (int y = 0; y <= resolution ; y++)
        {
            for (int x = 0; x <= resolution; x++)
            {
                // Center the plane by offsetting the vertex positions
                float xPos = (x * xPerStep) - (size.x / 2);
                float yPos = 0; // Keep the plane flat
                float zPos = (y * yPerStep) - (size.y / 2);
                vertices.Add(new Vector3(xPos, yPos, zPos));
            }
        }

        triangles = new List<int>();
        for (int row = 0; row < resolution; row++)
        {
            for (int column = 0; column < resolution; column++)
            {
                int i = (row * resolution) + row + column;
                triangles.Add(i);
                triangles.Add(i + (resolution) + 1);
                triangles.Add(i + (resolution) + 2);

                triangles.Add(i);
                triangles.Add(i + resolution + 2);
                triangles.Add(i + 1);
            }
        }
    }

    void AssignMesh()
    {
        myMesh.Clear();
        myMesh.vertices = vertices.ToArray();
        myMesh.triangles = triangles.ToArray();
    }
    void LeftToRightSine(float time)
    {

    }

    void RippleSine(float time)
    {

    }
}
