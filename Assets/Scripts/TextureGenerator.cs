using UnityEngine;

/// <summary>
/// Static utility class that constructs Texture2D assets from color arrays and height maps.
/// </summary>
public static class TextureGenerator
{
    /// <summary>
    /// Creates a pixel-sharp Texture2D asset from a 1D array of Color values.
    /// </summary>
    /// <param name="colourMap">1D array containing pixel colors in row-major order.</param>
    /// <param name="width">Texture pixel width.</param>
    /// <param name="height">Texture pixel height.</param>
    /// <returns>Configured Texture2D object.</returns>
    public static Texture2D TextureFromColourMap(Color[] colourMap, int width, int height)
    {
        Texture2D texture = new Texture2D(width, height);
        texture.filterMode = FilterMode.Point; // Prevents blur; maintains sharp pixel boundaries
        texture.wrapMode = TextureWrapMode.Clamp; // Prevents edge texture bleeding
        texture.SetPixels(colourMap);
        texture.Apply();
        return texture;
    }

    /// <summary>
    /// Converts a 2D float heightmap array into a grayscale Texture2D asset.
    /// </summary>
    /// <param name="heightMap">2D array containing height values between 0.0 and 1.0.</param>
    /// <returns>Grayscale preview Texture2D object.</returns>
    public static Texture2D TextureFromHeightMap(float[,] heightMap)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        Color[] colourMap = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // Interpolate from black (0.0) to white (1.0) based on elevation value
                colourMap[y * width + x] = Color.Lerp(Color.black, Color.white, heightMap[x, y]);
            }
        }

        return TextureFromColourMap(colourMap, width, height);
    }
}