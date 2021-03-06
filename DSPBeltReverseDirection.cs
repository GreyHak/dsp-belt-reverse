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
using System.Security.Permissions;

[module: UnverifiableCode]
#pragma warning disable CS0618 // Type or member is obsolete
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618 // Type or member is obsolete
namespace DSPBeltReverseDirection
{
    [BepInPlugin(pluginGuid, pluginName, pluginVersion)]
    [BepInProcess("DSPGAME.exe")]
    public class DSPBeltReverseDirection : BaseUnityPlugin
    {
        public const string pluginGuid = "greyhak.dysonsphereprogram.beltreversedirection";
        public const string pluginName = "DSP Belt Reverse Direction";
        public const string pluginVersion = "0.1.0";
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

        public static RectTransform reverseButton;
        public static Sprite reverseSprite;

        // The first call happens for some reason after every game load.
        // This causes the tip to be displayed without a mouse-over when the BeltWindow opens for the first time.
        static bool ignoreFirstReverseButtonOnPointerEnter = true;

        [HarmonyPrefix, HarmonyPatch(typeof(UIButton), "OnPointerEnter")]
        public static bool UIButton_OnPointerEnter_Prefix(UIButton __instance)
        {
            if (reverseButton != null && __instance == reverseButton.GetComponent<UIButton>() && ignoreFirstReverseButtonOnPointerEnter)
            {
                ignoreFirstReverseButtonOnPointerEnter = false;
                return false;
            }
            return true;
        }

        [HarmonyPrefix, HarmonyPatch(typeof(GameMain), "Begin")]
        public static void GameMain_Begin_Prefix()
        {
            ignoreFirstReverseButtonOnPointerEnter = true;
            if (GameMain.instance != null && GameObject.Find("Game Menu/button-1-bg"))
            {
                if (!GameObject.Find("greyhak-reverse-button"))
                {
                    RectTransform prefab = GameObject.Find("Game Menu/button-1-bg").GetComponent<RectTransform>();
                    Vector3 referencePosition = prefab.localPosition;
                    reverseButton = GameObject.Instantiate<RectTransform>(prefab);
                    reverseButton.gameObject.name = "greyhak-reverse-button";
                    UIButton uiButton = reverseButton.GetComponent<UIButton>();
                    uiButton.tips.tipTitle = "Reverse Belt Direction";
                    uiButton.tips.tipText = "Click to reverse the direction of the conveyer belt.";
                    uiButton.tips.delay = 0f;
                    reverseButton.transform.Find("button-1/icon").GetComponent<Image>().sprite = GetSprite();
                    reverseButton.SetParent(UIRoot.instance.uiGame.beltWindow.windowTrans);
                    reverseButton.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                    reverseButton.localPosition = new Vector3(66, -60, 0);
                    uiButton.OnPointerDown(null);
                    uiButton.OnPointerEnter(null);
                    uiButton.button.onClick.AddListener(() =>
                    {
                        ReverseBelt(UIRoot.instance.uiGame.beltWindow.beltId);
                    });

                }
            }
        }

