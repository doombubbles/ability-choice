using System.Collections.Generic;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Weapons;

namespace AbilityChoice.AbilityChoices.Hero.ObynGreenfoot;

public class Brambles : HeroAbilityChoice
{
    public override string HeroId => TowerType.ObynGreenfoot;

    public override Dictionary<int, string> Descriptions1 => new()
    {
        { 3, "Creates spiked bushes on the track that can pop 50 bloons." },
    };

    public override Dictionary<int, string> Descriptions2 => new()
    {
        { 3, $"Frequently creates small spiked bushes on the track that can pop {50 / Factor} Bloons each." },
        { 7, $"Brambles can pop {100 / Factor} Bloons." },
        { 16, $"Brambles pop {500 / Factor} bloons each and can damage all Bloon types." },
    };

    private const int Factor = 5;

    public override void Apply1(TowerModel model)
    {
        var ability = AbilityModel(model);
        var attack = ability.GetDescendant<AttackModel>().Duplicate();
        var weapon = attack.weapons[0];

        weapon.Rate = ability.Cooldown;

        model.AddBehavior(attack);
    }

    public override void Apply2(TowerModel model)
    {
        var ability = AbilityModel(model);
        var attack = ability.GetDescendant<AttackModel>().Duplicate();
        var weapon = attack.weapons[0];
        var projectile = weapon.projectile;

        weapon.Rate = ability.Cooldown / Factor;
        projectile.pierce /= Factor;
        projectile.scale = .7f;
        projectile.RemoveBehavior<CreateEffectOnExhaustedModel>();

        model.AddBehavior(attack);
    }

    protected override void ApplyBoth(TowerModel model)
    {
        AbilityModel(model).GetDescendant<WeaponModel>().fireBetweenRounds = false;
    }
}