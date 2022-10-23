using Assets.Scripts.Models.Towers;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;

namespace AbilityChoice.AbilityChoices.Primary;

public class GlueStorm : GlueStrike
{
    public override string UpgradeId => UpgradeType.GlueStorm;
    
    public override string Description1 => "Frequently glues all bloons on screen, making them take increased damage.";

    public override string Description2 => "Glue weakens and slows Bloons further. Range is increased.";

    protected override float Factor => 7.5f;

    public override void Apply2(TowerModel model)
    {
        base.Apply1(model);
        model.range *= 2;
        model.GetAttackModel().range *= 2;
        model.GetWeapon().rate /= 2f;
    }
}