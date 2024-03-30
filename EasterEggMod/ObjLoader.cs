using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Globalization;

namespace Nova_Max.EasterEggMod
{
    public static class ObjLoader
    {
        public static Mesh LoadMeshFromObj(string filePath)
        {
            List<Vector3> originalVertices = new List<Vector3>();
            List<Vector2> originalUVs = new List<Vector2>();
            List<Vector3> originalNormals = new List<Vector3>();

            List<Vector3> processedVertices = new List<Vector3>();
            List<Vector2> processedUVs = new List<Vector2>();
            List<Vector3> processedNormals = new List<Vector3>();
            List<int> triangles = new List<int>();

            foreach (string line in File.ReadAllLines(filePath))
            {
                if (line.StartsWith("v "))
                {
                    var tokens = line.Split(' ');
                    var vertex = new Vector3(
                        float.Parse(tokens[1], CultureInfo.InvariantCulture),
                        float.Parse(tokens[2], CultureInfo.InvariantCulture),
                        float.Parse(tokens[3], CultureInfo.InvariantCulture));
                    originalVertices.Add(vertex);
                }
                else if (line.StartsWith("vt "))
                {
                    var tokens = line.Split(' ');
                    var uv = new Vector2(
                        float.Parse(tokens[1], CultureInfo.InvariantCulture),
                        float.Parse(tokens[2], CultureInfo.InvariantCulture));
                    originalUVs.Add(uv);
                }
                else if (line.StartsWith("vn "))
                {
                    var tokens = line.Split(' ');
                    var normal = new Vector3(
                        float.Parse(tokens[1], CultureInfo.InvariantCulture),
                        float.Parse(tokens[2], CultureInfo.InvariantCulture),
                        float.Parse(tokens[3], CultureInfo.InvariantCulture));
                    originalNormals.Add(normal);
                }
                else if (line.StartsWith("f "))
                {
                    var tokens = line.Split(' ');
                    var indices = new List<int>();
                    for (int i = 1; i < tokens.Length; i++)
                    {
                        var parts = tokens[i].Split('/');
                        var vertexIndex = int.Parse(parts[0]) - 1;
                        int uvIndex = parts.Length > 1 && parts[1] != "" ? int.Parse(parts[1]) - 1 : -1;
                        int normalIndex = parts.Length > 2 && parts[2] != "" ? int.Parse(parts[2]) - 1 : -1;

                        //Add vertex
                        processedVertices.Add(originalVertices[vertexIndex]);
                        indices.Add(processedVertices.Count - 1);

                        //Add UV if available
                        if (uvIndex != -1) processedUVs.Add(originalUVs[uvIndex]);

                        //Add normal if available
                        if (normalIndex != -1) processedNormals.Add(originalNormals[normalIndex]);
                    }

                    // Handle quad by creating two triangles
                    // Triangle 1
                    triangles.Add(indices[0]);
                    triangles.Add(indices[1]);
                    triangles.Add(indices[2]);
                    //If it's a quad, add another triangle
                    if (indices.Count == 4)
                    {
                        triangles.Add(indices[2]);
                        triangles.Add(indices[3]);
                        triangles.Add(indices[0]);
                    }
                }
            }

            Mesh mesh = new Mesh
            {
                vertices = processedVertices.ToArray(),
                uv = processedUVs.Count > 0 ? processedUVs.ToArray() : null,
                triangles = triangles.ToArray(),
            };

            if (processedNormals.Count > 0)
            {
                mesh.normals = processedNormals.ToArray();
            }
            else
            {
                mesh.RecalculateNormals();
            }

            return mesh;
        }
    }
}
