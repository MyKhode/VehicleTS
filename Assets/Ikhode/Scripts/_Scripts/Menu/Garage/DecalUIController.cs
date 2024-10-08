using UnityEngine;
using UnityEngine.UI;

public class DecalUIController : MonoBehaviour
{
    public GameObject DecalElementObject;
    public Transform decalParent;

    public VehicleData vehicleData;

    public Vector2 decalOffsetLeftDefault, decalOffsetRightDefault, decalOffsetTopDefault;
    public float decalWidthLeftDefault, decalHeightLeftDefault, decalWidthTopDefault;
    public float decalWidthRightDefault, decalHeightRightDefault, decalHeightTopDefault;

    public Button resetOffsetValueButton;
    public Toggle topDecalToggle, leftDecalToggle, rightDecalToggle;

    public Slider topOffsetXSlider, topOffsetYSlider, topWidthSlider, topHeightSlider;
    public Slider leftOffsetXSlider, leftOffsetYSlider, leftWidthSlider, leftHeightSlider;
    public Slider rightOffsetXSlider, rightOffsetYSlider, rightWidthSlider, rightHeightSlider;

    void Start()
    {
        InitializeSliders();
        InitializeToggles();
        resetOffsetValueButton.onClick.AddListener(ResetDecalValues);
        SetupListeners();
        LoadDecalImagesFromResources();

        // For debugging, force update sliders here
        topOffsetXSlider.value = vehicleData.Decal_Offset_Top.x;
        topOffsetYSlider.value = vehicleData.Decal_Offset_Top.y;
        topWidthSlider.value = vehicleData.Decal_Width_Top;
        topHeightSlider.value = vehicleData.Decal_Height_Top;
    }


    void InitializeSliders()
    {
        Debug.Log($"Setting Top Decal Offset X: {vehicleData.Decal_Offset_Top.x}");
        Debug.Log($"Setting Top Decal Offset Y: {vehicleData.Decal_Offset_Top.y}");

        topOffsetXSlider.value = vehicleData.Decal_Offset_Top.x;
        topOffsetYSlider.value = vehicleData.Decal_Offset_Top.y;
        topWidthSlider.value = vehicleData.Decal_Width_Top;
        topHeightSlider.value = vehicleData.Decal_Height_Top;

        leftOffsetXSlider.value = vehicleData.Decal_Offset_Left.x;
        leftOffsetYSlider.value = vehicleData.Decal_Offset_Left.y;
        leftWidthSlider.value = vehicleData.Decal_Width_Left;
        leftHeightSlider.value = vehicleData.Decal_Height_Left;

        rightOffsetXSlider.value = vehicleData.Decal_Offset_Right.x;
        rightOffsetYSlider.value = vehicleData.Decal_Offset_Right.y;
        rightWidthSlider.value = vehicleData.Decal_Width_Right;
        rightHeightSlider.value = vehicleData.Decal_Height_Right;
    }

    void InitializeToggles()
    {
        // Initialize toggles with the values from vehicleData
        topDecalToggle.isOn = vehicleData.DisplayDecalTop;
        leftDecalToggle.isOn = vehicleData.DisplayDecalLeft;
        rightDecalToggle.isOn = vehicleData.DisplayDecalRight;
    }

