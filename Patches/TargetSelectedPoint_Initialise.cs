using AbilityChoice.Displays;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2CppAssets.Scripts.Simulation.Objects;
using Il2CppAssets.Scripts.Simulation.Towers.Behaviors.Attack.Behaviors;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;

namespace AbilityChoice.Patches;

[HarmonyPatch(typeof(TargetSelectedPoint), nameof(TargetSelectedPoint.Initialise))]
internal static class TargetSelectedPoint_Initialise
{
    [HarmonyPostfix]
    private static void Postfix(TargetSelectedPoint __instance, Entity target)
    {
        if (__instance.targetSelectedPointModel.displayInvalid?.AssetGUID != ModContent.GetDisplayGUID<MegaMineInvalid>())
            return;

        var tower = InGame.instance.bridge.Simulation.towerManager.GetTowers().ToList()
            .First(tower => tower.entity.Id == target.Parent.Id);

        __instance.SetTargetPosition(tower.Position, true);
    }
}