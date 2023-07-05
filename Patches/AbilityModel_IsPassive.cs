using HarmonyLib;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities;

namespace AbilityChoice.Patches;

[HarmonyPatch(typeof(AbilityModel), nameof(AbilityModel.IsPassive), MethodType.Getter)]
internal static class AbilityModel_IsPassive
{
    [HarmonyPrefix]
    private static bool Prefix(AbilityModel __instance, ref bool __result)
    {
        if (__instance.CooldownSpeedScale < 0)
        {
            __result = true;
            return false;
        }

        return true;
    }
}