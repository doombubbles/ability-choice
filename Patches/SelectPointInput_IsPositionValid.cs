using AbilityChoice.Displays;
using HarmonyLib;
using Il2CppAssets.Scripts.Models.Map;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using UnityEngine;

namespace AbilityChoice.Patches;

[HarmonyPatch(typeof(SelectPointInput), nameof(SelectPointInput.IsPositionValid))]
internal static class SelectPointInput_IsPositionValid
{
    [HarmonyPostfix]
    private static void Postfix(SelectPointInput __instance, Vector2 cursorPosWorld, ref bool __result)
    {
        var selectTargetCiData = __instance.cii;

        if (selectTargetCiData.targetInvalidImageId?.GUID == ModContent.GetDisplayGUID<MegaMineInvalid>())
        {
            var point = new Il2CppAssets.Scripts.Simulation.SMath.Vector2(cursorPosWorld);
            __result &= InGame.instance.bridge.Simulation.Map.GetAreaAtPoint(point)?.areaModel?.type == AreaType.water;
        }
    }
}