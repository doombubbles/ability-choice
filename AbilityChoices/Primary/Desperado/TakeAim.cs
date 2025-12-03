using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
namespace AbilityChoice.AbilityChoices.Primary.Desperado;

public class TakeAim : TowerAbilityChoice
{
    public override string UpgradeId => UpgradeType.Deadeye;

    public override string Description1 => "Modified Ability: Permanently boost one tower at a time. Desperado gains a long range rifle to use against distant targets. Rifle shots excel against Fortified targets and can pop Lead Bloons. ";

    public override string Description2 => "All towers in range have camo detection and slightly increased attack range. Desperado gains a long range rifle to use against distant targets. Rifle shots excel against Fortified targets and can pop Lead Bloons. ";

    protected virtual float Multiplier => 0.05f;

    public override string BackUpAbilityName => "TakeAim2";

    public override void Apply1(TowerModel model)
    {
        // see OverclockHandler
        model.GetDescendant<TakeAimModel>().SetName(OverclockHandler.Enabled);
    }

    public override void Apply2(TowerModel model)
    {
        var takeAim = AbilityModel(model).GetBehavior<TakeAimModel>();

        var range = model.GetAttackModel("Rifle").range;

        model.AddBehavior(new RangeSupportModel("", true, Multiplier, 0,
            takeAim.Mutator.saveId, null, false, takeAim.buffLocsName, takeAim.buffIconName)
        {
            appliesToOwningTower = true,
            showBuffIcon = true,
            isCustomRadius = true,
            customRadius = range
        });

        model.AddBehavior(new VisibilitySupportModel("", true, takeAim.Mutator.saveId + "2", false, null, takeAim.buffLocsName, takeAim.buffIconName)
        {
            appliesToOwningTower = false,
            showBuffIcon = true,
            isCustomRadius = true,
            customRadius = range
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