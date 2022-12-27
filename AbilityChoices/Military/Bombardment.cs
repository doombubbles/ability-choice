using Il2CppAssets.Scripts.Models.GenericBehaviors;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;

namespace AbilityChoice.AbilityChoices.Military;

public class Bombardment : AbilityChoice
{
    public override string UpgradeId => UpgradeType.ArtilleryBattery;

    public override string Description1 => "Main attack upgrades to 3+ barrels for extremely fast attacks.";
    
    public override string Description2 => "Main attack upgrades to 3+ barrels for incredibly fast attacks with expanded radius.";

    public override void Apply1(TowerModel model)
    {
        var abilityModel = AbilityModel(model);
        var turbo = abilityModel.GetBehavior<TurboModel>();

        var bonus = CalcAvgBonus(turbo.Lifespan / abilityModel.Cooldown, 1 / turbo.multiplier);
        model.GetWeapon().Rate /= bonus;
    }

    public override void Apply2(TowerModel model)
    {
        var abilityModel = AbilityModel(model);
        var turbo = abilityModel.GetBehavior<TurboModel>();

        var bonus = turbo.projectileRadiusScaleBonus;
        
        model.GetDescendants<ProjectileModel>().ForEach(projectileModel =>
        {
            if (projectileModel.HasBehavior<DisplayModel>(out var displayModel))
            {
                displayModel.scale *= 1 + bonus;
            }
            projectileModel.radius *= 1 + bonus;
        });
    }
}