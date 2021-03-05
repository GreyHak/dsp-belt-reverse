//
// Copyright (c) 2021, Aaron Shumate
// All rights reserved.
//
// This source code is licensed under the BSD-style license found in the
// LICENSE.txt file in the root directory of this source tree. 
//
// Dyson Sphere Program is developed by Youthcat Studio and published by Gamera Game.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using BepInEx.Logging;
using System.Security;
//using System.Security.Permissions;

//[module: UnverifiableCode]
//#pragma warning disable CS0618 // Type or member is obsolete
//[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
//#pragma warning restore CS0618 // Type or member is obsolete
namespace DSPBeltReverseDirection
{
    [BepInPlugin(pluginGuid, pluginName, pluginVersion)]
    [BepInProcess("DSPGAME.exe")]
    public class DSPBeltReverseDirection : BaseUnityPlugin
    {
        public const string pluginGuid = "greyhak.dysonsphereprogram.beltreversedirection";
        public const string pluginName = "DSP Belt Reverse Direction";
        public const string pluginVersion = "1.0.0";
        new internal static ManualLogSource Logger;
        new internal static BepInEx.Configuration.ConfigFile Config;
        Harmony harmony;

        public void Awake()
        {
            Logger = base.Logger;  // "C:\Program Files (x86)\Steam\steamapps\common\Dyson Sphere Program\BepInEx\LogOutput.log"
            Config = base.Config;  // "C:\Program Files (x86)\Steam\steamapps\common\Dyson Sphere Program\BepInEx\config\greyhak.dysonsphereprogram.beltreversedirection.cfg"

            harmony = new Harmony(pluginGuid);
            harmony.PatchAll(typeof(DSPBeltReverseDirection));

            Logger.LogInfo("Initialization complete.");
        }

        static int beltId = 0;

        [HarmonyPostfix, HarmonyPatch(typeof(UIBeltWindow), "_OnUpdate")]
        public static void UIBeltWindow_OnUpdate_Postfix(UIBeltWindow __instance)
        {
            if (beltId != __instance.beltId)  // This isn't a sufficient check to debounce
                Logger.LogInfo("Update: Grabbed belt ID " + __instance.beltId.ToString());
            beltId = __instance.beltId;
        }

        [HarmonyPostfix, HarmonyPatch(typeof(PlayerAction_Build), "UpgradeMainLogic")]
        public static void PlayerAction_Build_UpgradeMainLogic_Postfix(PlayerAction_Build __instance)
        {
            if (GameMain.mainPlayer == null || GameMain.mainPlayer.factory == null)
                return;

            if (VFInput._counterRotate)
            {
                if (!VFInput.onGUI)
                {
                    UICursor.SetCursor(ECursor.DysonNodeCreate);  // Won't do anything with Prefix returning true.  For Postfix, you need to hold for the game not to override it.

                    if (beltId != 0)
                    {
                        ref CargoTraffic cargoTraffic = ref GameMain.mainPlayer.factory.cargoTraffic;
                        ref BeltComponent beltComponent = ref cargoTraffic.beltPool[beltId];
                        ref CargoPath cargoPath = ref cargoTraffic.pathPool[beltComponent.segPathId];

                        if (cargoPath.belts.Count > 1)
                        {
                            Logger.LogWarning("Reverse!");
                            List<int> cargoPathBelts = new List<int>();
                            for (int beltIdx = cargoPath.belts.Count - 1; beltIdx >= 0; --beltIdx)
                            {
                                Logger.LogInfo("[" + beltIdx.ToString() + "]=" + cargoPath.belts[beltIdx].ToString());
                                cargoPathBelts.Add(cargoPath.belts[beltIdx]);
                            }

                            Logger.LogInfo("Start: " + cargoTraffic.beltPool[cargoPathBelts[0]].id.ToString() + " -> " + cargoTraffic.beltPool[cargoPathBelts[1]].id.ToString());
                            GameMain.mainPlayer.factory.cargoTraffic.AlterBeltConnections(cargoTraffic.beltPool[cargoPathBelts[0]].id, cargoTraffic.beltPool[cargoPathBelts[1]].id, 0, 0, 0);
                            for (int beltIdx = 1; beltIdx < cargoPathBelts.Count - 1; ++beltIdx)
                            {
                                Logger.LogInfo(cargoTraffic.beltPool[cargoPathBelts[beltIdx - 1]].id.ToString() + " -> " + cargoTraffic.beltPool[cargoPathBelts[beltIdx]].id.ToString() + " -> " + cargoTraffic.beltPool[cargoPathBelts[beltIdx + 1]].id.ToString());
                                GameMain.mainPlayer.factory.cargoTraffic.AlterBeltConnections(cargoTraffic.beltPool[cargoPathBelts[beltIdx]].id, cargoTraffic.beltPool[cargoPathBelts[beltIdx + 1]].id, cargoTraffic.beltPool[cargoPathBelts[beltIdx - 1]].id, 0, 0);
                            }
                            Logger.LogInfo(cargoTraffic.beltPool[cargoPathBelts[cargoPathBelts.Count - 2]].id.ToString() + " -> " + cargoTraffic.beltPool[cargoPathBelts[cargoPathBelts.Count - 1]].id.ToString() + " Done");
                            GameMain.mainPlayer.factory.cargoTraffic.AlterBeltConnections(cargoTraffic.beltPool[cargoPathBelts[cargoPathBelts.Count - 1]].id, 0, cargoTraffic.beltPool[cargoPathBelts[cargoPathBelts.Count - 2]].id, 0, 0);
                        }

                        beltId = 0;
                    }
                }
            }
        }
    }
}