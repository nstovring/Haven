using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshRenderer),typeof(MeshFilter))]
public class Lightshaft3D : MonoBehaviour
{
    private Vector3 start = new Vector3(0, 0, 0);

    private int resolution = 1;
    public float width = 0.25f;
    public float length = 1f;
    public float thickness = 0.002f;
    public bool closeMesh = false;
    public float uvScale = 0.5f;
    public float uvOffset = 0.5f;
    private GameObject sceneSunlight;
    private Vector3 sunVector;

    private void OnEnable()
    {
        var sceneLights = new Light[1];
        sceneLights = FindObjectsOfType<Light>();
        for (int i=0; i<sceneLights.Length;i++)
        {
            if (sceneLights[i].type==LightType.Directional)
            {
                sceneSunlight = sceneLights[i].gameObject;
            }
        }
        if (!Application.isEditor && Application.isPlaying && sceneSunlight != null)
        {
            GetComponent<MeshFilter>().mesh = CreateMesh();
        }
    }

    public void Update()
    {
        if (Application.isEditor && !Application.isPlaying && sceneSunlight != null)
        {
            GetComponent<MeshFilter>().mesh = CreateMesh();
        }
    }

    public Vector3 PointAlongSunVector(Vector3 startPoint, float length)
    {
        Vector3 endPoint;
        sunVector = (sceneSunlight.transform.rotation * Quaternion.Inverse(gameObject.transform.rotation)) * Vector3.forward;
        endPoint = startPoint + length * sunVector;
        return endPoint;
    }

    public Mesh CreateMesh()
    {
        Mesh mesh;

        mesh = new Mesh();

        float scaling = 1;
        float width = this.width / 2f;
        List<Vector3> verticesList = new List<Vector3>();
        List<int> triangleList = new List<int>();
        List<Vector2> uvList = new List<Vector2>();
        Vector3 upNormal = new Vector3(1, 0, 0);

        int s = 0;

            Vector3 segmentEnd = PointAlongSunVector( start, length);

            if (s == 0 || s == resolution - 1)
            sunVector.Normalize();
            Vector3 segmentRight = Vector3.Cross(upNormal, sunVector);

            var offset = new Vector3();
            if (closeMesh)
            {
                offset = segmentRight.normalized * (thickness / 2) * scaling;
                segmentRight *= thickness;
            }
            else
            {
                offset = Vector3.zero;
                segmentRight *= thickness;
            }
            Vector3 bottomRight = segmentRight + upNormal * width + offset;
            Vector3 topRight = segmentRight + upNormal * -width + offset;
            Vector3 bottomLeft = -segmentRight + upNormal * width + offset;
            Vector3 topLeft = -segmentRight + upNormal * -width + offset;

            int currentTriangleIndex = verticesList.Count;

            Vector3[] segmentVerts = new Vector3[]
            {
                start + bottomRight,
                start + bottomLeft,
                start + topLeft,
                start + topRight,
            };
            verticesList.AddRange(segmentVerts);

            Vector2[] uvs = new Vector2[]
            {
                new Vector2(0,1),
                new Vector2(1,1),
                new Vector2(0,1),
                new Vector2(1,1),
                new Vector2(0,0),
                new Vector2(1,0),
                new Vector2(0,0),
                new Vector2(1,0),
            };
            uvList.AddRange(uvs);

            int[] segmentTriangles = new int[1];

            if (closeMesh)
            {
                segmentTriangles = new int[]
                {
                    currentTriangleIndex + 6, currentTriangleIndex + 5, currentTriangleIndex + 1, //left face
				    currentTriangleIndex + 1, currentTriangleIndex + 2, currentTriangleIndex + 6,
                    currentTriangleIndex + 7, currentTriangleIndex + 3, currentTriangleIndex + 0, //right face
				    currentTriangleIndex + 0, currentTriangleIndex + 4, currentTriangleIndex + 7,
                    currentTriangleIndex + 1, currentTriangleIndex + 5, currentTriangleIndex + 4, //top face
				    currentTriangleIndex + 4, currentTriangleIndex + 0, currentTriangleIndex + 1,
                    currentTriangleIndex + 3, currentTriangleIndex + 7, currentTriangleIndex + 6, //bottom face
				    currentTriangleIndex + 6, currentTriangleIndex + 2, currentTriangleIndex + 3
                };
            }
            if (!closeMesh)
            {
                segmentTriangles = new int[]
                {
                        currentTriangleIndex + 1, currentTriangleIndex + 5, currentTriangleIndex + 4, //top face
				        currentTriangleIndex + 4, currentTriangleIndex + 0, currentTriangleIndex + 1,
                        currentTriangleIndex + 3, currentTriangleIndex + 7, currentTriangleIndex + 6, //bottom face
				        currentTriangleIndex + 6, currentTriangleIndex + 2, currentTriangleIndex + 3
                };
            }
            triangleList.AddRange(segmentTriangles);

        if (s == resolution - 1)
        {
            currentTriangleIndex = verticesList.Count;

            verticesList.AddRange(new Vector3[] {
                    segmentEnd + bottomRight,
                    segmentEnd + bottomLeft,
                    segmentEnd + topLeft,
                    segmentEnd + topRight
                });

        }

        mesh.vertices = verticesList.ToArray();
        mesh.triangles = triangleList.ToArray();
        mesh.uv = uvList.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        ;
        mesh.name = "Lightshaft";
        return mesh;
    }
}
