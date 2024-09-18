using System.Collections.Generic;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Weapons.Behaviors;
using Il2CppAssets.Scripts.Simulation.Towers.Behaviors;

namespace AbilityChoice.AbilityChoices.Hero.Rosalia;

public class FlightBoost : HeroAbilityChoice
{
    public override string HeroId => TowerType.Rosalia;

    public override Dictionary<int, string> Descriptions1 => new()
    {
        {7, "Upgraded Jetpack! Nearby aircraft fly faster & Rosalia pursues Bloons over the whole map! "},
        {18, "Upgraded Weapons! Rosalia always attacks using both weapons!"}
    };

    public override Dictionary<int, string> Descriptions2 => new()
    {
        {7, "Upgraded Weapons! Rosalia always attacks using both weapons!"},
        {18, "Upgraded Jetpack! Nearby aircraft fly faster & Rosalia pursues Bloons over the whole map!"}
    };

    public override void Apply1(TowerModel model)
    {
        AlwaysPursuit(model);
        if (model.tier >= 18)
        {
            BothWeapons(model);
        }
    }

    public override void Apply2(TowerModel model)
    {
        BothWeapons(model);
        if (model.tier >= 18)
        {
            AlwaysPursuit(model);
        }
    }

    private void AlwaysPursuit(TowerModel model)
    {
        model.GetDescendant<RosaliaMovementModel>().name += "Pursuit";

        var flightBoost = model.GetDescendant<FlightBoostAbilityModel>();

        var mutator = flightBoost.Mutator;
        model.AddBehavior(new RateSupportModel(nameof(FlightBoost), flightBoost.moveSpeedMultiplier, true,
            nameof(FlightBoost), false, mutator.priority, null, mutator.buffIndicator.buffName,
            mutator.buffIndicator.iconName));
    }

    private void BothWeapons(TowerModel model)
    {
        model.GetDescendant<SwapProjectileModel>().emitBoth = true;
    }


    [HarmonyPatch(typeof(RosaliaMovement), nameof(RosaliaMovement.Process))]
    internal static class RosaliaMovement_Process
    {
        [HarmonyPrefix]
        internal static void Prefix(RosaliaMovement __instance)
        {
            if (__instance.rosaliaMovementModel.name.Contains("Pursuit"))
            {
                __instance.TogglePursuit(true);
            }
        }
    }

    [HarmonyPatch(typeof(RateSupportModel.RateSupportMutator), nameof(RateSupportModel.RateSupportMutator.Mutate))]
    internal static class RateSupportModel_RateSupportMutator_Mutate
    {
        [HarmonyPrefix]
        internal static bool Prefix(RateSupportModel.RateSupportMutator __instance, Model model, ref bool __result)
        {
            if (__instance.id != nameof(FlightBoost)) return true;
            
            var mult = __instance.multiplier;
            
            model.GetDescendants<HeliMovementModel>().ForEach(movementModel =>
            {
                movementModel.maxSpeed *= mult;
                movementModel.movementForceStart *= mult;
                movementModel.movementForceEnd *= mult;
                movementModel.brakeForce *= mult;
                movementModel.otherHeliRepulsonForce *= mult;
            });
            
            model.GetDescendants<PathMovementModel>().ForEach(movementModel =>
            {
                movementModel.speed *= mult;
                movementModel.rotation *= mult;
                movementModel.bankRotation *= mult;
            });

            __result = true;
            return false;
        }
    }
}