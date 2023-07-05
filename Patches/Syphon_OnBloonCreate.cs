using System;
using HarmonyLib;
using Il2CppAssets.Scripts.Simulation.Bloons;
using Il2CppAssets.Scripts.Simulation.Towers.Behaviors;
using Il2CppAssets.Scripts.Simulation.Towers.Behaviors.Abilities.Behaviors;

namespace AbilityChoice.Patches;

[HarmonyPatch(typeof(Syphon), nameof(Syphon.OnBloonCreate))]
internal static class Syphon_OnBloonCreate
{
    public static int counter;

    [HarmonyPrefix]
    private static void Prefix(Syphon __instance, Bloon bloon)
    {
        var syphonFunding = __instance.entity.GetBehavior<SyphonFunding>();
        if (syphonFunding == null || __instance.syphonModel.ignoreTags.Any(bloon.HasTag)) return;

        var everyHowMany = (int) Math.Round(1 / syphonFunding.syphonFundingModel.lifespan);

        // The SyphonFundingModel is on the tower and not the ability
        __instance.syphonFunding = syphonFunding;

        syphonFunding.active = counter % everyHowMany == 0;

        counter++;
    }
}