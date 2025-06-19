using System.Collections.Generic;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.GenericBehaviors;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Filters;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Weapons;

namespace AbilityChoice.AbilityChoices.Hero.Sauda;

public class LeapingSwordAttack : HeroAbilityChoice
{
    public override string HeroId => TowerType.Sauda;

    public override Dictionary<int, string> Descriptions1 => new()
    {
        { 3, "Sauda can throw blades on her targeted Bloons that stick in the track and can pop Lead Bloons." }
    };

    public override Dictionary<int, string> Descriptions2 => new()
    {
        { 3, "Saudas swords leap out to nearby enemies beyond her range." }
    };

    private const int Factor = 5;

    public override void Apply1(TowerModel model)
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
        impact.GetBehaviors<DamageModifierForTagModel>().ForEach(modifier => modifier.damageAddative /= Factor);
        impact.GetDescendants<SaudaAfflictionDamageModifierModel>().ForEach(saudaDamage =>
        {
            saudaDamage.lv7MoabBonus /= Factor;
            saudaDamage.lv11MoabBonus /= Factor;
            saudaDamage.lv19MoabBonus /= Factor;
            saudaDamage.lv7NonMoabBonus /= Factor;
            saudaDamage.lv11NonMoabBonus /= Factor;
            saudaDamage.lv19NonMoabBonus /= Factor;
        });

        dot.GetBehavior<AgeModel>().Lifespan /= Factor;

        impact.AddBehavior(new CreateProjectileOnExpireModel("", dot, new SingleEmissionModel("", null), true, false));

        var melee = model.GetAttackModel();

        var attack = new AttackModel(Name, new[]
        {
            new WeaponModel("", -1, ability.Cooldown / Factor, impact, emission: new SingleEmissionModel("", new[]
            {
                new EmissionRotationZeroModel("")
            }), ejectZ: 9999)
        }, 9999, melee.behaviors, null, 0, 0, 0, true, false, 0, true, 0);


        model.AddBehavior(attack);
    }

    public override void Apply2(TowerModel model)
    {
        var ability = AbilityModel(model);
        var leapingSword = ability.GetBehavior<LeapingSwordModel>();
        var proj = leapingSword.dotProjectileModel;

        var weaponModel = model.GetDescendant<WeaponModel>();

        proj.RemoveBehavior<AgeModel>();
        proj.RemoveBehavior<RefreshPierceModel>();
        proj.RemoveBehavior<ClearHitBloonsModel>();
        proj.pierce = weaponModel.projectile.pierce * 2;
        proj.ignorePierceExhaustion = false;

        proj.AddBehavior(new TravelStraitModel("", 150f, .5f));
        proj.AddBehavior(new TrackTargetModel("", 999, true, false, 180, false, 1000, false, true));

        proj.GetDamageModel().damage /= Factor;
        if (proj.HasBehavior(out SaudaAfflictionDamageModifierModel saudaDamage))
        {
            saudaDamage.lv7MoabBonus /= Factor;
            saudaDamage.lv11MoabBonus /= Factor;
            saudaDamage.lv19MoabBonus /= Factor;
            saudaDamage.lv7NonMoabBonus /= Factor;
            saudaDamage.lv11NonMoabBonus /= Factor;
            saudaDamage.lv19NonMoabBonus /= Factor;
        }

        model.AddBehavior(new AttackModel(Name, new[]
        {
            new WeaponModel("", -1, weaponModel.Rate * 2, proj, emission: new SingleEmissionModel("", null),
                ejectY: weaponModel.ejectY)
        }, model.range, new[]
        {
            new AttackFilterModel("", new[]
            {
                new FilterInvisibleModel("", false, false)
            })
        }, null, 0, 0, 0, false, false, 0, false, 0));
    }
}