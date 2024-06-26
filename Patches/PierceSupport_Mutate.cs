﻿using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using Il2CppAssets.Scripts.Simulation.Towers.Behaviors;

namespace AbilityChoice.Patches;

[HarmonyPatch(typeof(PierceSupport.MutatorTower), nameof(PierceSupport.MutatorTower.Mutate))]
internal static class PierceSupport_Mutate
{
    [HarmonyPrefix]
    internal static bool Prefix(PierceSupport.MutatorTower __instance, Model model, ref bool __result)
    {
        if (__instance.parent.pierceSupportModel.name.EndsWith("_MULT"))
        {
            var mult = __instance.parent.pierceSupportModel.pierce;
            model.GetDescendants<ProjectileModel>().ForEach(projectileModel => projectileModel.pierce *= mult);
            __result = true;
            return false;
        }

        return true;
    }
}