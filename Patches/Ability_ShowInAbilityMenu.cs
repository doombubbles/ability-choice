using HarmonyLib;
using Il2CppAssets.Scripts.Simulation.Towers.Behaviors.Abilities;

namespace AbilityChoice.Patches;

[HarmonyPatch(typeof(Ability), nameof(Ability.ShowInAbilityMenu), MethodType.Getter)]
internal static class Ability_ShowInAbilityMenu
{
    [HarmonyPrefix]
    private static bool Prefix(Ability __instance, ref bool __result)
    {
        if (__instance.abilityModel.displayName.Contains(AbilityChoiceMod.DontShowAbilityKeyword))
        {
            __result = false;
            return false;
        }

        return true;
    }
}