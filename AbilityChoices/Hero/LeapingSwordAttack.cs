using System;
using System.Collections.Generic;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.GenericBehaviors;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Weapons;
using Il2CppAssets.Scripts.Unity;

namespace AbilityChoice.AbilityChoices.Hero;

public class LeapingSwordAttack : HeroAbilityChoice
{
    public override string HeroId => TowerType.Sauda;

    public override Dictionary<int, string> Descriptions1 => new()
    {
        { 3, "Sauda can throw blades on her targeted Bloons that stick in the track and can pop Lead Bloons." }
    };

    private const int Factor = 5;

    protected override void Apply1(TowerModel model)
    {
        var ability = AbilityModel(model);
        var leapingSword = ability.GetBehavior<LeapingSwordModel>();

        var impact = leapingSword.impactProjectileModel;
        var dot = leapingSword.dotProjectileModel;

        var time = .2f;

        impact.display = impact.GetBehavior<DisplayModel>().display = dot.display;
        impact.AddBehavior(new ArriveAtTargetModel("", time, new[] { .95f, 1f }, true, false, 0, true, true, false, 0,
            true));
        impact.GetBehavior<AgeModel>().Lifespan += time;
        impact.GetBehavior<DamageModel>().damage /= Factor;
        impact.GetBehaviors<DamageModifierForTagModel>().ForEach(modifier => { modifier.damageAddative /= Factor; });

        dot.GetBehavior<AgeModel>().Lifespan /= Factor;

        impact.AddBehavior(new CreateProjectileOnExpireModel("", dot, new SingleEmissionModel("", null), true));

        var melee = model.GetAttackModel();

        var attack = new AttackModel(Name, new[]
        {
            new WeaponModel("", -1, ability.Cooldown / Factor, impact, emission: new SingleEmissionModel("", new[]
            {
                new EmissionRotationZeroModel("")
            }), ejectZ: 9999)
        }, 9999, melee.behaviors, null, 0, 0, 0, true, false, 0, false, 0);


        model.AddBehavior(attack);
    }

    protected override void Apply2(TowerModel model)
    {
        // TODO leaping sword 2
    }
}