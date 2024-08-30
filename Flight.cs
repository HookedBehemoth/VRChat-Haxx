/*
 * Copyright (c) 2021-2022 HookedBehemoth
 *
 * This program is free software; you can redistribute it and/or modify it
 * under the terms and conditions of the GNU General Public License,
 * version 3, as published by the Free Software Foundation.
 *
 * This program is distributed in the hope it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for
 * more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using ActionMenuApi.Api;
using FlightMod;
using MelonLoader;
using Il2CppTMPro;
using UnityEngine;
using UnityEngine.XR;
using Il2CppVRC.SDKBase;

using ActionMenuDriver = Il2Cpp.MonoBehaviourPublicObGaObAc1ObAcBoCoObUnique;
using ActionMenuOpener = Il2Cpp.MonoBehaviourPublicCaObAc1BoSiBoObObObUnique;
using ActionMenuType = Il2Cpp.MonoBehaviourPublicCaObAc1BoSiBoObObObUnique.EnumNPublicSealedvaLeRi3vUnique;
using SelectedOutline = Il2Cpp.MonoBehaviourPublicInLi1MeHaInMeRe1MeUnique;
using RoomManager = Il2Cpp.MonoBehaviourPublicBoApSiApBoObStApBo1Unique;
using Il2CppSystem.Collections.Generic;
using VRCMotionState = Il2Cpp.MonoBehaviourPublicLaSiBoSiChBoObVeBoSiUnique;

[assembly: MelonInfo(typeof(FM), "FlightMod", "1.0.0", "stolen from emm", null)]
[assembly: MelonGame("VRChat", "VRChat")]

namespace FlightMod
{
    public class ESP
    {
        private static bool _ESPEnabled;

        private static float timer;

        public static bool ESPEnabled
        {
            get
            {
                return _ESPEnabled;
            }
            set
            {
                _ESPEnabled = value;
                ApplyState();
            }
        }

        private static void ApplyState()
        {
            var playerManager = Il2Cpp.MonoBehaviourPublicStObStLi1DiOb2InObUnique.field_Private_Static_MonoBehaviourPublicStObStLi1DiOb2InObUnique_0;
            foreach (var current in playerManager.field_Private_List_1_MonoBehaviourPublicAPOb_vOb_pBo_UObBoVRUnique_0)
            {
                Transform val = current.transform.Find("SelectRegion");
                if (val != null)
                {
                    var renderer = val.GetComponent<Renderer>();
                    SelectedOutline.Method_Internal_Static_Void_Renderer_Boolean_PDM_0(renderer, _ESPEnabled);
                }
            }
        }

        public static void Update()
        {
            timer += Time.deltaTime;
            if (!(timer < 3f))
            {
                timer -= 3f;
                ApplyState();
            }
        }
    }
    internal class Flight
    {
        public static bool FlightEnabled;
        private static GameObject localPlayer;
        private static VRCMotionState motionState;
        // private static InputStateController stateController;
        private static CharacterController characterController;
        private static Vector3 originalGravity;

        public static bool NoclipEnabled
        {
            get
            {
                return !characterController.enabled;
            }
            set
            {
                characterController.enabled = !value;
            }
        }

        private static Il2Cpp.MonoBehaviour1PublicOb_pOb_s_pBoGaOb_pStUnique LocalPlayer
        {
            get
            {
                return Il2Cpp.MonoBehaviour1PublicOb_pOb_s_pBoGaOb_pStUnique.field_Internal_Static_MonoBehaviour1PublicOb_pOb_s_pBoGaOb_pStUnique_0;
            }
        }

        public static void Update()
        {
            if (RoomManager.field_Internal_Static_ApiWorld_0 == null || LocalPlayer == null)
            {
                return;
            }
            if (localPlayer == null)
            {
                localPlayer = LocalPlayer.gameObject;
                if (localPlayer == null)
                {
                    return;
                }
                if (motionState == null)
                {
                    motionState = localPlayer.GetComponent<VRCMotionState>();
                }
                // if (stateController == null)
                // {
                //     stateController = localPlayer.GetComponent<InputStateController>();
                // }
                if (characterController == null)
                {
                    characterController = localPlayer.GetComponent<CharacterController>();
                }
            }
            if (FlightEnabled)
            {
                if (Physics.gravity != Vector3.zero)
                {
                    originalGravity = Physics.gravity;
                    Physics.gravity = Vector3.zero;
                }
                var val = VRCPlayerApi.AllPlayers.Find((Il2CppSystem.Predicate<VRCPlayerApi>)(x => x.isLocal));
                if (!val.IsValid())
                {
                    return;
                }
                Vector3 val2 = Vector3.zero;
                if (IsXrPresent)
                {
                    var driver = ActionMenuExtra.GetDriver();
                    float num = Time.deltaTime * val.GetRunSpeed();
                    if (!driver.GetLeftOpener().isOpen())
                    {
                        val2 += localPlayer.transform.forward * Input.GetAxis("Vertical") * num;
                        val2 += localPlayer.transform.right * Input.GetAxis("Horizontal") * num;
                    }
                    if (!driver.GetRightOpener().isOpen())
                    {
                        val2 += new Vector3(0f, Input.GetAxis("Oculus_CrossPlatform_SecondaryThumbstickVertical") * num);
                    }
                }
                else
                {
                    float num2 = Time.deltaTime * (Input.GetKey((KeyCode)304) ? val.GetRunSpeed() : val.GetWalkSpeed());
                    val2 += Camera.main.transform.forward * Input.GetAxis("Vertical") * num2;
                    val2 += Camera.main.transform.right * Input.GetAxis("Horizontal") * num2;
                    if (Input.GetKey((KeyCode)113))
                    {
                        val2 -= new Vector3(0f, num2);
                    }
                    if (Input.GetKey((KeyCode)101))
                    {
                        val2 += new Vector3(0f, num2);
                    }
                }
                if (NoclipEnabled)
                {
                    localPlayer.transform.position += val2;
                }
                else
                {
                    localPlayer.transform.position += new Vector3(0f, val2.y);
                }
                if (motionState != null)
                {
                    motionState.Reset();
                }
                // if (stateController != null && !NoclipEnabled)
                // {
                //     stateController.ResetLastPosition();
                // }
            }
            else if (originalGravity != Vector3.zero)
            {
                Physics.gravity = originalGravity;
                originalGravity = Vector3.zero;
            }
        }

        public static bool? _isXrPresent = null;

        public static bool IsXrPresent
        {
            get
            {
                if (_isXrPresent is bool present) {
                    return present;
                }

                var xrDisplaySubsystems = new List<XRDisplaySubsystem>();
                SubsystemManager.GetInstances(xrDisplaySubsystems);
                foreach (var xrDisplay in xrDisplaySubsystems)
                {
                    if (xrDisplay.running)
                    {
                        return (_isXrPresent = true).Value;
                    }
                }
                return (_isXrPresent = false).Value;
            }
        }
    }
    public class FM : MelonMod
    {
        public override void OnSceneWasUnloaded(int buildIndex, string sceneName)
        {
            EconomyUdonBehaviourPath.OnLeavingWorld();
        }
        public override void OnInitializeMelon()
        {
            HarmonyInstance.PatchAll();
            // HarmonyInstance.PatchAll(typeof(EconomyListPurchasesPatch));
            // HarmonyInstance.PatchAll(typeof(EconomyDoesAnyPlayerOwnProductPatch));
            // HarmonyInstance.PatchAll(typeof(EconomyDoesPlayerOwnProductPatch));
            // HarmonyInstance.PatchAll(typeof(EconomyGetPlayersWhoOwnProductPatch));
            // HarmonyInstance.PatchAll(typeof(EconomyListProductOwnersPatch));
            // Helper.Patch();
            // ClassInjector.RegisterTypeInIl2Cpp<EconomyHaxx.UdonProductStub>(new RegisterTypeOptions { Interfaces = new[] { typeof(IProduct) } }); 
            AMUtils.AddToModsFolder("Cheats", menu =>
            {
                menu.AddButton("Purchase all", EconomyUdonBehaviourPath.PurchaseAll);
                menu.AddSubMenu("Player Modifiers", menu =>
                {
                    var player = Networking.LocalPlayer;
                    menu.AddRestrictedRadialPuppet("Jump Impulse", (value) => player.SetJumpImpulse(value * 100.0f), startingValue: player.GetJumpImpulse() / 100.0f);
                    menu.AddRestrictedRadialPuppet("Run Speed", (value) => player.SetRunSpeed(value * 100.0f), startingValue: player.GetRunSpeed() / 100.0f);
                    menu.AddRestrictedRadialPuppet("Walk Speed", (value) => player.SetWalkSpeed(value * 100.0f), startingValue: player.GetWalkSpeed() / 100.0f);
                    menu.AddRestrictedRadialPuppet("Strafe Speed", (value) => player.SetStrafeSpeed(value * 100.0f), startingValue: player.GetStrafeSpeed() / 100.0f);
                    menu.AddRestrictedRadialPuppet("Gravity Strength", (value) => player.SetGravityStrength(value * 100.0f), startingValue: player.GetGravityStrength() / 100.0f);
                });
                menu.AddSubMenu("Teleport", menu =>
                {
                    var self = VRCPlayerApi.AllPlayers.Find((Il2CppSystem.Predicate<VRCPlayerApi>)(x => x.isLocal));
                    foreach (var player in VRCPlayerApi.AllPlayers)
                    {
                        menu.AddButton(player.displayName, () =>
                        {
                            self.TeleportTo(player.GetPosition(), player.GetRotation());
                        });
                    }
                });
                menu.AddToggle("Flight", Flight.FlightEnabled, (Action<bool>)delegate (bool state)
                {
                    Flight.FlightEnabled = state;
                    if (!state)
                    {
                        Flight.NoclipEnabled = false;
                    }
                }, (Texture2D)null, false);
                menu.AddToggle("Noclip", Flight.NoclipEnabled, state => Flight.NoclipEnabled = state);
                menu.AddToggle("ESP", ESP.ESPEnabled, state => ESP.ESPEnabled = state);
                menu.AddToggle("FPS", PlayerInfo.FpsEnabled, state => PlayerInfo.FpsEnabled = state);
            });
        }

        public override void OnUpdate()
        {
            Flight.Update();
            PlayerInfo.Update();
            ESP.Update();
        }
    }
    public static class PlayerInfo
    {
        public static bool FpsEnabled;

        private static float timer;

        private static string NameplatePath = "Contents/Main/Text Container/Sub Text/Text";
        public static void Update()
        {
            if (!FpsEnabled)
            {
                return;
            }
            timer += Time.deltaTime;
            if (!(timer < 3f))
            {
                timer -= 3f;
                var playerManager = Il2Cpp.MonoBehaviourPublicStObStLi1DiOb2InObUnique.field_Private_Static_MonoBehaviourPublicStObStLi1DiOb2InObUnique_0;
                var nameplateManager = Il2Cpp.MonoBehaviourPublicOb1BoHaBoLi1ObCoTeUnique.field_Public_Static_MonoBehaviourPublicOb1BoHaBoLi1ObCoTeUnique_0;
                int i = 0;
                foreach (var current in playerManager.field_Private_List_1_MonoBehaviourPublicAPOb_vOb_pBo_UObBoVRUnique_0)
                {
                    short num = current._vrcplayer.prop_Int16_0;
                    byte field_Private_Byte_ = current._playerNet.prop_Byte_0;
                    float num2 = ((field_Private_Byte_ != 0) ? Mathf.Floor(1000f / (float)(int)field_Private_Byte_) : 0f);
                    // MelonLogger.Msg($"{current.Method_Internal_get_APIUser_0().get_displayName()}: {num}ms, {num2}fps");
                    var currentNameplate = nameplateManager.field_Private_List_1_MonoBehaviourPublicIDisposableSiInCoSiGaCoObSiGrCoUnique_0[i];
                    var gameObject = currentNameplate.transform.Find(NameplatePath).gameObject;
                    gameObject.active = true;
                    var component = gameObject.GetComponent<TextMeshProUGUI>();
                    component.text = $"{num}ms, {num2}fps";
                    component.fontSize = 14f;
                    i++;
                }
            }
        }
    }
    // internal static class Reflections
    // {
    //     private delegate void ResetLastPositionAction(InputStateController @this);

    //     private static ResetLastPositionAction ourResetLastPositionAction;

    //     private static ResetLastPositionAction ResetLastPositionAct
    //     {
    //         get
    //         {
    //             if (ourResetLastPositionAction != null)
    //             {
    //                 return ourResetLastPositionAction;
    //             }
    //             ourResetLastPositionAction = (ResetLastPositionAction)Delegate.CreateDelegate(typeof(ResetLastPositionAction), typeof(InputStateController).GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public).Single((MethodInfo it) => XrefScanner.XrefScan((MethodBase)it).Any((XrefInstance jt) => (int)jt.Type == 1 && jt.TryResolve()?.Name == "get_transform")));
    //             return ourResetLastPositionAction;
    //         }
    //     }

    //     public static void ResetLastPosition(this InputStateController instance)
    //     {
    //         ResetLastPositionAct(instance);
    //     }
    // }

    internal static class ActionMenuExtra
    {
        public static bool isOpen(this ActionMenuOpener actionMenuOpener)
        {
            return actionMenuOpener.field_Private_Boolean_0; //only bool on action menu opener, shouldnt change
        }

        public static ActionMenuType GetActionMenuType(this ActionMenuOpener opener)
        {
            return opener.field_Public_EnumNPublicSealedvaLeRi3vUnique_0;
        }

        public static ActionMenuDriver GetDriver()
        {
            // return ActionMenuDriver.field_Public_Static_MonoBehaviourPublicObGaObAcCoObMeEmObExUnique_0;
            return ActionMenuDriver.field_Public_Static_MonoBehaviourPublicObGaObAc1ObAcBoCoObUnique_0;
        }

        public static ActionMenuOpener GetLeftOpener(this ActionMenuDriver actionMenuDriver)
        {
            // var opener = actionMenuDriver.field_Public_MonoBehaviourPublicObBoSiObObObUnique_0;
            var opener = actionMenuDriver.field_Public_MonoBehaviourPublicCaObAc1BoSiBoObObObUnique_0;
            if (opener.GetActionMenuType() ==
                ActionMenuType.Left)
                return opener;
            return actionMenuDriver.field_Public_MonoBehaviourPublicCaObAc1BoSiBoObObObUnique_1;
        }

        public static ActionMenuOpener GetRightOpener(this ActionMenuDriver actionMenuDriver)
        {
            var opener = actionMenuDriver.field_Public_MonoBehaviourPublicCaObAc1BoSiBoObObObUnique_1;
            if (opener.GetActionMenuType() == ActionMenuType.Right)
                return opener;
            return actionMenuDriver.field_Public_MonoBehaviourPublicCaObAc1BoSiBoObObObUnique_0;
        }

        public static ActionMenuOpener GetActionMenuOpener()
        {
            var driver = GetDriver();
            if (!driver.GetLeftOpener().isOpen() &&
                driver.GetRightOpener().isOpen())
                return driver.GetRightOpener();

            if (driver.GetLeftOpener().isOpen() &&
                !driver.GetRightOpener().isOpen())
                return driver.GetLeftOpener();

            return null;
        }
    }
}
