using System.Collections.Generic;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions;
using Il2CppAssets.Scripts.Simulation.Towers.Behaviors;

namespace AbilityChoice.AbilityChoices.Hero;

public class TargetFocus : HeroAbilityChoice
{
    public override string HeroId => TowerType.StrikerJones;

    public override Dictionary<int, string> Descriptions1 => new()
    {
        {
            7,
            "Pops 2 layers per shot and Mortar Monkeys blast radius increased by 10% and accuracy increased by 25%."
        }
    };

    [HarmonyPatch(typeof(ProjectileRadiusSupport.MutatorTower), nameof(ProjectileRadiusSupport.MutatorTower.Mutate))]
    internal static class ProjectileRadiusSupport_MutatorTower_Mutate
    {
        [HarmonyPrefix]
        private static void Prefix(ProjectileRadiusSupport.MutatorTower __instance, Model model)
        {
            if (__instance.id != "StrikerJonesProjectileRadiusBuff" ||
                !model.Is(out TowerModel towerModel) ||
                towerModel.baseId != TowerType.MortarMonkey ||
                !GetInstance<TargetFocus>().Enabled)
            {
                return;
            }
            
            model.GetDescendants<RandomTargetSpreadModel>().ForEach(spreadModel => spreadModel.spread *= .75f);
        }
    }
}