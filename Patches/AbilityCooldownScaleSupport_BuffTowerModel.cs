using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Simulation.SMath;
using Il2CppAssets.Scripts.Simulation.Towers.Behaviors;

namespace AbilityChoice.Patches;

[HarmonyPatch(typeof(AbilityCooldownScaleSupport.MutatorTower),
    nameof(AbilityCooldownScaleSupport.MutatorTower.BuffTowerModel))]
internal static class AbilityCooldownScaleSupport_BuffTowerModel
{
    [HarmonyPostfix]
    internal static void Postfix(AbilityCooldownScaleSupport.MutatorTower __instance, TowerModel towerModel)
    {
        towerModel.GetDescendants<ActivateAbilityAfterIntervalModel>().ForEach(i =>
        {
            i.interval /= __instance.abilityCooldownSpeedScale;
            i.intervalFrames = Math.CeilToInt(i.intervalFrames / __instance.abilityCooldownSpeedScale);
        });
    }
}