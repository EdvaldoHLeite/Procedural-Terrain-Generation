using UnityEngine;

public static class TextureGenerator
{
    /**
     * Generate the map with the set of colours
     */
    public static Texture2D TextureFromColourMap(Color[] colourMap, int width, int height)
    {
        Texture2D texture = new Texture2D(width, height);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(colourMap);
        texture.Apply();
        return texture;
    }

    /**
     * Generate the map with the height map from perlin noise
     */
    public static Texture2D TextureFromHeightMap(float[,] heightMap)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        Texture2D texture = new Texture2D(width, height, TextureFormat.ARGB32, false);

        Color[] colourMap = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                colourMap[y * width + x] = Color.Lerp(Color.black, Color.white, heightMap[x, y]); // Responsible to map those values bettween 0.0 and 1.0 to the gray pattern
            }
        }

        return TextureFromColourMap(colourMap, width, height);
    }
}
