using Assets.Scripts.Models.Towers;
using Assets.Scripts.Models.Towers.Behaviors;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;

namespace AbilityChoice.AbilityChoices.Support;

public class IMFLoan : AbilityChoice
{
    public override string UpgradeId => UpgradeType.IMFLoan;
    public override string AbilityName => UpgradeId;

    public override string Description1 => "Periodically gives you $10,000 that has to be paid back over time.";
    public override string Description2 => "Bank capacity is increased to a base of $17,500";

    public override void Apply1(TowerModel model)
    {
        TechBotify(model);
    }

    public override void Apply2(TowerModel model)
    {
        model.GetBehavior<BankModel>().capacity += 7500;
    }

    public override void RemoveAbility(TowerModel model)
    {
        if (Mode2)
        {
            base.RemoveAbility(model);
        }
    }
}