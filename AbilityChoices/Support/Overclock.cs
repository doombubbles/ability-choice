using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;

namespace AbilityChoice.AbilityChoices.Support;

public class Overclock : TowerAbilityChoice
{
    public override string UpgradeId => UpgradeType.Overclock;

    public override string Description1 =>
        "Modified Ability: Permanently boost (based on tier) one tower at a time.";

    public override string Description2 => "All towers in range are partially overclocked.";

    protected virtual float Multiplier => .3f;

    public override void Apply1(TowerModel model)
    {
        // see OverclockHandler
        model.GetDescendant<OverclockModel>().name += OverclockHandler.Enabled;
    }

    public override void Apply2(TowerModel model)
    {
        var overclock = AbilityModel(model).GetBehavior<OverclockModel>();

        model.AddBehavior(new RangeSupportModel(nameof(Overclock), true,
            1 / CalcAvgBonus(Multiplier, 1 / overclock.rateModifier),
            CalcAvgBonus(Multiplier, overclock.villageRangeModifier), overclock.mutatorId, null,
            false, overclock.buffLocsName, overclock.buffIconName)
        {
            appliesToOwningTower = true,
            showBuffIcon = true,
        });
    }

    protected override void RemoveAbility(TowerModel model)
    {
        if (Mode2)
        {
            base.RemoveAbility(model);
        }
    }
}