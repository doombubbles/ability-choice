using HarmonyLib;
using Il2CppAssets.Scripts.Simulation.Towers.Behaviors;

namespace AbilityChoice.Patches;

[HarmonyPatch(typeof(ActivateAbilityAfterInterval), nameof(ActivateAbilityAfterInterval.Initialise))]
internal static class ActivateAbilityAfterInterval_Initialise
{
    [HarmonyPostfix]
    internal static void Postfix(ActivateAbilityAfterInterval __instance)
    {
        if (!__instance.model.name.Contains(nameof(AbilityChoice.TechBotify))) return;

        var ability = __instance.GetAbility();
        if (ability != null && !ability.abilityModel.startOffCooldown)
        {
            __instance.lastActivatedAt = __instance.Sim.roundTime.elapsed;
        }
    }
}