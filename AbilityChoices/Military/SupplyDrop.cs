using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;

namespace AbilityChoice.AbilityChoices.Military;

public class SupplyDrop : TowerAbilityChoice
{
    public override string UpgradeId => UpgradeType.SupplyDrop;

    public override string Description1 =>
        "Occasionally drops crates of cash. Regular attack also damages Lead Bloons and increases Shrapnel popping power.";

    public override string Description2 =>
        "Bullets can bounce twice as much. Regular attack also damages Lead Bloons and increases Shrapnel popping power.";


    protected override void Apply1(TowerModel model)
    {
        TechBotify(model);
    }

    protected override void Apply2(TowerModel model)
    {
        model.GetWeapon().projectile.pierce *= 2;
    }

    protected override void RemoveAbility(TowerModel model)
    {
        if (Mode2)
        {
            base.RemoveAbility(model);
        }
    }
}