using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;

namespace AbilityChoice.AbilityChoices.Magic;

public class SummonLordPhoenix : SummonPheonix
{
    public override string UpgradeId => UpgradeType.WizardLordPhoenix;

    public override string Description1 => "Summons a somewhat powerful Lava Phoenix.";

    public override string Description2 => "Wizard gains the attacks of both Phoenixes itself (non-globally).";

    protected override void Apply1(TowerModel model)
    {
        var permaBehavior = model.GetBehavior<TowerCreateTowerModel>().Duplicate();
        var abilityModel = AbilityModel(model);
        var uptime = abilityModel.GetDescendant<TowerModel>().GetBehavior<TowerExpireModel>().Lifespan /
                     abilityModel.Cooldown;
        var lordPhoenix = abilityModel.GetDescendant<TowerModel>();

        lordPhoenix.behaviors = lordPhoenix.behaviors.RemoveItemOfType<Model, TowerExpireModel>();
        foreach (var weaponModel in lordPhoenix.GetWeapons())
        {
            weaponModel.rate /= uptime;
        }

        permaBehavior.towerModel = lordPhoenix;

        model.AddBehavior(permaBehavior);
    }

    protected override void Apply2(TowerModel model)
    {
        base.Apply2(model);

        var towerCreateTowerModel = model.GetBehavior<TowerCreateTowerModel>();
        model.RemoveBehavior(towerCreateTowerModel);

        AddAttacksFromSubTower(model, towerCreateTowerModel.towerModel);
    }
}