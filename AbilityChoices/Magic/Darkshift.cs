using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;

namespace AbilityChoice.AbilityChoices.Magic;

public class Darkshift : TowerAbilityChoice
{
    public override string UpgradeId => UpgradeType.DarkKnight;

    public override string Description1 =>
        "Dark blades increase knockback and pierce and deal extra damage to MOAB-class Bloons. Increased range.";

    protected override void Apply1(TowerModel model)
    {
        model.IncreaseRange(10);
    }
}