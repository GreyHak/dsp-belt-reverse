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
        public const string pluginVersion = "1.1.3";
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
                    uiButton.tips.tipText = "Click to reverse the direction of the conveyor belt.";
                    uiButton.tips.delay = 0f;
                    reverseButton.transform.Find("button-1/icon").GetComponent<Image>().sprite = GetSprite();
                    reverseButton.SetParent(UIRoot.instance.uiGame.beltWindow.windowTrans);
                    reverseButton.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                    reverseButton.localPosition = new Vector3(5, 42, 0);
                    uiButton.OnPointerDown(null);
                    uiButton.OnPointerEnter(null);
                    uiButton.button.onClick.AddListener(() =>
                    {
                        ReverseBelt();
                    });
                    uiButton.onRightClick += PrintDebugInformationAboutPath;
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

        public const int BELT_INPUT_SLOT = 1;
        public const int BELT_OUTPUT_SLOT = 0;

        // See this debug data in BepInEx console by setting Logging.Console.Enabled = true.
        // See this debug data in LogOutput.log by adding Debug to Logging.Disk.LogLevels.
        public static void PrintDebugInformationAboutPath(int data)
        {
            int selectedBeltId = UIRoot.instance.uiGame.beltWindow.beltId;
            PlanetFactory factory = GameMain.mainPlayer.factory;
            CargoTraffic cargoTraffic = factory.cargoTraffic;
            BeltComponent selectedBeltComponent = cargoTraffic.beltPool[selectedBeltId];
            CargoPath cargoPath = cargoTraffic.pathPool[selectedBeltComponent.segPathId];
            List<int> cargoPathBelts = cargoPath.belts;

            Logger.LogDebug("===============================================================================");
            Logger.LogDebug("selectedBeltId=" + selectedBeltId.ToString() + ", selectedBeltComponent.segPathId=" + selectedBeltComponent.segPathId.ToString());
            Logger.LogDebug("cargoPathBelts.Count=" + cargoPathBelts.Count.ToString() + ", cargoPath.pathLength=" + cargoPath.pathLength.ToString());
            for (int beltIdx = 0; beltIdx < cargoPathBelts.Count; ++beltIdx)
            {
                BeltComponent beltComponent = cargoTraffic.beltPool[cargoPathBelts[beltIdx]];
                Logger.LogDebug("  Belt[" + beltIdx.ToString() +
                    "]: id=" + beltComponent.id.ToString() +
                    ", entityId=" + beltComponent.entityId.ToString() +
                    ", outputId=" + beltComponent.outputId.ToString() +
                    ", mainInputId=" + beltComponent.mainInputId.ToString() +
                    ", backInputId=" + beltComponent.backInputId.ToString() +
                    ", leftInputId=" + beltComponent.leftInputId.ToString() +
                    ", rightInputId=" + beltComponent.rightInputId.ToString());

                factory.ReadObjectConn(beltComponent.entityId, BELT_INPUT_SLOT, out bool inputsOutputFlag, out int inputEntityId, out int inputEntitySlot);
                factory.ReadObjectConn(beltComponent.entityId, BELT_OUTPUT_SLOT, out bool outputsOutputFlag, out int outputEntityId, out int outputEntitySlot);

                Logger.LogDebug("    Input slot: inputsOutputFlag=" + inputsOutputFlag.ToString() + ", inputEntityId=" + inputEntityId.ToString() + ", inputEntitySlot=" + inputEntitySlot.ToString());
                PrintDebugInformationAboutEntity(inputEntityId);
                Logger.LogDebug("    Output slot: outputsOutputFlag=" + outputsOutputFlag.ToString() + ", outputEntityId=" + outputEntityId.ToString() + ", outputEntitySlot=" + outputEntitySlot.ToString());
                PrintDebugInformationAboutEntity(outputEntityId);
            }
            Logger.LogDebug("===============================================================================");
        }

        public static void PrintDebugInformationAboutEntity(int entityId)
        {
            if (entityId != 0)
            {
                PlanetFactory factory = GameMain.mainPlayer.factory;
                CargoTraffic cargoTraffic = factory.cargoTraffic;

                EntityData entity = factory.entityPool[entityId];
                if (entity.splitterId != 0)
                {
                    SplitterComponent splitterComponent = cargoTraffic.splitterPool[entity.splitterId];
                    Logger.LogDebug("      Is splitterId=" + entity.splitterId.ToString() +
                        ", beltA=" + splitterComponent.beltA.ToString() +
                        ", beltB=" + splitterComponent.beltB.ToString() +
                        ", beltC=" + splitterComponent.beltC.ToString() +
                        ", beltD=" + splitterComponent.beltD.ToString());
                }
                if (entity.minerId != 0)
                {
                    MinerComponent minerComponent = factory.factorySystem.minerPool[entity.minerId];
                    Logger.LogDebug("      Is minerId=" + entity.minerId.ToString());
                }
                if (entity.tankId != 0)
                {
                    TankComponent tankComponent = factory.factoryStorage.tankPool[entity.tankId];
                    Logger.LogDebug("      Is tankId=" + entity.tankId.ToString() +
                        ", belt0=" + tankComponent.belt0.ToString() + (tankComponent.isOutput0 ? "(output)" : "(input)") +
                        ", belt1=" + tankComponent.belt1.ToString() + (tankComponent.isOutput1 ? "(output)" : "(input)") +
                        ", belt2=" + tankComponent.belt2.ToString() + (tankComponent.isOutput2 ? "(output)" : "(input)") +
                        ", belt3=" + tankComponent.belt3.ToString() + (tankComponent.isOutput3 ? "(output)" : "(input)"));
                }
                if (entity.fractionateId != 0)
                {
                    FractionateComponent fractionateComponent = factory.factorySystem.fractionatePool[entity.fractionateId];
                    Logger.LogDebug("      Is fractionateId=" + entity.fractionateId.ToString() +
                        ", belt0=" + fractionateComponent.belt0.ToString() + (fractionateComponent.isOutput0 ? "(output)" : "(input)") +
                        ", belt1=" + fractionateComponent.belt1.ToString() + (fractionateComponent.isOutput1 ? "(output)" : "(input)") +
                        ", belt2=" + fractionateComponent.belt2.ToString() + (fractionateComponent.isOutput2 ? "(output)" : "(input)"));
                }
                if (entity.powerExcId != 0)
                {
                    PowerExchangerComponent powerExchangerComponent = factory.powerSystem.excPool[entity.powerExcId];
                    Logger.LogDebug("      Is powerExcId=" + entity.powerExcId.ToString() +
                        ", belt0=" + powerExchangerComponent.belt0.ToString() + (powerExchangerComponent.isOutput0 ? "(output)" : "(input)") +
                        ", belt1=" + powerExchangerComponent.belt1.ToString() + (powerExchangerComponent.isOutput1 ? "(output)" : "(input)") +
                        ", belt2=" + powerExchangerComponent.belt2.ToString() + (powerExchangerComponent.isOutput2 ? "(output)" : "(input)") +
                        ", belt3=" + powerExchangerComponent.belt3.ToString() + (powerExchangerComponent.isOutput3 ? "(output)" : "(input)"));
                }
                if (entity.stationId != 0)
                {
                    StationComponent stationComponent = factory.transport.stationPool[entity.stationId];
                    Logger.LogDebug("      Is stationId=" + entity.stationId.ToString() + " name=" + stationComponent.name);
                }

                for (int slotIdx = 0; slotIdx < 6; ++slotIdx)
                {
                    factory.ReadObjectConn(entityId, slotIdx, out bool outputFlag, out int otherEntityId, out int otherEntitySlot);
                    if (otherEntityId != 0)
                    {
                        Logger.LogDebug("        Slot " + slotIdx.ToString() + (outputFlag ? " outputting to " : " inputting from ") + "entity " + otherEntityId.ToString() + " slot " + otherEntitySlot.ToString());
                    }
                }
            }
        }

        struct ReverseConnection
        {
            public int targetId;
            public int outputId;
            public int inputId0;
            public int inputId1;
            public int inputId2;
        }

        static void ReverseBelt()
        {
            int selectedBeltId = UIRoot.instance.uiGame.beltWindow.beltId;
            PlanetFactory factory = GameMain.mainPlayer.factory;
            CargoTraffic cargoTraffic = factory.cargoTraffic;
            BeltComponent selectedBeltComponent = cargoTraffic.beltPool[selectedBeltId];
            CargoPath cargoPath = cargoTraffic.pathPool[selectedBeltComponent.segPathId];

            if (cargoPath.belts.Count > 1)
            {
                Logger.LogMessage("Reverse!");

                bool grabbedItemsFlag = true;
                List<int> cargoIds = new List<int>();
                for (int index = 0; index + 9 < cargoPath.bufferLength;)
                {
                    if (cargoPath.buffer[index] == 0)
                    {
                        cargoIds.Add(0);
                        ++index;
                    }
                    else if (
                        (cargoPath.buffer[index + 0] == 246) ||
                        (cargoPath.buffer[index + 1] == 247) ||
                        (cargoPath.buffer[index + 2] == 248) ||
                        (cargoPath.buffer[index + 3] == 249) ||
                        (cargoPath.buffer[index + 4] == 250) ||
                        (cargoPath.buffer[index + 9] == byte.MaxValue) )
                    {
                        int extractedCargoId = (int)
                            (cargoPath.buffer[index + 5] - 1 +
                            (cargoPath.buffer[index + 6] - 1) * 100) +
                            (int)(cargoPath.buffer[index + 7] - 1) * 10000 +
                            (int)(cargoPath.buffer[index + 8] - 1) * 1000000;
                        cargoIds.Add(extractedCargoId);
                        index += 10;
                    }
                    else
                    {
                        Logger.LogWarning("Unable to identify items on the belt.");
                        grabbedItemsFlag = false;
                        break;
                    }
                }
                if (grabbedItemsFlag)
                {
                    Array.Clear(cargoPath.buffer, 0, cargoPath.bufferLength);
                }

                int firstBeltId = cargoPath.belts[0];
                int lastBeltId = cargoPath.belts[cargoPath.belts.Count - 1];
                BeltComponent firstBelt = cargoTraffic.beltPool[firstBeltId];
                BeltComponent lastBelt = cargoTraffic.beltPool[lastBeltId];

                // For machine connections we reference PlanetFactory.CreateEntityLogicComponents
                // These include splitter, miner, tank, fractionate, powerExchanger (and station)
                factory.ReadObjectConn(firstBelt.entityId, BELT_INPUT_SLOT, out bool unusedFlag, out int entityIdOfMachineOutputting, out int slotOfMachineOutputting);
                factory.ReadObjectConn(lastBelt.entityId, BELT_OUTPUT_SLOT, out unusedFlag, out int entityIdOfMachineGettingInput, out int slotOfMachineGettingInput);
                // Note: "Machine" at this point is likely to just be another conveyer.

                List<ReverseConnection> reverseConnections = new List<ReverseConnection>();
                for (int beltIdx = cargoPath.belts.Count - 1; beltIdx >= 0; --beltIdx)
                {
                    BeltComponent thisBelt = cargoTraffic.beltPool[cargoPath.belts[beltIdx]];
                    Logger.LogDebug((beltIdx > 0 ? cargoTraffic.beltPool[cargoPath.belts[beltIdx - 1]].id.ToString() : "start") + " -> " + thisBelt.id.ToString() + " -> " + (beltIdx + 1 < cargoPath.belts.Count ? cargoTraffic.beltPool[cargoPath.belts[beltIdx + 1]].id.ToString() : "end"));
                    Logger.LogDebug("   outputId=" + thisBelt.outputId.ToString() + ", backInputId=" + thisBelt.backInputId.ToString() + ", leftInputId=" + thisBelt.leftInputId.ToString() + ", rightInputId=" + thisBelt.rightInputId.ToString());

                    if (beltIdx == cargoPath.belts.Count - 1 && thisBelt.outputId != 0 && cargoTraffic.beltPool[thisBelt.outputId].segPathId != thisBelt.segPathId)
                    {
                        // About to break a primary segment, so consider this belt as having no output
                        thisBelt.outputId = 0;
                    }

                    ReverseConnection reverseConnection = new ReverseConnection
                    {
                        targetId = thisBelt.id,
                        outputId = thisBelt.mainInputId
                    };
                    Logger.LogDebug("      targetId=" + reverseConnection.targetId.ToString() + ", outputId=" + reverseConnection.outputId.ToString());

                    List<int> inputs = new List<int>();
                    if (thisBelt.outputId != 0) inputs.Add(thisBelt.outputId);
                    if (thisBelt.backInputId != 0 && thisBelt.backInputId != reverseConnection.outputId) inputs.Add(thisBelt.backInputId);
                    if (thisBelt.leftInputId != 0 && thisBelt.leftInputId != reverseConnection.outputId) inputs.Add(thisBelt.leftInputId);
                    if (thisBelt.rightInputId != 0 && thisBelt.rightInputId != reverseConnection.outputId) inputs.Add(thisBelt.rightInputId);
                    if (inputs.Count > 0) reverseConnection.inputId0 = inputs[0];
                    if (inputs.Count > 1) reverseConnection.inputId1 = inputs[1];
                    if (inputs.Count > 2) reverseConnection.inputId2 = inputs[2];
                    Logger.LogDebug("      inputId0=" + reverseConnection.inputId0.ToString() + ", inputId1=" + reverseConnection.inputId1.ToString() + ", inputId2=" + reverseConnection.inputId2.ToString());

                    reverseConnections.Add(reverseConnection);
                }

                // The order of this loop can be swaped, and still performs the same change.
                foreach (ReverseConnection reverseConnection in reverseConnections)
                {
                    cargoTraffic.AlterBeltConnections(reverseConnection.targetId, reverseConnection.outputId, reverseConnection.inputId0, reverseConnection.inputId1, reverseConnection.inputId2);

                    int entityIdOfThisBelt = cargoTraffic.beltPool[reverseConnection.targetId].entityId;
                    int entityIdOfOutputBelt = reverseConnection.outputId == 0 ? 0 : cargoTraffic.beltPool[reverseConnection.outputId].entityId;
                    int entityIdOfMainInputBelt = reverseConnection.inputId0 == 0 ? 0 : cargoTraffic.beltPool[reverseConnection.inputId0].entityId;

                    // This loop will disconnect all inserters.  It's based on PlanetFactory.RemoveEntityWithComponents() and PlanetFactory.ApplyEntityDisconnection()
                    for (int slotIdx = 0; slotIdx < 16; slotIdx++)
                    {
                        factory.ReadObjectConn(entityIdOfThisBelt, slotIdx, out bool flag, out int otherEntityId, out int otherSlotId);
                        if (otherEntityId > 0)
                        {
                            int inserterId = factory.entityPool[otherEntityId].inserterId;
                            if (inserterId > 0)  // Is otherEntityId an inserter entity?
                            {
                                if (factory.factorySystem.inserterPool[inserterId].insertTarget == entityIdOfThisBelt)
                                {
                                    Logger.LogDebug($"Disconnecting inserter insert target {inserterId} from {entityIdOfThisBelt}");
                                    factory.factorySystem.SetInserterInsertTarget(inserterId, 0, 0);
                                }
                                if (factory.factorySystem.inserterPool[inserterId].pickTarget == entityIdOfThisBelt)
                                {
                                    Logger.LogDebug($"Disconnecting inserter pick target {inserterId} from {entityIdOfThisBelt}");
                                    factory.factorySystem.SetInserterPickTarget(inserterId, 0, 0);
                                }
                            }
                        }
                    }

                    factory.ClearObjectConn(entityIdOfThisBelt);
                    factory.WriteObjectConnDirect(entityIdOfThisBelt, BELT_OUTPUT_SLOT, true, entityIdOfOutputBelt, BELT_INPUT_SLOT);
                    factory.WriteObjectConnDirect(entityIdOfThisBelt, BELT_INPUT_SLOT, false, entityIdOfMainInputBelt, BELT_OUTPUT_SLOT);
                    factory.OnBeltBuilt(entityIdOfThisBelt);  // This reconnects the inserters
                }

                if (entityIdOfMachineOutputting > 0)
                {
                    EntityData entityOfMachineOutputting = factory.entityPool[entityIdOfMachineOutputting];
                    if (entityOfMachineOutputting.splitterId != 0)
                    {
                        Logger.LogDebug("      Belt receiving input from splitter " + entityOfMachineOutputting.splitterId.ToString());
                        cargoTraffic.ConnectToSplitter(entityOfMachineOutputting.splitterId, firstBeltId, slotOfMachineOutputting, true);
                    }
                    else if (entityOfMachineOutputting.minerId != 0)
                    {
                        Logger.LogDebug("      Belt receiving input from miner " + entityOfMachineOutputting.minerId.ToString());
                        factory.factorySystem.SetMinerInsertTarget(entityOfMachineOutputting.minerId, 0);
                    }
                    else if (entityOfMachineOutputting.tankId != 0)
                    {
                        Logger.LogDebug("      Belt receiving input from tank " + entityOfMachineOutputting.tankId.ToString());
                        factory.factoryStorage.SetTankBelt(entityOfMachineOutputting.tankId, firstBeltId, slotOfMachineOutputting, false);
                    }
                    else if (entityOfMachineOutputting.fractionateId != 0)
                    {
                        Logger.LogDebug("      Belt receiving input from fractionator " + entityOfMachineOutputting.fractionateId.ToString());
                        factory.factorySystem.SetFractionateBelt(entityOfMachineOutputting.fractionateId, firstBeltId, slotOfMachineOutputting, false);
                    }
                    else if (entityOfMachineOutputting.powerExcId != 0)
                    {
                        Logger.LogDebug("      Belt receiving input from power exchanger " + entityOfMachineOutputting.powerExcId.ToString());
                        factory.powerSystem.SetExchangerBelt(entityOfMachineOutputting.powerExcId, firstBeltId, slotOfMachineOutputting, false);
                    }
                    else if (entityOfMachineOutputting.stationId != 0)
                    {
                        Logger.LogDebug("      Belt receiving input from station " + entityOfMachineOutputting.stationId.ToString());
                        factory.ApplyEntityInput(entityIdOfMachineOutputting, firstBelt.entityId, slotOfMachineOutputting, slotOfMachineOutputting, 0);
                        Logger.LogDebug("         Station now set to " + factory.transport.stationPool[entityOfMachineOutputting.stationId].slots[slotOfMachineOutputting].dir.ToString());
                    }
                    factory.WriteObjectConnDirect(firstBelt.entityId, BELT_OUTPUT_SLOT, true, entityIdOfMachineOutputting, slotOfMachineOutputting);
                    factory.WriteObjectConnDirect(entityIdOfMachineOutputting, slotOfMachineOutputting, false, firstBelt.entityId, BELT_OUTPUT_SLOT);
                }
                if (entityIdOfMachineGettingInput > 0)
                {
                    EntityData entityOfMachineGettingInput = factory.entityPool[entityIdOfMachineGettingInput];
                    if (entityOfMachineGettingInput.splitterId != 0)
                    {
                        Logger.LogDebug("      Belt outputting to splitter " + entityOfMachineGettingInput.splitterId.ToString());
                        cargoTraffic.ConnectToSplitter(entityOfMachineGettingInput.splitterId, lastBeltId, slotOfMachineGettingInput, false);
                    }
                    else if (entityOfMachineGettingInput.minerId != 0)
                    {
                        Logger.LogWarning("      ERROR: Belt outputting to miner " + entityOfMachineGettingInput.minerId.ToString());
                    }
                    else if (entityOfMachineGettingInput.tankId != 0)
                    {
                        Logger.LogDebug("      Belt outputting to tank " + entityOfMachineGettingInput.tankId.ToString());
                        factory.factoryStorage.SetTankBelt(entityOfMachineGettingInput.tankId, lastBeltId, slotOfMachineGettingInput, true);
                    }
                    else if (entityOfMachineGettingInput.fractionateId != 0)
                    {
                        Logger.LogDebug("      Belt outputting to fractionator " + entityOfMachineGettingInput.fractionateId.ToString());
                        factory.factorySystem.SetFractionateBelt(entityOfMachineGettingInput.fractionateId, lastBeltId, slotOfMachineGettingInput, true);
                    }
                    else if (entityOfMachineGettingInput.powerExcId != 0)
                    {
                        Logger.LogDebug("      Belt outputting to power exchanger " + entityOfMachineGettingInput.powerExcId.ToString());
                        factory.powerSystem.SetExchangerBelt(entityOfMachineGettingInput.powerExcId, lastBeltId, slotOfMachineGettingInput, true);
                    }
                    else if (entityOfMachineGettingInput.stationId != 0)
                    {
                        Logger.LogDebug("      Belt outputting to station " + entityOfMachineGettingInput.stationId.ToString());
                        factory.ApplyEntityOutput(entityIdOfMachineGettingInput, lastBelt.entityId, slotOfMachineGettingInput, slotOfMachineGettingInput, 0);
                        Logger.LogDebug("         Station now set to " + factory.transport.stationPool[entityOfMachineGettingInput.stationId].slots[slotOfMachineGettingInput].dir.ToString());
                    }
                    factory.WriteObjectConnDirect(lastBelt.entityId, BELT_INPUT_SLOT, false, entityIdOfMachineGettingInput, slotOfMachineGettingInput);
                    factory.WriteObjectConnDirect(entityIdOfMachineGettingInput, slotOfMachineGettingInput, true, lastBelt.entityId, BELT_INPUT_SLOT);
                }

                if (grabbedItemsFlag)
                {
                    CargoPath newCargoPath = cargoTraffic.pathPool[cargoTraffic.beltPool[UIRoot.instance.uiGame.beltWindow.beltId].segPathId];

                    int index = 4;
                    for (int cargoIdIdx = cargoIds.Count - 1; cargoIdIdx >= 0; --cargoIdIdx)
                    {
                        int insertCargoId = cargoIds[cargoIdIdx];
                        if (insertCargoId == 0)
                        {
                            index++;
                        }
                        else
                        {
                            if (index + 10 > newCargoPath.bufferLength)
                            {
                                Logger.LogInfo("New cargo path is not large enough to fit all the items from the original path.  Sending item to Icarus' inventory.");

                                CargoContainer cargoContainer = cargoPath.cargoContainer;
                                Cargo cargo = cargoContainer.cargoPool[insertCargoId];
                                int cargoItem = cargo.item;

                                cargoContainer.RemoveCargo(insertCargoId);
                                if (GameMain.mainPlayer.package.AddItemStacked(cargoItem, 1) == 1)
                                {
                                    UIItemup.Up(cargoItem, 1);
                                }
                            }
                            else
                            {
                                newCargoPath.InsertCargoDirect(index, insertCargoId);
                                index += 10;
                            }
                        }
                    }
                }

                cargoTraffic.RemoveCargoPath(cargoPath.id);

                // Audio comes from LDB.audios.  Good built-in choices are "warp-end" or "ui-click-2" (the upgrade sound).
                VFAudio.Create("ui-click-2", null, GameMain.mainPlayer.factory.entityPool[selectedBeltComponent.entityId].pos, true);
            }
        }
    }
}
