using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    public Renderer textureRender;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;

    public void DrawTexture(Texture2D texture)
    {
        textureRender.sharedMaterial.mainTexture = texture;
        textureRender.transform.localScale = new Vector3(texture.width, 1, texture.height);

        // Disable mesh renderer if active
        if (meshRenderer != null) meshRenderer.gameObject.SetActive(false);
        if (textureRender != null) textureRender.gameObject.SetActive(true);
    }

    public void DrawMesh(MeshData meshData, Texture2D texture)
    {
        meshFilter.sharedMesh = meshData.CreateMesh();
        meshRenderer.sharedMaterial.mainTexture = texture;

        // Reset scale on mesh transform to avoid scaling distortion
        meshFilter.transform.localScale = Vector3.one;

        // Disable 2D texture render plane if active
        if (textureRender != null) textureRender.gameObject.SetActive(false);
        if (meshRenderer != null) meshRenderer.gameObject.SetActive(true);
    }
}
