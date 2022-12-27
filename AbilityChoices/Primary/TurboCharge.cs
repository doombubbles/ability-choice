using AbilityChoice.Displays;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Unity;
using BTD_Mod_Helper;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;

namespace AbilityChoice.AbilityChoices.Primary;

public class TurboCharge : AbilityChoice
{
    public override string UpgradeId => UpgradeType.TurboCharge;

    public override string Description1 => "Significantly increased attack speed.";

    public override string Description2 => "Moderate attack speed increase, and boomerangs shock Bloons.";

    public override void Apply1(TowerModel model)
    {
        var abilityModel = AbilityModel(model);
        var turbo = abilityModel.GetBehavior<TurboModel>();
        var damage = model.GetWeapon().projectile.GetDamageModel().damage;

        var bonus = CalcAvgBonus(turbo.Lifespan / abilityModel.Cooldown,
            (damage + turbo.extraDamage + 1) / (damage + 1) / turbo.multiplier);
        model.GetWeapon().Rate /= bonus;
    }

    public override void Apply2(TowerModel model)
    {
        var abilityModel = AbilityModel(model);
        var turbo = abilityModel.GetBehavior<TurboModel>();
        var projectile = model.GetWeapon().projectile;
        var damageModel = projectile.GetDamageModel();
        var damage = damageModel.damage;

        var damageBoostFactor = (damage + turbo.extraDamage) / damage;

        var bonus = CalcAvgBonus(turbo.Lifespan / abilityModel.Cooldown,
            (damage + turbo.extraDamage + 1) / (damage + 1) / turbo.multiplier);

        var rateBonus = bonus / damageBoostFactor;

        model.GetWeapon().Rate /= rateBonus;

        var dartling = Game.instance.model.GetTower(TowerType.DartlingGunner, 2);
        var dart = dartling.GetWeapon().projectile;

        var addBehaviorToBloonModel = dart.GetBehavior<AddBehaviorToBloonModel>().Duplicate();
        addBehaviorToBloonModel.overlayType = ElectricShock.OverlayType;
        addBehaviorToBloonModel.mutationId = nameof(ElectricShock);
        projectile.AddBehavior(addBehaviorToBloonModel);
        var damageModifierForBloonStateModel = dart.GetBehavior<DamageModifierForBloonStateModel>().Duplicate();
        damageModifierForBloonStateModel.bloonState = nameof(ElectricShock);
        damageModifierForBloonStateModel.bloonStates = new[] {nameof(ElectricShock)};
        projectile.AddBehavior(damageModifierForBloonStateModel);
        projectile.collisionPasses = new[] {0, 1};
    }
}