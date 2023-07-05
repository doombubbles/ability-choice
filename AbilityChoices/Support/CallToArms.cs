using System;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.TowerFilters;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace AbilityChoice.AbilityChoices.Support;

public class CallToArms : TowerAbilityChoice
{
    public override string AbilityName => UpgradeType.CallToArms;

    public override string UpgradeId => UpgradeType.CallToArms;

    public override string Description1 => "Permanent weaker nearby attack speed / pierce buff.";

    public override string Description2 => "Permanent moderate nearby attack speed.";

    protected virtual bool IsGlobal => true;

    protected override void Apply1(TowerModel model)
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

        var buff2 = new PierceSupportModel($"PierceSupportModel_{Name}_MULT", true, bonus, $"Village:{Name}2",
            new Il2CppReferenceArray<TowerFilterModel>(0), IsGlobal, buffIndicator.buffName, buffIndicator.iconName)
        {
            onlyShowBuffIfMutated = true,
            showBuffIcon = false,
            isUnique = true
        };


        model.AddBehavior(buff);
        model.AddBehavior(buff2);
    }

    protected override void Apply2(TowerModel model)
    {
        var ability = AbilityModel(model);

        var c2a = ability.GetBehavior<CallToArmsModel>();
        var buffIndicator = c2a.Mutator.buffIndicator;

        var newBonus = Math.Pow(c2a.multiplier, AbilityChoiceMod.MoreBalanced ? 1.5 : 2);

        var bonus = CalcAvgBonus(c2a.Lifespan / ability.Cooldown, (float) newBonus);

        var buff = new RateSupportModel($"RateSupportModel_{Name}", 1 / bonus, true, $"Village:{Name}", IsGlobal, 1,
            new Il2CppReferenceArray<TowerFilterModel>(0), buffIndicator.buffName, buffIndicator.iconName)
        {
            onlyShowBuffIfMutated = true,
            isUnique = true
        };

        model.AddBehavior(buff);
    }
}