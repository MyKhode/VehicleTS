using UnityEngine;
using Unity.Netcode;

/// <summary>
/// Syncs player input and vehicle state over the network using Netcode for GameObjects (NGO).
/// </summary>
[RequireComponent(typeof(RCC_CarControllerV3))]
[AddComponentMenu("BoneCracker Games/Realistic Car Controller/Network/NGO/RCC Netcode Network")]
public class RCC_NetcodeNetwork : NetworkBehaviour
{
    private RCC_CarControllerV3 carController;
    private Rigidbody rigid;

    // Set permissions: Owner can write, and everyone can read
    private NetworkVariable<Vector3> correctPlayerPos = new NetworkVariable<Vector3>(
        default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private NetworkVariable<Quaternion> correctPlayerRot = new NetworkVariable<Quaternion>(
        default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private NetworkVariable<Vector3> currentVelocity = new NetworkVariable<Vector3>(
        default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private NetworkVariable<float> gasInput = new NetworkVariable<float>(
        default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private NetworkVariable<float> brakeInput = new NetworkVariable<float>(
        default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private NetworkVariable<float> steerInput = new NetworkVariable<float>(
        default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private NetworkVariable<float> handbrakeInput = new NetworkVariable<float>(
        default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private NetworkVariable<float> boostInput = new NetworkVariable<float>(
        default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private NetworkVariable<float> clutchInput = new NetworkVariable<float>(
        default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private NetworkVariable<int> gear = new NetworkVariable<int>(
        default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private NetworkVariable<int> direction = new NetworkVariable<int>(
        default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private NetworkVariable<bool> changingGear = new NetworkVariable<bool>(
        default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private NetworkVariable<bool> semiAutomaticGear = new NetworkVariable<bool>(
        default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private NetworkVariable<float> fuelInput = new NetworkVariable<float>(
        default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private NetworkVariable<bool> engineRunning = new NetworkVariable<bool>(
        default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private NetworkVariable<bool> lowBeamHeadLightsOn = new NetworkVariable<bool>(
        default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private NetworkVariable<bool> highBeamHeadLightsOn = new NetworkVariable<bool>(
        default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private NetworkVariable<RCC_CarControllerV3.IndicatorsOn> indicatorsOn = new NetworkVariable<RCC_CarControllerV3.IndicatorsOn>(
        default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private TextMesh nicknameText;

    public override void OnNetworkSpawn()
    {
        carController = GetComponent<RCC_CarControllerV3>();
        rigid = GetComponent<Rigidbody>();

        if (IsOwner)
        {
            correctPlayerPos.Value = transform.position;
            correctPlayerRot.Value = transform.rotation;
        }

        // Create nickname text
        GameObject nicknameTextGO = new GameObject("NickName TextMesh");
        nicknameTextGO.transform.SetParent(transform, false);
        nicknameTextGO.transform.localPosition = new Vector3(0f, 2f, 0f);
        nicknameTextGO.transform.localScale = new Vector3(.25f, .25f, .25f);
        nicknameText = nicknameTextGO.AddComponent<TextMesh>();
        nicknameText.anchor = TextAnchor.MiddleCenter;
        nicknameText.fontSize = 25;
    }

    private void FixedUpdate()
    {
         if (carController == null || rigid == null)
        {
            Debug.LogWarning("RCC_CarControllerV3 or Rigidbody is not initialized.");
            return;
        }
        if (IsOwner)
        {
            // Update NetworkVariables
            correctPlayerPos.Value = transform.position;
            correctPlayerRot.Value = transform.rotation;
            currentVelocity.Value = rigid.velocity;
            gasInput.Value = carController.throttleInput;
            brakeInput.Value = carController.brakeInput;
            steerInput.Value = carController.steerInput;
            handbrakeInput.Value = carController.handbrakeInput;
            boostInput.Value = carController.boostInput;
            clutchInput.Value = carController.clutchInput;
            gear.Value = carController.currentGear;
            direction.Value = carController.direction;
            changingGear.Value = carController.changingGear;
            semiAutomaticGear.Value = carController.semiAutomaticGear;
            fuelInput.Value = carController.fuelInput;
            engineRunning.Value = carController.engineRunning;
            lowBeamHeadLightsOn.Value = carController.lowBeamHeadLightsOn;
            highBeamHeadLightsOn.Value = carController.highBeamHeadLightsOn;
            indicatorsOn.Value = carController.indicatorsOn;
        }
        else
        {
            // Sync vehicle state from NetworkVariables
            transform.position = Vector3.Lerp(transform.position, correctPlayerPos.Value, Time.deltaTime * 5f);
            transform.rotation = Quaternion.Lerp(transform.rotation, correctPlayerRot.Value, Time.deltaTime * 5f);
            rigid.velocity = Vector3.Lerp(rigid.velocity, currentVelocity.Value, Time.deltaTime * 5f);

            carController.throttleInput = gasInput.Value;
            carController.brakeInput = brakeInput.Value;
            carController.steerInput = steerInput.Value;
            carController.handbrakeInput = handbrakeInput.Value;
            carController.boostInput = boostInput.Value;
            carController.clutchInput = clutchInput.Value;
            carController.currentGear = gear.Value;
            carController.direction = direction.Value;
            carController.changingGear = changingGear.Value;
            carController.semiAutomaticGear = semiAutomaticGear.Value;
            carController.fuelInput = fuelInput.Value;
            carController.engineRunning = engineRunning.Value;
            carController.lowBeamHeadLightsOn = lowBeamHeadLightsOn.Value;
            carController.highBeamHeadLightsOn = highBeamHeadLightsOn.Value;
            carController.indicatorsOn = indicatorsOn.Value;
        }

         // Display nickname text if available
        if (nicknameText && IsOwner)
        {
            nicknameText.text = NetworkManager.Singleton.LocalClientId.ToString();
            if (Camera.main)
            {
                nicknameText.transform.LookAt(Camera.main.transform);
                nicknameText.transform.rotation = Quaternion.Euler(0, nicknameText.transform.eulerAngles.y + 180f, 0);
            }
        }
        else if (nicknameText == null)
        {
            Debug.LogError("Nickname TextMesh is not initialized.");
        }
    }
}
