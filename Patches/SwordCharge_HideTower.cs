using HarmonyLib;
using Il2CppAssets.Scripts.Simulation.Towers.Behaviors.Abilities.Behaviors;

namespace AbilityChoice.Patches;

[HarmonyPatch(typeof(SwordCharge), nameof(SwordCharge.HideTower))]
internal static class SwordCharge_HideTower
{
    [HarmonyPrefix]
    private static bool Prefix(SwordCharge __instance)
    {
        return __instance.swordChargeModel.effectAtEndModel != null;
    }
}