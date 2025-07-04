﻿using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Weapons;
namespace AbilityChoice.AbilityChoices.Primary.BombShooter;

public class AssassinateMOAB : TowerAbilityChoice
{
    private const float ExpectedPierce = 4;
    private const float Factor = 10;

    public override string UpgradeId => UpgradeType.MOABAssassin;

    public override string Description1 =>
        "Frequently shoots out mini Moab Assassin missiles at the strongest Moab on screen.";

    public override string Description2 => "Main attacks do further increased MOAB damage with more range.";

    public override void Apply1(TowerModel model)
    {
        var abilityModel = AbilityModel(model);

        var newAttack = abilityModel.GetDescendant<AttackModel>().Duplicate();

        var newAttackWeapon = newAttack.weapons[0];
        var damageModel = newAttackWeapon.projectile.GetDamageModel();
        damageModel.damage /= Factor;

        newAttackWeapon.Rate = abilityModel.Cooldown / Factor;

        newAttackWeapon.projectile.scale = 0.5f;

        newAttack.GetDescendant<CreateProjectileOnContactModel>().projectile.GetDamageModel().damage = 1;

        model.AddBehavior(newAttack);
    }

    public override void Apply2(TowerModel model)
    {
        var abilityModel = AbilityModel(model);
        var baseWeaponRate = BaseTowerModel.GetWeapon().Rate;

        var proj = model.GetWeapon().GetDescendant<CreateProjectileOnContactModel>().projectile;

        var abilityDamage = abilityModel.GetDescendant<WeaponModel>().projectile.GetDamageModel();
        var addedMoabDps = abilityDamage.damage / abilityModel.Cooldown;
        foreach (var moabDamage in model.FindDescendants<DamageModifierForTagModel>(tag => tag.tag == BloonTag.Moabs))
        {
            moabDamage.damageAddative += addedMoabDps * baseWeaponRate / ExpectedPierce;
        }

        var splash = abilityModel.GetDescendant<CreateProjectileOnContactModel>().projectile;
        var addedSplashDps = splash.pierce * splash.GetDamageModel().damage / abilityModel.Cooldown;
        proj.GetDamageModel().damage += addedSplashDps * baseWeaponRate / ExpectedPierce;

        model.range += 5;
        model.GetDescendants<AttackModel>().ForEach(attackModel => attackModel.range += 5);
    }
}