        public static Sprite GetSprite()
        {
            Texture2D tex = new Texture2D(48, 48, TextureFormat.RGBA32, false);
            Color color = new Color(1, 1, 1, 1);

            // Draw a plane like the one re[resending drones in the Mecha Panel...
            for (int x = 0; x < 48; x++)
            {
                for (int y = 0; y < 48; y++)
                {
                    if (((x >= 7) && (x <= 39) && (y >= 10) && (y <= 16)) ||  // top
                        ((x == 33) && (y >= 1) && (y <= 25)) ||
                        ((x == 34) && (y >= 2) && (y <= 24)) ||
                        ((x == 35) && (y >= 3) && (y <= 23)) ||
                        ((x == 36) && (y >= 4) && (y <= 22)) ||
                        ((x == 37) && (y >= 5) && (y <= 21)) ||
                        ((x == 38) && (y >= 6) && (y <= 20)) ||
                        ((x == 39) && (y >= 7) && (y <= 19)) ||
                        ((x == 40) && (y >= 8) && (y <= 18)) ||
                        ((x == 41) && (y >= 9) && (y <= 17)) ||
                        ((x == 42) && (y >= 10) && (y <= 16)) ||
                        ((x == 43) && (y >= 11) && (y <= 15)) ||
                        ((x == 44) && (y >= 12) && (y <= 14)) ||
                        ((x == 45) && (y == 13)) ||
                        ((x >= 8) && (x <= 40) && (y >= 31) && (y <= 37)) ||  // bottom
                        ((x == 2) && (y == 34)) ||
                        ((x == 3) && (y >= 33) && (y <= 35)) ||
                        ((x == 4) && (y >= 32) && (y <= 36)) ||
                        ((x == 5) && (y >= 31) && (y <= 37)) ||
                        ((x == 6) && (y >= 30) && (y <= 38)) ||
                        ((x == 7) && (y >= 29) && (y <= 39)) ||
                        ((x == 8) && (y >= 28) && (y <= 40)) ||
                        ((x == 9) && (y >= 27) && (y <= 41)) ||
                        ((x == 10) && (y >= 26) && (y <= 42)) ||
                        ((x == 11) && (y >= 25) && (y <= 43)) ||
                        ((x == 12) && (y >= 24) && (y <= 44)) ||
                        ((x == 13) && (y >= 23) && (y <= 45)) ||
                        ((x == 14) && (y >= 22) && (y <= 46)))
                    {
                        tex.SetPixel(x, y, color);
                    }
                    else
                    {
                        tex.SetPixel(x, y, new Color(0, 0, 0, 0));
                    }
                }
            }

            tex.name = "greyhak-reverse-icon";
            tex.Apply();

            return Sprite.Create(tex, new Rect(0f, 0f, 48f, 48f), new Vector2(0f, 0f), 1000);
        }

        struct ReverseConnection
        {
            public int targetId;
            public int outputId;
            public int inputId0;
            public int inputId1;
            public int inputId2;
        }

