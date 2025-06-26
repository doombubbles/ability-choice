using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors;
namespace AbilityChoice.AbilityChoices.Military.MonkeyBuccaneer;

public class LargeBADHarpoon : TowerAbilityChoice
{
    public override string UpgradeId => UpgradeType.MonkeybuccaneerParagon;

    public override string Description1 => "The greatest thing ever to float on water. Occasionally uses a mega hook to destroy the strongest non-Boss Bloon on screen.";

    public override string Description2 => "The greatest thing ever to float on water. Can use nearly all of its hooks in order to destroy a BAD Bloon.";

    public override void Apply1(TowerModel model)
    {
        var ability = AbilityModel(model);
        var attack = ability.GetDescendant<AttackModel>();

        attack.weapons[0]!.Rate = ability.Cooldown;
        model.AddBehavior(attack);
    }

    public override void Apply2(TowerModel model)
    {
        model.GetDescendants<TargetGrapplableModel>().ForEach(grapplableModel => grapplableModel.badHooksRequired = 8);
    }
}