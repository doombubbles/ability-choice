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

    public override string Description2 => "All towers in range have increased Attack Speed.";

    protected virtual float Multiplier => .8f;

    protected override void Apply1(TowerModel model)
    {
        // see OverclockHandler
    }

    protected override void Apply2(TowerModel model)
    {
        var overclock = AbilityModel(model).GetBehavior<OverclockModel>();

        var rateSupport = new RateSupportModel("RateSupportModel_", Multiplier, true,
            overclock.mutatorSaveId, false, 1, null, overclock.buffLocsName, overclock.buffIconName)
        {
            appliesToOwningTower = true,
            showBuffIcon = true
        };

        model.AddBehavior(rateSupport);
    }

    protected override void RemoveAbility(TowerModel model)
    {
        if (Mode2)
        {
            base.RemoveAbility(model);
        }
    }
}