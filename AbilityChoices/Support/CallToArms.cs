﻿using Assets.Scripts.Models.Towers;
using Assets.Scripts.Models.Towers.Behaviors;
using Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Assets.Scripts.Models.Towers.TowerFilters;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using UnhollowerBaseLib;

namespace AbilityChoice.AbilityChoices.Support;

public class CallToArms : AbilityChoice
{
    public override string AbilityName => UpgradeType.CallToArms;

    public override string UpgradeId => UpgradeType.CallToArms;

    public override string Description1 => "Permanent weaker nearby attack speed / pierce buff.";
    
    public override string Description2 => "Permanent moderate nearby attack speed.";

    protected virtual bool IsGlobal => false;

    public override void Apply1(TowerModel model)
    {
        var ability = AbilityModel(model);

        var c2a = ability.GetBehavior<CallToArmsModel>();
        var buffIndicator = c2a.Mutator.buffIndicator;

        var bonus = CalcAvgBonus(c2a.Lifespan / ability.Cooldown, c2a.multiplier);

        var buff = new RateSupportModel($"RateSupportModel_{Name}", 1 / bonus, true, $"Village:{Name}", IsGlobal, 1,
            new Il2CppReferenceArray<TowerFilterModel>(0), buffIndicator.buffName, buffIndicator.iconName)
        {
            onlyShowBuffIfMutated = true,
            isUnique = true
        };

        var buff2 = new PierceSupportModel($"MultPierceSupportModel_{Name}", true, bonus, $"Village:{Name}2",
            new Il2CppReferenceArray<TowerFilterModel>(0), IsGlobal, buffIndicator.buffName, buffIndicator.iconName)
        {
            onlyShowBuffIfMutated = true,
            showBuffIcon = false,
            isUnique = true
        };


        model.AddBehavior(buff);
        model.AddBehavior(buff2);
    }

    public override void Apply2(TowerModel model)
    {
        var ability = AbilityModel(model);

        var c2a = ability.GetBehavior<CallToArmsModel>();
        var buffIndicator = c2a.Mutator.buffIndicator;

        var bonus = CalcAvgBonus(c2a.Lifespan / ability.Cooldown, c2a.multiplier * c2a.multiplier);

        var buff = new RateSupportModel($"RateSupportModel_{Name}", 1 / bonus, true, $"Village:{Name}", IsGlobal, 1,
            new Il2CppReferenceArray<TowerFilterModel>(0), buffIndicator.buffName, buffIndicator.iconName)
        {
            onlyShowBuffIfMutated = true,
            isUnique = true
        };

        model.AddBehavior(buff);
    }
}