using UnityEngine;

/// <summary>
/// Attached to the Map Generator GameObject. Handles terrain settings, 
/// coordinates Perlin noise generation, color mapping, and rendering mode delegation.
/// </summary>
public class MapGenerator : MonoBehaviour
{
    /// <summary>
    /// Specifies the target rendering output mode for the terrain visualization.
    /// </summary>
    public enum DrawMode
    {
        NoiseMap,  // Renders raw grayscale height values
        ColourMap, // Renders region-based colors according to height thresholds
        Mesh       // Renders a fully textured 3D terrain mesh
    };

    /// <summary> The selected display mode for map generation. </summary>
    public DrawMode drawMode;

    /// <summary> Side length (width and height) of a square terrain chunk in vertices (e.g., 241). </summary>
    public const int mapChunkSize = 241;

    /// <summary> 
    /// Level of Detail index (0 to 6). Determines mesh simplification step size. 
    /// Higher values skip vertices to reduce poly count.
    /// </summary>
    [Range(0, 6)]
    public int levelOfDetails;

    /// <summary> Zoom factor for the Perlin noise map. Smaller values zoom in; larger values zoom out. </summary>
    public float noiseScale;

    /// <summary> Vertical height scaling factor applied to the 3D terrain mesh. </summary>
    public float meshHeightMultiplier;

    /// <summary> Controls vertical height distribution (e.g., flattening water/plains and steepening mountains). </summary>
    public AnimationCurve meshHeightCurve;

    /// <summary> When enabled via custom editor scripts, automatically updates the preview when inspector values change. </summary>
    public bool autoUpdate;

    /// <summary> Number of layered noise passes (octaves) combined to form detail levels. </summary>
    public int octaves;

    /// <summary> Determines rate of decay for noise amplitude across successive octaves (0 to 1). </summary>
    [Range(0, 1)]
    public float persistance;

    /// <summary> Determines rate of increase for noise frequency across successive octaves (typically >= 1). </summary>
    public float lacunarity;

    /// <summary> Seed value for pseudo-random number generation to ensure reproducible maps. </summary>
    public int seed;

    /// <summary> World position offset to scroll across the infinite noise map. </summary>
    public Vector2 offset;

    /// <summary> Array defining height ranges and associated colors (e.g., water, sand, grass, snow). </summary>
    public TerrainType[] regions;

    /// <summary> Horizontal spacing multiplier scaling the 3D mesh dimensions to match world unit preferences. </summary>
    public float meshScale = 10f;

    /// <summary>
    /// Core pipeline function: generates noise heightmap, evaluates terrain regions, 
    /// builds color maps, and delegates rendering to MapDisplay based on DrawMode.
    /// </summary>
    public void GenerateMap()
    {
        // Generate raw float height values between 0.0 and 1.0
        float[,] noiseMap = Noise.GenerateNoiseMap(
            mapChunkSize, mapChunkSize, seed, noiseScale,
            octaves, persistance, lacunarity, offset
        );

        // Allocate a 1D pixel array for the output color texture
        Color[] colourMap = new Color[mapChunkSize * mapChunkSize];

        // Map height values to region colors
        for (int y = 0; y < mapChunkSize; y++)
        {
            for (int x = 0; x < mapChunkSize; x++)
            {
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < regions.Length; i++)
                {
                    if (currentHeight < regions[i].height)
                    {
                        // Convert 2D coordinates (x, y) to 1D index: y * width + x
                        colourMap[y * mapChunkSize + x] = regions[i].colour;
                        break;
                    }
                }
            }
        }

        // Locate display component in scene and trigger appropriate render mode
        MapDisplay mapDisplay = FindAnyObjectByType<MapDisplay>();
        if (drawMode == DrawMode.NoiseMap)
        {
            mapDisplay.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
        }
        else if (drawMode == DrawMode.ColourMap)
        {
            mapDisplay.DrawTexture(TextureGenerator.TextureFromColourMap(colourMap, mapChunkSize, mapChunkSize));
        }
        else if (drawMode == DrawMode.Mesh)
        {
            mapDisplay.DrawMesh(
                MeshGenerator.GeneratorTerrainMesh(noiseMap, meshHeightMultiplier, meshHeightCurve, levelOfDetails, meshScale),
                TextureGenerator.TextureFromColourMap(colourMap, mapChunkSize, mapChunkSize)
            );
        }
    }

    /// <summary>
    /// Editor callback triggered when inspector parameters change. Validates boundary limits.
    /// </summary>
    void OnValidate()
    {
        if (lacunarity < 1)
            lacunarity = 1;
        if (octaves < 0)
            octaves = 0;
    }
}

/// <summary>
/// Serializable structure matching a height threshold to a specific terrain color.
/// </summary>
[System.Serializable]
public struct TerrainType
{
    /// <summary> Identifier label for inspector organization (e.g., "Deep Water"). </summary>
    public string name;

    /// <summary> Upper height bound (0.0 to 1.0) for this region layer. </summary>
    public float height;

    /// <summary> Color assigned to terrain falling under this height threshold. </summary>
    public Color colour;
}