//----------------------------------------------
//            Realistic Car Controller
//
// Copyright © 2014 - 2024 BoneCracker Games
// https://www.bonecrackergames.com
// Buğra Özdoğanlar
//
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [CreateAssetMenu(fileName = "RCC_LocalDemoVehicles", menuName = "RealisticCarController/RCC_LocalDemoVehicles")]
public class RCC_LocalDemoVehicles : ScriptableObject {

    public RCC_CarControllerV3[] vehicles;

    #region singleton
    private static RCC_LocalDemoVehicles instance;
    public static RCC_LocalDemoVehicles Instance { get { if (instance == null) instance = Resources.Load("RCC_LocalDemoVehicles") as RCC_LocalDemoVehicles; return instance; } }
    #endregion

}
