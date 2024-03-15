using UnityEngine;
using NaughtyAttributes;
using System.Collections.Generic;

namespace Scripts.Generation
{
    public class MeshGenerator : MonoBehaviour
    {
        [System.Serializable]
        public struct TerrainMesh
        {
            public Vector3[] vertices;
            public Vector3[] triangles;
            public GameObject gameObject;
            public Transform transform;

            public TerrainMesh(Vector3[] vertices, Vector3[] triangles, GameObject gameObject)
            {
                this.vertices = vertices;
                this.triangles = triangles;
                this.gameObject = gameObject;
                transform = gameObject.transform;
            }
        }

        [Min(1)] public Vector2 dimensions = new Vector2();
        [Min(1)] public Vector2 terrainGrids = new Vector2();
        public Material defaultMaterial;
        public TerrainMesh[] terrainMesh;
        private int[] trianglesToInt;

        [Button]
        public void GenerateMesh()
        {
            ClearMeshes();
            terrainMesh = new TerrainMesh[(int)(terrainGrids.x * terrainGrids.y)];

            for (int i = 0; i < terrainMesh.Length; i++)
            {
                Vector3[] vertices = new Vector3[((int)dimensions.x + 1) * ((int)dimensions.y + 1)];
                Vector3[] triangles = new Vector3[(int)(dimensions.x * dimensions.y * 2)];
                GameObject meshObject = new GameObject("Mesh Object", typeof(MeshFilter), typeof(MeshRenderer));
                Mesh generatedMesh = new Mesh();
                terrainMesh[i] = new TerrainMesh(vertices, triangles, meshObject);

                CalculateVertices(i);
                DrawTriangles(i);
                ConvertTrianglesToInt(i);

                generatedMesh.vertices = vertices;
                generatedMesh.triangles = trianglesToInt;
                generatedMesh.RecalculateNormals();
                terrainMesh[i].gameObject.GetComponent<MeshFilter>().mesh = generatedMesh;
                terrainMesh[i].gameObject.GetComponent<MeshRenderer>().material = defaultMaterial;               

                /*for (int x = 0; x < (int)(terrainGrids.x * dimensions.x); x += (int)dimensions.x)
                {
                    for (int z = 0; z < (int)(terrainGrids.y * dimensions.y); z += (int)dimensions.y)
                    {
                        meshGameObject.transform.position = new Vector3(x, 0, z);
                    }
                }
                */
            }

            /*foreach (TerrainMesh mesh in temporaryList)
            {
                for (int x = 0; x < (int)(terrainGrids.x * dimensions.x); x += (int)dimensions.x)
                {
                    mesh.transform.position = new Vector3(x, 0, 0);
                }

                temporaryList.Remove(mesh);
            }*/
        }

        private void CalculateVertices(int iteration)
        {
            for (int j = 0, x = 0; x <= dimensions.x; x++)
            {
                for (int z = 0;  z <= dimensions.y; z++)
                {
                    terrainMesh[iteration].vertices[j] = new Vector3(x, 0, z);
                    j++;
                }
            }
        }

        private void DrawTriangles(int iteration)
        {
            int vert = 0;
            int tris = 0;
            for (int x = 0; x < (int)dimensions.x; x++)
            {
                for (int z = 0; z < (int)dimensions.y; z++)
                {
                    terrainMesh[iteration].triangles[tris] = new Vector3(vert, vert + 1, vert + dimensions.x + 1);
                    terrainMesh[iteration].triangles[tris + 1] = new Vector3(vert + 1, vert + dimensions.x + 2, vert + dimensions.x + 1);
                    vert++;
                    tris += 2;
                }

                vert++;
            }
        }

        private void ConvertTrianglesToInt(int iteration)
        {
            trianglesToInt = new int[terrainMesh[iteration].triangles.Length * 3];
            int triangleCounter = 0;

            for (int k = 0; k < trianglesToInt.Length; k += 3)
            {
                trianglesToInt[k] = (int)terrainMesh[iteration].triangles[triangleCounter].x;
                trianglesToInt[k + 1] = (int)terrainMesh[iteration].triangles[triangleCounter].y;
                trianglesToInt[k + 2] = (int)terrainMesh[iteration].triangles[triangleCounter].z;
                triangleCounter++;
            }
        }

        [Button]
        public void ClearMeshes()
        {
            foreach (MeshFilter createdObjects in FindObjectsOfType<MeshFilter>())
            {
                DestroyImmediate(createdObjects.gameObject);
            }
        }
    }    
}
