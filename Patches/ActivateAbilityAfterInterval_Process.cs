using HarmonyLib;
using Il2CppAssets.Scripts.Simulation.Towers.Behaviors;
namespace AbilityChoice.Patches;

[HarmonyPatch(typeof(ActivateAbilityAfterInterval), nameof(ActivateAbilityAfterInterval.Process))]
internal static class ActivateAbilityAfterInterval_Process
{
    [HarmonyPrefix]
    internal static void Prefix(ActivateAbilityAfterInterval __instance)
    {
        if (!__instance.Model.name.Contains(nameof(AbilityChoice.TechBotify))) return;

        var ability = __instance.GetAbility();

        if (ability != null)
        {
            __instance.activateAbilityAfterIntervalModel.interval = ability.abilityModel.cooldown;
            __instance.activateAbilityAfterIntervalModel.intervalFrames = ability.abilityModel.cooldownFrames;
        }
    }
}