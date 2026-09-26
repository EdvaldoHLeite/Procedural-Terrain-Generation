using UnityEngine;

/**
 * Class attached to Map generation Object, 
 * Creates the Perlin Noise Map and draws that
 */
public class MapGenerator : MonoBehaviour
{
    public int mapWidth;
    public int mapHeight;
    public float noiseScale;
    public bool autoUpdate;
    public int octaves;
    [Range(0,1)]
    public float persistance;
    public float lacunarity;
    public int seed;
    public Vector2 offset;

    public void GenerateMap()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed, noiseScale, octaves, persistance, lacunarity, offset);

        MapDisplay mapDisplay = FindAnyObjectByType<MapDisplay>();
        mapDisplay.DrawNoiseMap(noiseMap);
    }

    void OnValidate()
    {
        if (mapWidth < 1)
        {
            mapWidth = 1;
        } 
        if (mapHeight < 1)
        {
            mapWidth = 1;
        }
        if (lacunarity < 1)
            lacunarity = 1;
        if (octaves < 0)
            octaves = 0;
    }
}
