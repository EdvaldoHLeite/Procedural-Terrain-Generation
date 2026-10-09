using UnityEngine;
using System.Collections;

/// <summary>
/// Static library class providing multi-octave Perlin noise generation.
/// </summary>
public static class Noise
{
    /// <summary>
    /// Generates a normalized 2D array of noise values using layered Perlin octaves.
    /// </summary>
    /// <param name="mapWidth">Width of output grid in samples.</param>
    /// <param name="mapHeight">Height of output grid in samples.</param>
    /// <param name="seed">Random seed initializer.</param>
    /// <param name="scale">Base sampling zoom scale.</param>
    /// <param name="octaves">Number of detail layers to stack.</param>
    /// <param name="persistance">Amplitude reduction factor per octave layer.</param>
    /// <param name="lacunarity">Frequency multiplication factor per octave layer.</param>
    /// <param name="offset">Global coordinate offset translation.</param>
    /// <returns>2D float array containing normalized heightmap values between 0.0 and 1.0.</returns>
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];

        // Initialize pseudo-random generator with seed
        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];

        // Generate random sample offsets for each octave to prevent overlapping patterns
        for (int i = 0; i < octaves; i++)
        {
            // Range (-100000 to 100000) avoids Perlin noise symmetry/repetition around origin (0,0)
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) + offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        // Prevent division by zero
        if (scale <= 0)
        {
            scale = 0.0001f;
        }

        // Track minimum and maximum generated values for normalization
        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        // Center scaling zoom around center of the map
        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                // Stack octave layers
                for (int i = 0; i < octaves; i++)
                {
                    // Sample coordinates scaled by frequency and shifted by octave offsets
                    float sampleX = ((x - halfWidth) / scale - octaveOffsets[i].x) * frequency;
                    float sampleY = ((y - halfHeight) / scale - octaveOffsets[i].y) * frequency;

                    // Convert Unity Perlin range [0, 1] to [-1, 1] to allow negative elevation adjustments
                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                // Update min/max bounds for inverse lerp normalization
                if (noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }

                noiseMap[x, y] = noiseHeight;
            }
        }

        // Normalize all noise map values back into [0.0, 1.0] range
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
            }
        }

        return noiseMap;
    }
}