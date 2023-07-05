using System.Collections.Generic;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using Il2CppAssets.Scripts.Models.Towers.Weapons.Behaviors;

namespace AbilityChoice.AbilityChoices.Hero;

public class WallOfTrees : HeroAbilityChoice
{
    public override string HeroId => TowerType.ObynGreenfoot;

    public override string AbilityName => "Wall of Trees";

    public override Dictionary<int, string> Descriptions1 => new()
    {
        {
            10,
            "Summons walls of trees across the track that destroy all Bloons that enter. When full, the trees burst into money."
        }
    };

    protected override void Apply1(TowerModel model)
    {
        var ability = AbilityModel(model);
        var attack = ability.GetDescendant<AttackModel>().Duplicate();
        var weapon = attack.weapons[0];
        var limitProjectile = weapon.GetBehavior<LimitProjectileModel>();

        weapon.Rate = ability.Cooldown;
        limitProjectile.limitByDestroyedPriorProjectile = false;
        // limitProjectile.delayInFrames = 30;


        model.AddBehavior(attack);
    }

    protected override void Apply2(TowerModel model)
    {
        // TODO wall of trees 2
    }
}