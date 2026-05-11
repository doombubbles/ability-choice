using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Simulation.Towers;
using PrimordialWrathSim = Il2CppAssets.Scripts.Simulation.Towers.Behaviors.Abilities.Behaviors.PrimordialWrath;

namespace AbilityChoice.AbilityChoices.Magic.Druid;

public class PrimordialWrath : TowerAbilityChoice
{
    public override string UpgradeId => UpgradeType.DruidParagon;

    public override string Description1 =>
        "That from which all life branches. " +
        $"Toggle to forego its own income generation to go into a {Factor1:P0} effectiveness Wrathful state.";

    public override string Description2 =>
        "That from which all life branches. " +
        $"Permanently foregoes all of its own income generation to always be in a {Factor2:P0} effectiveness Wrathful state.";

    private const float Factor1 = .25f;
    private const float Factor2 = .3f;

    public override void Apply1(TowerModel model)
    {
        var ability = AbilityModel(model);

        ability.RemoveBehavior<ToggleEffectWhileAbilityActiveModel>();

        var primordialWrath = ability.GetBehavior<PrimordialWrathModel>();

        primordialWrath.depletionPercent = primordialWrath.depletionAsMultiplier =
            primordialWrath.cashPerSecondMin = primordialWrath.cashPerSecondMax = 0;

        primordialWrath.maxMultiplier = 1 + (primordialWrath.maxMultiplier - 1) * Factor1;
        primordialWrath.damageMultiplierPerThreshold *= Factor1;
        primordialWrath.rangeMultiplier = 1 + (primordialWrath.rangeMultiplier - 1) * Factor1;
        primordialWrath.attackSpreadMultiplier = 1 - (1 - primordialWrath.attackSpreadMultiplier) * Factor1;

        primordialWrath.wrathActiveSound = null;
        primordialWrath.wrathInactiveSound = null;
    }

    public override void Apply2(TowerModel model)
    {
        model.RemoveBehavior<AbilityModel>("Passive");
        model.RemoveBehavior<BuffIconPerTowerInRangeModel>();

        var ability = AbilityModel(model);

        var primordialWrath = ability.GetBehavior<PrimordialWrathModel>();

        var paragonTower = model.GetBehavior<ParagonTowerModel>();
        paragonTower.RemoveChildDependants(paragonTower.displayDegreePaths);
        paragonTower.displayDegreePaths = primordialWrath.displayDegreePaths;
        paragonTower.AddChildDependants(paragonTower.displayDegreePaths);

        model.GetAttackModel().GetDescendant<ProjectileModel>()
            .SetDisplay(primordialWrath.wrathProjectileDisplay.assetPath);

        model.GetAttackModel("Spawner").GetDescendant<TowerModel>().GetDescendant<ProjectileModel>()
            .SetDisplay(primordialWrath.wrathSubtowerProjectileDisplay.assetPath);

        model.GetDescendants<DamageModifierPrimordialWrathModel>().ForEach(wrath =>
        {
            wrath.active = true;
            wrath.damageMultiplier *= Factor2;
            wrath.maxDamageMultiplier = 1 + (wrath.maxDamageMultiplier - 1) * Factor2;
        });

        model.range *= 1 + (primordialWrath.rangeMultiplier - 1) * Factor2;
        foreach (var attackModel in model.GetAttackModels())
        {
            attackModel.range *= 1 + (primordialWrath.rangeMultiplier - 1) * Factor2;
        }

        model.GetDescendants<ArcEmissionModel>().ForEach(emissionModel =>
        {
            emissionModel.angle *= 1 - (1 - primordialWrath.attackSpreadMultiplier) * Factor2;
        });
    }

    protected override void RemoveAbility(TowerModel model)
    {
        if (Mode2)
        {
            base.RemoveAbility(model);
        }
    }

    [HarmonyPatch(typeof(PrimordialWrathSim), nameof(PrimordialWrathSim.AddDebuff))]
    internal static class PrimordialWrathSim_AddDebuff
    {
        [HarmonyPrefix]
        internal static bool Prefix(PrimordialWrathSim __instance, Tower tower)
        {
            return !GetInstance<PrimordialWrath>().Enabled || __instance.ability.tower == tower;
        }
    }
}