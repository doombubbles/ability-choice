using Il2CppAssets.Scripts.Unity.UI_New.Upgrade;
using Il2CppAssets.Scripts.Unity.Utils;
using Il2CppAssets.Scripts.Utils;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;

namespace AbilityChoice.Patches;

[HarmonyPatch(typeof(UpgradeDetails), nameof(UpgradeDetails.Update))]
internal static class UpgradeDetails_Update
{
    [HarmonyPostfix]
    private static void Postfix(UpgradeDetails __instance)
    {
        AbilityChoice.HandleIcon(__instance);
    }
}