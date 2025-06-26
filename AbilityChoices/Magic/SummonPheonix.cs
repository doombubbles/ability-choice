using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors;
using Il2CppAssets.Scripts.Unity;

namespace AbilityChoice.AbilityChoices.Magic;

public class SummonPheonix : TowerAbilityChoice // Yup this is an actual typo
{
    public override string UpgradeId => UpgradeType.SummonPhoenix;

    public override string Description1 => "Summons a Phoenix which wreaks a moderate amount of bloon havoc.";

    public override string Description2 => "Wizard gains the attacks of the Phoenix itself (non-globally).";

    public override void Apply1(TowerModel model)
    {
        var abilityModel = AbilityModel(model);
        var uptime = abilityModel.GetDescendant<TowerModel>().GetBehavior<TowerExpireModel>().Lifespan /
                     abilityModel.Cooldown;
        var lord = Game.instance.model.GetTower(TowerType.WizardMonkey, model.tiers[0], 5, model.tiers[2]);

        var permaBehavior = lord.GetBehavior<TowerCreateTowerModel>().Duplicate();

        permaBehavior.RemoveChildDependant(permaBehavior.towerModel);
        permaBehavior.towerModel = abilityModel.GetDescendant<TowerModel>();
        permaBehavior.AddChildDependant(permaBehavior.towerModel);

        permaBehavior.towerModel.GetWeapon().rate /= uptime;

        model.AddBehavior(permaBehavior);
    }

    public override void Apply2(TowerModel model)
    {
        model.range = model.GetBehaviors<AttackModel>().Max(attackModel => attackModel.range);

        var abilityModel = AbilityModel(model);

        var phoenix = abilityModel.GetDescendant<TowerModel>();

        AddAttacksFromSubTower(model, phoenix);
    }

    protected void AddAttacksFromSubTower(TowerModel model, TowerModel subTowerModel)
    {
        foreach (var attackModel in subTowerModel.GetAttackModels())
        {
            attackModel.range = model.range;
            attackModel.targetProvider = null;

            attackModel.RemoveBehavior<PathMovementFromScreenCenterModel>();
            attackModel.RemoveBehavior<PathMovementFromScreenCenterPatternModel>();

            foreach (var targetSupplierModel in model.GetAttackModel().GetBehaviors<TargetSupplierModel>())
            {
                attackModel.AddBehavior(targetSupplierModel);
                attackModel.fireWithoutTarget = false;
            }

            attackModel.weapons[0].ejectZ = 0;

            model.AddBehavior(attackModel);
        }
    }
}