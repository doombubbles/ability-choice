using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Unity;

namespace AbilityChoice.AbilityChoices.Support;

public class SpikeStorm : TowerAbilityChoice
{
    public override string UpgradeId => UpgradeType.SpikeStorm;

    public override string Description1 => "Continuously shoots out a stream of spikes over the entire track.";

    public override string Description2 =>
        "Gains extremely accelerated production for the first few seconds of each round.";

    public override void Apply1(TowerModel model)
    {
        var ability = AbilityModel(model);

        var attacks = ability.GetBehavior<ActivateAttackModel>().attacks;
        var abilityAttack = attacks[0].Duplicate();
        var abilityWeapon = abilityAttack.weapons[0];
        abilityWeapon.fireBetweenRounds = false;
        abilityWeapon.rate *= ability.Cooldown / attacks.Length;
        model.AddBehavior(abilityAttack);
    }

    public override void Apply2(TowerModel model)
    {
        if (!model.HasBehavior(out StartOfRoundRateBuffModel buffModel))
        {
            buffModel = Game.instance.model.GetTower(TowerType.SpikeFactory, 0, 0, 2)
                .GetBehavior<StartOfRoundRateBuffModel>().Duplicate();
            buffModel.modifier = .3f;
            model.AddBehavior(buffModel);
        }

        buffModel.modifier /= 10f;
    }

    protected override void RemoveAbility(TowerModel model)
    {
        if (model.appliedUpgrades.Contains(UpgradeType.CarpetOfSpikes)) return;

        base.RemoveAbility(model);
    }
}