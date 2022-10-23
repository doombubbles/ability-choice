﻿using System;
using AbilityChoice.Displays;
using Assets.Scripts.Models.Towers;
using Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Assets.Scripts.Models.Towers.Projectiles.Behaviors;
using Assets.Scripts.Unity;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;

namespace AbilityChoice.AbilityChoices.Primary;

public class PermaCharge : AbilityChoice
{
    public override string UpgradeId => UpgradeType.PermaCharge;

    public override string Description1 =>
        "Perma Charge has permanent super fast attack speed, and a moderate damage increase.";

    public override string Description2 =>
        "Perma Charge has permanent super fast attack speed, and an even more powerful shock effect.";

    public override void Apply1(TowerModel model)
    {
        var abilityModel = AbilityModel(model);
        var damageUp = abilityModel.GetBehavior<DamageUpModel>();
        var damageModel = model.GetWeapon().projectile.GetDamageModel();

        var bonus = CalcAvgBonus(damageUp.lifespanFrames / (float) abilityModel.cooldownFrames,
            (damageUp.additionalDamage + damageModel.damage) / damageModel.damage);

        damageModel.damage += bonus;
    }

    public override void Apply2(TowerModel model)
    {
        var abilityModel = AbilityModel(model);
        var damageUp = abilityModel.GetBehavior<DamageUpModel>();
        var damageModel = model.GetWeapon().projectile.GetDamageModel();

        var bonus = CalcAvgBonus(damageUp.lifespanFrames / (float) abilityModel.cooldownFrames,
            (damageUp.additionalDamage + damageModel.damage) / damageModel.damage);
        var projectile = model.GetWeapon().projectile;

        var dartling = Game.instance.model.GetTower(TowerType.DartlingGunner, 2);
        var dart = dartling.GetWeapon().projectile;

        var addBehaviorToBloonModel = dart.GetBehavior<AddBehaviorToBloonModel>().Duplicate();
        addBehaviorToBloonModel.overlayType = nameof(ElectricShock);
        addBehaviorToBloonModel.mutationId = nameof(ElectricShock);
        addBehaviorToBloonModel.lifespan = 5;
        projectile.AddBehavior(addBehaviorToBloonModel);
        var damageModifierForBloonStateModel = dart.GetBehavior<DamageModifierForBloonStateModel>().Duplicate();
        damageModifierForBloonStateModel.bloonState = nameof(ElectricShock);
        damageModifierForBloonStateModel.bloonStates = new[] {nameof(ElectricShock)};
        damageModifierForBloonStateModel.damageAdditive += bonus - 1;
        projectile.AddBehavior(damageModifierForBloonStateModel);
        projectile.collisionPasses = new[] {0, 1};
    }
}