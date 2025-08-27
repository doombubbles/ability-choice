using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Unity;

namespace AbilityChoice.AbilityChoices.Magic.Druid;

public class JunglesBounty : TowerAbilityChoice
{
    public override string UpgradeId => UpgradeType.JunglesBounty;

    public override string AbilityName => "Jungle's Bounty";

    public override string Description1 => "Vines create stronger thorn piles. Periodically generates cash, plus extra per Banana Farm near the Druid.";

    public override string Description2 => "Vines create stronger thorn piles. Nearby income generation is increased by 20%.";

    protected virtual float Income => 1.2f;

    public override void Apply1(TowerModel model)
    {
        TechBotify(model);
    }

    public override void Apply2(TowerModel model)
    {
        var village = Game.instance.model.GetTower(TowerType.MonkeyVillage, 0, 0, 4);
        var behavior = village.GetBehavior<MonkeyCityIncomeSupportModel>().Duplicate();
        behavior.incomeModifier = Income;
        behavior.appliesToOwningTower = false;
        behavior.maxStackSize = 0;
        behavior.showBuffIcon = false;
        model.AddBehavior(behavior);
    }

    protected override void RemoveAbility(TowerModel model)
    {
        if (Mode2)
        {
            base.RemoveAbility(model);
        }
    }
}