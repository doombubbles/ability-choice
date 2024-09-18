﻿using System.Collections.Generic;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Filters;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Weapons;

namespace AbilityChoice.AbilityChoices.Hero.Benjamin;

public class SyphonFunding : HeroAbilityChoice
{
    public override string HeroId => TowerType.Benjamin;

    public override Dictionary<int, string> Descriptions1 => new()
    {
        { 10, "Every few bloons each round get downgraded by 1 rank and give double cash per pop." },
        { 20, "More Bloons get downgraded, and they now give triple cash per pop." }
    };

    public override Dictionary<int, string> Descriptions2 => new()
    {
        { 10, "Bloon Trojan gives more cash and has Strong targeting priority instead of random." },
        { 16, "Bloon Trojan is sent more often and earns even more cash." },
        { 20, "Bloon Trojan can affect ZOMGs, and earns tons of cash." }
    };

    public override void Apply1(TowerModel model)
    {
        var ability = AbilityModel(model);
        var syphonFunding = ability.GetBehavior<SyphonFundingModel>();

        syphonFunding.lifespan /= ability.Cooldown;

        model.AddBehavior(syphonFunding);

        // See Syphon_OnBloonCreate
    }

    public override void Apply2(TowerModel model)
    {
        model.RemoveBehavior<SyphonModel>();

        var trojanAttack = model.GetAttackModel("BloonTrojan");
        var trojanWeapon = trojanAttack.GetChild<WeaponModel>();
        var trojanProj = trojanWeapon.projectile;

        trojanProj.GetBehavior<IncreaseBloonWorthWithTierModel>().cashPerTier = model.tier switch
        {
            < 16 => 2,
            < 20 => 5,
            _ => 10
        };

        trojanAttack.RemoveBehavior<RandomTargetModel>();
        trojanAttack.targetProvider = new TargetStrongModel("", false, false);
        trojanAttack.AddBehavior(trojanAttack.targetProvider);

        if (model.tier >= 20)
        {
            var attackFilter = trojanAttack.GetBehavior<AttackFilterModel>();
            attackFilter.filters = attackFilter.filters.RemoveItem(attackFilter.filters
                .OfIl2CppType<FilterOutTagModel>().First(filter => filter.tag == BloonTag.Zomg));

            var projectileFilter = trojanProj.GetBehavior<ProjectileFilterModel>();
            projectileFilter.filters = projectileFilter.filters.RemoveItem(projectileFilter.filters
                .OfIl2CppType<FilterOutTagModel>().First(filter => filter.tag == BloonTag.Zomg));

            trojanProj.filters = trojanProj.filters.RemoveItem(trojanProj.filters.OfIl2CppType<FilterOutTagModel>()
                .First(filter => filter.tag == BloonTag.Zomg));
        }
    }
}