using UnityEngine;
using UnityEngine.UI;

public class RemoveCanvasTextureBackground : MonoBehaviour
{
    public RawImage MinimapCanvasView;
    public Camera MinimapCamera;
    public Color BackgroundColor = Color.black;
    private RenderTexture renderTexture;

    void Start()
    {
        if (MinimapCanvasView == null)
        {
            Debug.LogError("MinimapCanvasView is not assigned!");
            return;
        }

        if (MinimapCamera == null)
        {
            Debug.LogError("MinimapCamera is not assigned!");
            return;
        }

        // Create a new RenderTexture
        renderTexture = new RenderTexture(55, 55, 24);
        MinimapCamera.targetTexture = renderTexture;

        // Set the RawImage to use the RenderTexture
        MinimapCanvasView.texture = renderTexture;

        // Set the camera's background color
        MinimapCamera.backgroundColor = BackgroundColor;
    }

    void LateUpdate()
    {
        RemoveBackground();
    }

    void RemoveBackground()
    {
        RenderTexture.active = renderTexture;
        Texture2D tex = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);
        tex.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        tex.Apply();

        Color[] pixels = tex.GetPixels();

        for (int i = 0; i < pixels.Length; i++)
        {
            if (ColorApproximately(pixels[i], BackgroundColor, 0.1f))
            {
                pixels[i] = Color.clear;
            }
        }

        tex.SetPixels(pixels);
        tex.Apply();

        MinimapCanvasView.texture = tex;
        RenderTexture.active = null;
    }

    bool ColorApproximately(Color a, Color b, float tolerance)
    {
        return Mathf.Abs(a.r - b.r) < tolerance &&
               Mathf.Abs(a.g - b.g) < tolerance &&
               Mathf.Abs(a.b - b.b) < tolerance;
    }
}
