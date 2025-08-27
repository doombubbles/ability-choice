using HarmonyLib;
using Il2CppAssets.Scripts.Simulation.SMath;
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
            var cooldown = ability.abilityModel.Cooldown * (1 - ability.abilityModel.CooldownSpeedScale);

            var model = __instance.activateAbilityAfterIntervalModel;

            model.interval = Math.Min(model.interval, cooldown);
            model.intervalFrames = Math.Min(model.intervalFrames, (int) Math.Round(cooldown * 60));
        }
    }
}