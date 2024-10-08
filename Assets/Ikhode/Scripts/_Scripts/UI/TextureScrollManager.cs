using UnityEngine;

public class TextureScrollManager : MonoBehaviour
{
    public float scrollSpeedX = 0.5f; // Speed of scrolling in the X direction
    public float scrollSpeedY = 0.5f; // Speed of scrolling in the Y direction

    private Vector2 _globalOffset;

    void Start()
    {
        _globalOffset = Vector2.zero;
    }

    void Update()
    {
        // Update global offset based on the scroll speed
        _globalOffset.x += scrollSpeedX * Time.deltaTime;
        _globalOffset.y += scrollSpeedY * Time.deltaTime;

        // Ensure the offset wraps around for a seamless loop
        _globalOffset.x %= 1;
        _globalOffset.y %= 1;
    }

    public Vector2 GetGlobalOffset()
    {
        return _globalOffset;
    }
}