    void SetupListeners()
    {
        // Top decal listeners (Sliders)
        topOffsetXSlider.onValueChanged.AddListener(value =>
        {
            vehicleData.Decal_Offset_Top.x = value;
            vehicleData.OnDecalDataChanged.Invoke();
        });
        topOffsetYSlider.onValueChanged.AddListener(value =>
        {
            vehicleData.Decal_Offset_Top.y = value;
            vehicleData.OnDecalDataChanged.Invoke();
        });
        topWidthSlider.onValueChanged.AddListener(value =>
        {
            vehicleData.Decal_Width_Top = value;
            vehicleData.OnDecalDataChanged.Invoke();
        });
        topHeightSlider.onValueChanged.AddListener(value =>
        {
            vehicleData.Decal_Height_Top = value;
            vehicleData.OnDecalDataChanged.Invoke();
        });

        // Left decal listeners (Sliders)
        leftOffsetXSlider.onValueChanged.AddListener(value =>
        {
            vehicleData.Decal_Offset_Left.x = value;
            vehicleData.OnDecalDataChanged.Invoke();
        });
        leftOffsetYSlider.onValueChanged.AddListener(value =>
        {
            vehicleData.Decal_Offset_Left.y = value;
            vehicleData.OnDecalDataChanged.Invoke();
        });
        leftWidthSlider.onValueChanged.AddListener(value =>
        {
            vehicleData.Decal_Width_Left = value;
            vehicleData.OnDecalDataChanged.Invoke();
        });
        leftHeightSlider.onValueChanged.AddListener(value =>
        {
            vehicleData.Decal_Height_Left = value;
            vehicleData.OnDecalDataChanged.Invoke();
        });

        // Right decal listeners (Sliders)
        rightOffsetXSlider.onValueChanged.AddListener(value =>
        {
            vehicleData.Decal_Offset_Right.x = value;
            vehicleData.OnDecalDataChanged.Invoke();
        });
        rightOffsetYSlider.onValueChanged.AddListener(value =>
        {
            vehicleData.Decal_Offset_Right.y = value;
            vehicleData.OnDecalDataChanged.Invoke();
        });
        rightWidthSlider.onValueChanged.AddListener(value =>
        {
            vehicleData.Decal_Width_Right = value;
            vehicleData.OnDecalDataChanged.Invoke();
        });
        rightHeightSlider.onValueChanged.AddListener(value =>
        {
            vehicleData.Decal_Height_Right = value;
            vehicleData.OnDecalDataChanged.Invoke();
        });

        // Toggle listeners
        topDecalToggle.onValueChanged.AddListener(isOn =>
        {
            vehicleData.DisplayDecalTop = isOn;
            vehicleData.OnDecalDataChanged.Invoke(); // Notify that the display status has changed
        });

        leftDecalToggle.onValueChanged.AddListener(isOn =>
        {
            vehicleData.DisplayDecalLeft = isOn;
            vehicleData.OnDecalDataChanged.Invoke();
        });

        rightDecalToggle.onValueChanged.AddListener(isOn =>
        {
            vehicleData.DisplayDecalRight = isOn;
            vehicleData.OnDecalDataChanged.Invoke();
        });
    }

    void LoadDecalImagesFromResources()
    {
        // Load decals and instantiate UI elements in the decalParent
        Object[] decals = Resources.LoadAll("Decals", typeof(Texture2D));

        if (decals.Length > 0)
        {
            for (int i = 0; i < decals.Length; i++)
            {
                GameObject decalButton = Instantiate(DecalElementObject, decalParent);
                Texture2D decalTexture = (Texture2D)decals[i];

                Image thumbnail = decalButton.transform.Find("Thumbnail/DecalImage").GetComponent<Image>();
                thumbnail.sprite = Sprite.Create(decalTexture, new Rect(0, 0, decalTexture.width, decalTexture.height), Vector2.zero);

                Button loadTopButton = decalButton.transform.Find("Button/loadTop").GetComponent<Button>();
                Button loadSideButton = decalButton.transform.Find("Button/loadSide").GetComponent<Button>();

                int index = i; // Capture index for use in the listener
                loadTopButton.onClick.AddListener(() =>
                {
                    vehicleData.UpdateDecalTopURL("Decals/" + decals[index].name);
                });

                loadSideButton.onClick.AddListener(() =>
                {
                    vehicleData.UpdateDecalSideURL("Decals/" + decals[index].name);
                });
            }
        }
        else
        {
            Debug.LogWarning("No decals found in Resources/Decals.");
        }
    }

    void ResetDecalValues()
    {
        Debug.Log($"Resetting Top Decal Offset to: {decalOffsetTopDefault}");

        // Reset Top Decal
        vehicleData.Decal_Offset_Top = decalOffsetTopDefault;
        vehicleData.Decal_Width_Top = decalWidthTopDefault;
        vehicleData.Decal_Height_Top = decalHeightTopDefault;

        // Reset Left Decal
        vehicleData.Decal_Offset_Left = decalOffsetLeftDefault;
        vehicleData.Decal_Width_Left = decalWidthLeftDefault;
        vehicleData.Decal_Height_Left = decalHeightLeftDefault;

        // Reset Right Decal
        vehicleData.Decal_Offset_Right = decalOffsetRightDefault;
        vehicleData.Decal_Width_Right = decalWidthRightDefault;
        vehicleData.Decal_Height_Right = decalHeightRightDefault;

        InitializeSliders(); // Update UI sliders to reflect the reset values
        vehicleData.OnDecalDataChanged.Invoke(); // Notify that data has changed
    }

}
