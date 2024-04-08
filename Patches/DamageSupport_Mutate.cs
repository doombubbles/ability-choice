using System;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Simulation.Towers.Behaviors;

namespace AbilityChoice.Patches;

[HarmonyPatch(typeof(DamageSupport.MutatorTower), nameof(DamageSupport.MutatorTower.Mutate))]
internal static class DamageSupport_Mutate
{
    [HarmonyPrefix]
    internal static bool Prefix(DamageSupport.MutatorTower __instance, Model model, ref bool __result)
    {
        if (__instance.increase < .99)
        {
            var mult = __instance.increase + 1;
            model.GetDescendants<DamageModel>()
                .ForEach(damageModel => damageModel.damage = (float) Math.Ceiling(damageModel.damage * mult));
            __result = true;
            return false;
        }

        return true;
    }
}