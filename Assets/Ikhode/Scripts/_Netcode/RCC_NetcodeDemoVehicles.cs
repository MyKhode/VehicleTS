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

public class RCC_NetcodeDemoVehicles : ScriptableObject {

    public RCC_CarControllerV3[] vehicles;

    #region singleton
    private static RCC_NetcodeDemoVehicles instance;
    public static RCC_NetcodeDemoVehicles Instance { get { if (instance == null) instance = Resources.Load("RCC_NetcodeDemoVehicles") as RCC_NetcodeDemoVehicles; return instance; } }
    #endregion

}
