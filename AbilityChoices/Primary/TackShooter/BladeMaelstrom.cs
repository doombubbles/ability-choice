﻿using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Weapons.Behaviors;
using Il2CppAssets.Scripts.Unity;
namespace AbilityChoice.AbilityChoices.Primary.TackShooter;

public class BladeMaelstrom : TowerAbilityChoice
{
    public override string UpgradeId => UpgradeType.BladeMaelstrom;
    public override string Description1 => "Shoots out a slow swirl of global, high pierce blades.";

    public override string Description2 =>
        "Blades have additional range and pierce, and seek out Bloons on their own.";

    protected virtual int Pierce => 6;
    protected virtual float Lifespan => 4;

    public override void Apply1(TowerModel model)
    {
        var abilityModel = AbilityModel(model);

        var activateAttackModel = abilityModel.GetBehavior<ActivateAttackModel>();
        var newAttack = activateAttackModel.attacks[0].Duplicate();

        var uptime = activateAttackModel.Lifespan / abilityModel.Cooldown;
        newAttack.weapons[0].Rate /= uptime;
        newAttack.GetDescendant<SpinModel>().rotationPerFrame *= uptime;
        newAttack.GetDescendant<SpinModel>().rotationPerSecond *= uptime;

        model.AddBehavior(newAttack);
    }

    public override void Apply2(TowerModel model)
    {
        model.range += 9;
        model.GetAttackModel().range += 9;

        var neva = Game.instance.model.GetTower(TowerType.MonkeyAce, 0, 0, 3);
        var behavior = neva.GetDescendant<AdoraTrackTargetModel>().Duplicate();

        var weaponProjectile = model.GetWeapon().projectile;
        weaponProjectile.AddBehavior(behavior);
        weaponProjectile.pierce += Pierce;
        weaponProjectile.GetBehavior<TravelStraitModel>().Lifespan *= Lifespan;
    }
}