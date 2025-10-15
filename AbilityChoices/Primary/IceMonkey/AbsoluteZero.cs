using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.TowerFilters;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace AbilityChoice.AbilityChoices.Primary.IceMonkey;

public class AbsoluteZero : Snowstorm
{
    public override string UpgradeId => UpgradeType.AbsoluteZero;

    public override string Description1 =>
        "Periodically freezes all Bloons on screen for 2s. Also globally buffs the attack speed of Ice Monkeys.";

    public override string Description2 =>
        "Cold Aura slows MOABs even further. Also globally buffs the attack speed of Ice Monkeys.";

    protected override int Factor => 5;

    public override void Apply2(TowerModel model)
    {
        base.Apply2(model);

        model.GetBehaviors<SlowBloonsZoneModel>().Last().speedScale -= .1f;
    }

    protected override void ApplyBoth(TowerModel model)
    {
        var abilityModel = AbilityModel(model);
        var support = abilityModel.GetBehavior<ActivateRateSupportZoneModel>();

        var avgBuff = CalcAvgBonus(support.lifespan / abilityModel.Cooldown, 1 / support.rateModifier);

        var buff = new RateSupportModel("AbilityChoice", 1 / avgBuff, true,
            "AbsoluteZeroRateBuff2", true, 1,
            new Il2CppReferenceArray<TowerFilterModel>([
                new FilterInBaseTowerIdModel("", new Il2CppStringArray([TowerType.IceMonkey]))
            ])
            , support.buffLocsName, support.buffIconName)
        {
            isGlobal = true,
            maxStackSize = 2,
            appliesToOwningTower = true
        };
        model.AddBehavior(buff);
    }
}