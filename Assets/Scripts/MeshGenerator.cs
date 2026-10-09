using UnityEngine;

/// <summary>
/// Static utility class responsible for converting 2D height arrays into 3D mesh geometry.
/// </summary>
public class MeshGenerator
{
    /// <summary>
    /// Generates 3D mesh data (vertices, triangles, UVs) from a 2D float array heightmap.
    /// </summary>
    /// <param name="heightMap">2D array containing normalized height values.</param>
    /// <param name="heightMultiplier">Vertical height scaling factor.</param>
    /// <param name="heightCurve">AnimationCurve to non-linearly adjust height distribution.</param>
    /// <param name="levelOfDetail">LOD index controlling vertex skip step size.</param>
    /// <param name="meshScale">Horizontal scale spacing factor between vertices.</param>
    /// <returns>Populated MeshData container ready for Unity Mesh conversion.</returns>
    public static MeshData GeneratorTerrainMesh(float[,] heightMap, float heightMultiplier, AnimationCurve heightCurve, int levelOfDetail, float meshScale)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        // Center offsets so mesh pivot rests at the origin (0,0)
        float topLeftX = (width - 1) / -2f;
        float topLeftZ = (height - 1) / -2f;

        // Calculate vertex step interval based on LOD (Level 0 = step 1, Level 1 = step 2, Level 2 = step 4, etc.)
        int meshSimplificationIncrement = (levelOfDetail == 0) ? 1 : levelOfDetail * 2;
        int verticesPerLine = (width - 1) / meshSimplificationIncrement + 1;

        MeshData meshData = new MeshData(verticesPerLine, verticesPerLine);
        int vertexIndex = 0;

        for (int y = 0; y < height; y += meshSimplificationIncrement)
        {
            for (int x = 0; x < width; x += meshSimplificationIncrement)
            {
                // Assign 3D position: X (scaled), Y (height evaluated through curve * multiplier * scale), Z (scaled)
                meshData.vertices[vertexIndex] = new Vector3(
                    (topLeftX + x) * meshScale,
                    heightCurve.Evaluate(heightMap[x, y]) * heightMultiplier * meshScale,
                    (topLeftZ - y) * meshScale
                );

                // Map 2D texture coordinates normalized between 0.0 and 1.0
                meshData.uvs[vertexIndex] = new Vector2(x / (float)width, y / (float)height);

                // Construct two triangles for each quad grid cell (excluding right and bottom border edges)
                if (x < width - 1 && y < height - 1)
                {
                    meshData.AddTriangle(vertexIndex, vertexIndex + verticesPerLine + 1, vertexIndex + verticesPerLine);
                    meshData.AddTriangle(vertexIndex + verticesPerLine + 1, vertexIndex, vertexIndex + 1);
                }

                vertexIndex++;
            }
        }

        return meshData;
    }
}

/// <summary>
/// Data container holding raw vertex positions, triangle indices, and UV coordinates 
/// for off-thread mesh construction.
/// </summary>
public class MeshData
{
    /// <summary> Array of 3D positions representing mesh vertices. </summary>
    public Vector3[] vertices;

    /// <summary> Array of vertex indices defining triangle connections in clock-wise winding order. </summary>
    public int[] triangles;

    /// <summary> Array of 2D UV texture mapping coordinates. </summary>
    public Vector2[] uvs;

    /// <summary> Internal tracker for writing to the triangles array sequentially. </summary>
    int triangleIndex;

    /// <summary>
    /// Allocates buffer arrays based on calculated vertex grid dimensions.
    /// </summary>
    /// <param name="meshWidth">Number of vertices across width.</param>
    /// <param name="meshHeight">Number of vertices across height.</param>
    public MeshData(int meshWidth, int meshHeight)
    {
        vertices = new Vector3[meshWidth * meshHeight];

        // Total quad cells = (width-1) * (height-1); each quad needs 2 triangles = 6 index entries
        triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
        uvs = new Vector2[meshWidth * meshHeight];
    }

    /// <summary>
    /// Adds a single triangle by storing 3 vertex indices and advancing the write index.
    /// </summary>
    /// <param name="a">Index of first vertex.</param>
    /// <param name="b">Index of second vertex.</param>
    /// <param name="c">Index of third vertex.</param>
    public void AddTriangle(int a, int b, int c)
    {
        triangles[triangleIndex] = a;
        triangles[triangleIndex + 1] = b;
        triangles[triangleIndex + 2] = c;
        triangleIndex += 3;
    }

    /// <summary>
    /// Instantiates a standard UnityEngine.Mesh and assigns the populated vertex buffers and normals.
    /// </summary>
    /// <returns>A fully initialized 3D Unity Mesh object.</returns>
    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        return mesh;
    }
}