        static void ReverseBelt(int beltId)
        {
            PlanetFactory factory = GameMain.mainPlayer.factory;
            ref CargoTraffic cargoTraffic = ref factory.cargoTraffic;
            ref BeltComponent beltComponent = ref cargoTraffic.beltPool[beltId];
            ref CargoPath cargoPath = ref cargoTraffic.pathPool[beltComponent.segPathId];

            if (cargoPath.belts.Count > 1)
            {
                Logger.LogWarning("Reverse!");

                int firstBeltId = cargoPath.belts[0];
                int lastBeltId = cargoPath.belts[cargoPath.belts.Count - 1];
                BeltComponent firstBelt = cargoTraffic.beltPool[firstBeltId];
                BeltComponent lastBelt = cargoTraffic.beltPool[lastBeltId];

                // For machine connections we reference PlanetFactory.CreateEntityLogicComponents
                // These include splitter, miner, tank, fractionate, powerExchanger (and station)
                bool unusedFlag;
                int entityIdOfMachineOutputting;
                int slotOfMachineOutputting;
                int entityIdOfMachineGettingInput;
                int slotOfMachineGettingInput;
                factory.ReadObjectConn(firstBelt.entityId, 1, out unusedFlag, out entityIdOfMachineOutputting, out slotOfMachineOutputting);
                factory.ReadObjectConn(lastBelt.entityId, 0, out unusedFlag, out entityIdOfMachineGettingInput, out slotOfMachineGettingInput);
                // Note: "Machine" at this point is likely to just be another conveyer.

                List<ReverseConnection> reverseConnections = new List<ReverseConnection>();
                for (int beltIdx = cargoPath.belts.Count - 1; beltIdx >= 0; --beltIdx)
                {
                    BeltComponent thisBelt;
                    thisBelt = cargoTraffic.beltPool[cargoPath.belts[beltIdx]];
                    Logger.LogInfo((beltIdx > 0 ? cargoTraffic.beltPool[cargoPath.belts[beltIdx - 1]].id.ToString() : "start") + " -> " + thisBelt.id.ToString() + " -> " + (beltIdx + 1 < cargoPath.belts.Count ? cargoTraffic.beltPool[cargoPath.belts[beltIdx + 1]].id.ToString() : "end"));
                    Logger.LogInfo("   outputId=" + thisBelt.outputId.ToString() + ", backInputId=" + thisBelt.backInputId.ToString() + ", leftInputId=" + thisBelt.leftInputId.ToString() + ", rightInputId=" + thisBelt.rightInputId.ToString());

                    ReverseConnection reverseConnection = new ReverseConnection();
                    reverseConnection.targetId = thisBelt.id;
                    reverseConnection.outputId = thisBelt.mainInputId;
                    Logger.LogInfo("      targetId=" + reverseConnection.targetId.ToString() + ", outputId=" + reverseConnection.outputId.ToString());

                    List<int> inputs = new List<int>();
                    if (thisBelt.outputId != 0) inputs.Add(thisBelt.outputId);
                    if (thisBelt.backInputId != 0 && thisBelt.backInputId != reverseConnection.outputId) inputs.Add(thisBelt.backInputId);
                    if (thisBelt.leftInputId != 0 && thisBelt.leftInputId != reverseConnection.outputId) inputs.Add(thisBelt.leftInputId);
                    if (thisBelt.rightInputId != 0 && thisBelt.rightInputId != reverseConnection.outputId) inputs.Add(thisBelt.rightInputId);
                    if (inputs.Count > 0) reverseConnection.inputId0 = inputs[0];
                    if (inputs.Count > 1) reverseConnection.inputId1 = inputs[1];
                    if (inputs.Count > 2) reverseConnection.inputId2 = inputs[2];
                    Logger.LogInfo("      inputId0=" + reverseConnection.inputId0.ToString() + ", inputId1=" + reverseConnection.inputId1.ToString() + ", inputId2=" + reverseConnection.inputId2.ToString());

                    reverseConnections.Add(reverseConnection);
                }

                // The order of this loop can be swaped, and still performs the same change.
                foreach (ReverseConnection reverseConnection in reverseConnections)
                {
                    cargoTraffic.AlterBeltConnections(reverseConnection.targetId, reverseConnection.outputId, reverseConnection.inputId0, reverseConnection.inputId1, reverseConnection.inputId2);
                }

                if (entityIdOfMachineOutputting > 0)
                {
                    EntityData entityOfMachineOutputting = factory.entityPool[entityIdOfMachineOutputting];
                    if (entityOfMachineOutputting.splitterId != 0)
                    {
                        Logger.LogInfo("      Belt receiving input from splitter " + entityOfMachineOutputting.splitterId.ToString());
                        cargoTraffic.ConnectToSplitter(entityOfMachineOutputting.splitterId, lastBeltId, slotOfMachineOutputting, true);
                    }
                    if (entityOfMachineOutputting.minerId != 0)
                    {
                        Logger.LogInfo("      Belt receiving input from miner " + entityOfMachineOutputting.minerId.ToString());
                        factory.factorySystem.SetMinerInsertTarget(entityOfMachineOutputting.minerId, 0);
                    }
                    if (entityOfMachineOutputting.tankId != 0)
                    {
                        Logger.LogInfo("      Belt receiving input from tank " + entityOfMachineOutputting.tankId.ToString());
                        factory.factoryStorage.SetTankBelt(entityOfMachineOutputting.tankId, lastBeltId, slotOfMachineOutputting, false);
                    }
                    if (entityOfMachineOutputting.fractionateId != 0)
                    {
                        Logger.LogInfo("      Belt receiving input from fractionator " + entityOfMachineOutputting.fractionateId.ToString());
                        factory.factorySystem.SetFractionateBelt(entityOfMachineOutputting.fractionateId, lastBeltId, slotOfMachineOutputting, false);
                    }
                    if (entityOfMachineOutputting.powerExcId != 0)
                    {
                        Logger.LogInfo("      Belt receiving input from power exchanger " + entityOfMachineOutputting.powerExcId.ToString());
                        factory.powerSystem.SetExchangerBelt(entityOfMachineOutputting.powerExcId, lastBeltId, slotOfMachineOutputting, false);
                    }
                    if (entityOfMachineOutputting.stationId != 0)
                    {
                        Logger.LogInfo("      Belt receiving input from station " + entityOfMachineOutputting.stationId.ToString());
                        factory.ApplyEntityInput(entityOfMachineOutputting.id, firstBelt.entityId, slotOfMachineOutputting, slotOfMachineOutputting, 0);
                        factory.ClearObjectConn(firstBelt.entityId);
                        factory.WriteObjectConnDirect(firstBelt.entityId, 0, false, entityIdOfMachineOutputting, slotOfMachineOutputting);
                        factory.WriteObjectConnDirect(entityIdOfMachineOutputting, slotOfMachineOutputting, false, firstBelt.entityId, 0);
                        Logger.LogInfo("         Station now set to " + factory.transport.stationPool[entityOfMachineOutputting.stationId].slots[slotOfMachineOutputting].dir.ToString());
                    }
                }
                if (entityIdOfMachineGettingInput > 0)
                {
                    EntityData entityOfMachineGettingInput = factory.entityPool[entityIdOfMachineGettingInput];
                    if (entityOfMachineGettingInput.splitterId != 0)
                    {
                        Logger.LogInfo("      Belt outputting to splitter " + entityOfMachineGettingInput.splitterId.ToString());
                        cargoTraffic.ConnectToSplitter(entityOfMachineGettingInput.splitterId, firstBeltId, slotOfMachineGettingInput, false);
                    }
                    if (entityOfMachineGettingInput.minerId != 0)
                    {
                        Logger.LogInfo("      ERROR: Belt outputting to miner " + entityOfMachineGettingInput.minerId.ToString());
                    }
                    if (entityOfMachineGettingInput.tankId != 0)
                    {
                        Logger.LogInfo("      Belt outputting to tank " + entityOfMachineGettingInput.tankId.ToString());
                        factory.factoryStorage.SetTankBelt(entityOfMachineGettingInput.tankId, firstBeltId, slotOfMachineGettingInput, true);
                    }
                    if (entityOfMachineGettingInput.fractionateId != 0)
                    {
                        Logger.LogInfo("      Belt outputting to fractionator " + entityOfMachineGettingInput.fractionateId.ToString());
                        factory.factorySystem.SetFractionateBelt(entityOfMachineGettingInput.fractionateId, firstBeltId, slotOfMachineGettingInput, true);
                    }
                    if (entityOfMachineGettingInput.powerExcId != 0)
                    {
                        Logger.LogInfo("      Belt outputting to power exchanger " + entityOfMachineGettingInput.powerExcId.ToString());
                        factory.powerSystem.SetExchangerBelt(entityOfMachineGettingInput.powerExcId, firstBeltId, slotOfMachineGettingInput, true);
                    }
                    if (entityOfMachineGettingInput.stationId != 0)
                    {
                        Logger.LogInfo("      Belt outputting to station " + entityOfMachineGettingInput.stationId.ToString());
                        factory.ApplyEntityOutput(entityOfMachineGettingInput.id, lastBelt.entityId, slotOfMachineGettingInput, slotOfMachineGettingInput, 0);
                        factory.ClearObjectConn(lastBelt.entityId);
                        factory.WriteObjectConnDirect(lastBelt.entityId, 1, true, entityIdOfMachineGettingInput, slotOfMachineGettingInput);
                        factory.WriteObjectConnDirect(entityIdOfMachineGettingInput, slotOfMachineGettingInput, true, lastBelt.entityId, 1);
                        Logger.LogInfo("         Station now set to " + factory.transport.stationPool[entityOfMachineGettingInput.stationId].slots[slotOfMachineGettingInput].dir.ToString());
                    }
                }

                // Audio comes from LDB.audios.  Good built-in choices are "warp-end" or "ui-click-2" (the upgrade sound).
                VFAudio.Create("ui-click-2", null, GameMain.mainPlayer.factory.entityPool[beltComponent.entityId].pos, true);
            }
        }
    }
}
