using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppSystem.IO;
namespace AbilityChoice.AbilityChoices.Primary.Desperado;

public class MarkedToPop : TowerAbilityChoice
{
    public override string UpgradeId => UpgradeType.BountyHunter;

    public override string Description1 =>
        "Periodically Mark Bloons and make extra Execute attacks against Marked Bloons. " +
        "Marked Bloons take more damage and give more cash. " +
        "All Desperados can target Marked Bloons. ";

    public override string Description2 => "Desperado's main attacks have a chance to Mark Bloons for Execution. " +
                                           "Marked Bloons take more damage and give more cash. " +
                                           "All Desperados can target Marked Bloons. ";

    private const float Chance = 0.5f;

    public override void Apply1(TowerModel model)
    {
        var abilityModel = AbilityModel(model);
        var mtp = abilityModel.GetBehavior<MarkedToPopModel>();

        var markAttack = mtp.markingAttackModel.Duplicate();
        markAttack.weapons[0]!.Rate = abilityModel.Cooldown /
                                      (mtp.markingTimeFrames / (mtp.markingAttackModel.weapons[0]!.Rate * 60)); // ~10
        model.AddBehavior(markAttack);

        var executeAttack = mtp.executionAttackModel.Duplicate();
        executeAttack.weapons[0]!.Rate = abilityModel.Cooldown /
                                         (mtp.executionTimeMaxFrames /
                                          (mtp.executionAttackModel.weapons[0]!.Rate * 60)); // ~20
        model.AddBehavior(executeAttack);
    }

    public override void Apply2(TowerModel model)
    {
        var abilityModel = AbilityModel(model);
        var mtp = abilityModel.GetBehavior<MarkedToPopModel>();


        foreach (var attackModel in model.GetAttackModels())
        {
            foreach (var p in attackModel.GetDescendants<ProjectileModel>().ToArray()
                         .Where(p => p.GetDamageModel() != null))
            {
                var behavior = mtp.markingAttackModel.GetDescendant<AddBehaviorToBloonModel>().Duplicate();
                behavior.chance = Chance;
                p.AddBehavior(behavior);
            }
        }
    }
}