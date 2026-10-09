using UnityEngine;

/// <summary>
/// Handles scene rendering components for displaying generated textures and 3D terrain meshes.
/// </summary>
public class MapDisplay : MonoBehaviour
{
    /// <summary> Renderer for 2D plane texture previews (NoiseMap & ColourMap modes). </summary>
    public Renderer textureRender;

    /// <summary> MeshFilter target where generated 3D terrain geometry is assigned. </summary>
    public MeshFilter meshFilter;

    /// <summary> MeshRenderer target where terrain material and textures are assigned. </summary>
    public MeshRenderer meshRenderer;

    /// <summary>
    /// Assigns a 2D texture to the plane preview renderer and updates visibility states.
    /// </summary>
    /// <param name="texture">Target Texture2D to apply.</param>
    public void DrawTexture(Texture2D texture)
    {
        textureRender.sharedMaterial.mainTexture = texture;
        textureRender.transform.localScale = new Vector3(texture.width, 1, texture.height);

        // Toggle renderer visibility so 2D texture plane shows while 3D mesh is hidden
        if (meshRenderer != null) meshRenderer.gameObject.SetActive(false);
        if (textureRender != null) textureRender.gameObject.SetActive(true);
    }

    /// <summary>
    /// Assigns a generated 3D mesh and texture map to the MeshFilter and MeshRenderer.
    /// </summary>
    /// <param name="meshData">Container holding constructed vertex and triangle data.</param>
    /// <param name="texture">Surface texture map to apply to the terrain material.</param>
    public void DrawMesh(MeshData meshData, Texture2D texture)
    {
        meshFilter.sharedMesh = meshData.CreateMesh();
        meshRenderer.sharedMaterial.mainTexture = texture;

        // Reset Transform scale to (1,1,1) to prevent 2D texture scaling from distorting 3D geometry
        meshFilter.transform.localScale = Vector3.one;

        // Toggle renderer visibility so 3D terrain mesh shows while 2D plane is hidden
        if (textureRender != null) textureRender.gameObject.SetActive(false);
        if (meshRenderer != null) meshRenderer.gameObject.SetActive(true);
    }
}