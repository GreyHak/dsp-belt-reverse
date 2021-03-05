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

        public static RectTransform reverseButton;
        public static Sprite reverseSprite;

        [HarmonyPrefix, HarmonyPatch(typeof(GameMain), "Begin")]
        public static void GameMain_Begin_Prefix()
        {
            if (GameMain.instance != null && GameObject.Find("Game Menu/button-1-bg"))
            {
                if (!GameObject.Find("greyhak-reverse-button"))
                {
                    RectTransform prefab = GameObject.Find("Game Menu/button-1-bg").GetComponent<RectTransform>();
                    Vector3 referencePosition = prefab.localPosition;
                    reverseButton = GameObject.Instantiate<RectTransform>(prefab);
                    reverseButton.gameObject.name = "greyhak-reverse-button";
                    UIButton uiButton = reverseButton.GetComponent<UIButton>();
                    uiButton.CloseTip();
                    uiButton.tips.tipTitle = "Reverse Belt Direction";
                    uiButton.tips.tipText = "Click to reverse the direction of the conveyer belt.";
                    uiButton.tips.delay = 0f;
                    reverseButton.transform.Find("button-1/icon").GetComponent<Image>().sprite = GetSprite();
                    reverseButton.SetParent(UIRoot.instance.uiGame.beltWindow.windowTrans);
                    reverseButton.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                    reverseButton.localPosition = new Vector3(66, -70, 0);
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

        static void ReverseBelt(int beltId)
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

                //VFAudio.Create("build-start", null, Vector3.zero, true);
                VFAudio.Create("build-start", null, GameMain.mainPlayer.factory.entityPool[beltComponent.entityId].pos, true);
            }
        }
    }
}
