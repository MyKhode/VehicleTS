using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LineNavigationRender : MonoBehaviour
{
    public float speed = 1f; // Speed of offset change
    public bool adjustX = true; // Determine whether to adjust X or Y
    public float offsetAdjustment = 0f; // Initial offset adjustment

    private DecalProjector _decalProjector;
    private float m_offsetX;
    private float m_offsetY;

    void Start()
    {
        _decalProjector = GetComponent<DecalProjector>();
        if (_decalProjector == null)
        {
            Debug.LogError("DecalProjector component is missing from this GameObject.");
        }
    }

    void Update()
    {
        if (_decalProjector == null)
        {
            return;
        }

        // Calculate the offset change based on speed and time
        float offsetChange = speed * Time.deltaTime;
        if (adjustX)
        {
            m_offsetX += offsetChange;
        }
        else
        {
            m_offsetY += offsetChange;
        }

        // Update the UV bias
        Vector2 uvBias = _decalProjector.uvBias;
        if (adjustX)
        {
            uvBias.x = m_offsetX + offsetAdjustment;
        }
        else
        {
            uvBias.y = m_offsetY + offsetAdjustment;
        }
        _decalProjector.uvBias = uvBias;

        // Optionally, you may need to force a refresh of the decal properties if necessary
        // _decalProjector.OnValidate(); // Uncomment if necessary
    }
}
