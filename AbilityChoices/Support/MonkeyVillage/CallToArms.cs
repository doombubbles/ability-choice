using System;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.TowerFilters;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
namespace AbilityChoice.AbilityChoices.Support.MonkeyVillage;

public class CallToArms : TowerAbilityChoice
{
    public override string AbilityName => UpgradeType.CallToArms;

    public override string UpgradeId => UpgradeType.CallToArms;

    public override string Description1 => "Permanent weaker nearby attack speed / pierce buff.";

    public override string Description2 => "Permanent moderate nearby attack speed.";

    protected virtual bool IsGlobal => true;

    public override void Apply1(TowerModel model)
    {
        var ability = AbilityModel(model);

        var c2a = ability.GetBehavior<CallToArmsModel>();
        var buffIndicator = c2a.Mutator.buffIndicator;

        var bonus = CalcAvgBonus(c2a.Lifespan / ability.Cooldown, c2a.multiplier);

        var buff = RateSupportModel.Create(new()
        {
            name = Name,
            multiplier = 1 / bonus,
            isUnique = true,
            mutatorId = $"Village:{Name}",
            isGlobal = IsGlobal,
            priority = 1,
            filters = new Il2CppReferenceArray<TowerFilterModel>(0),
            buffLocsName = buffIndicator.buffName,
            buffIconName = buffIndicator.iconName,
            onlyShowBuffIfMutated = true
        });

        var buff2 = PiercePercentageSupportModel.Create(new()
        {
            name = Name,
            isUnique = true,
            percentIncrease = bonus,
            mutatorId = $"Village:{Name}2",
            filters = new Il2CppReferenceArray<TowerFilterModel>(0),
            isGlobal = IsGlobal,
            buffLocsName = buffIndicator.buffName,
            buffIconName = buffIndicator.iconName,
            priority = 1,
            onlyShowBuffIfMutated = true,
            showBuffIcon = false
        });


        model.AddBehavior(buff);
        model.AddBehavior(buff2);
    }

    public override void Apply2(TowerModel model)
    {
        var ability = AbilityModel(model);

        var c2a = ability.GetBehavior<CallToArmsModel>();
        var buffIndicator = c2a.Mutator.buffIndicator;

        var newBonus = Math.Pow(c2a.multiplier, AbilityChoiceMod.MoreBalanced ? 1.5 : 2);

        var bonus = CalcAvgBonus(c2a.Lifespan / ability.Cooldown, (float) newBonus);

        var buff = RateSupportModel.Create(new()
        {
            name = $"RateSupportModel_{Name}",
            multiplier = 1 / bonus,
            isUnique = true,
            mutatorId = $"Village:{Name}",
            isGlobal = IsGlobal,
            priority = 1,
            filters = new Il2CppReferenceArray<TowerFilterModel>(0),
            buffLocsName = buffIndicator.buffName,
            buffIconName = buffIndicator.iconName,
            onlyShowBuffIfMutated = true
        });

        model.AddBehavior(buff);
    }
}