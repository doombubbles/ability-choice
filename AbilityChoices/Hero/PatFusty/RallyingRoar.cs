using System.Collections.Generic;
using BTD_Mod_Helper.Api.Helpers;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Weapons;
using Il2CppAssets.Scripts.Models.Towers.Weapons.Behaviors;

namespace AbilityChoice.AbilityChoices.Hero.PatFusty;

public class RallyingRoar : HeroAbilityChoice
{
    private const int Factor = 3;
    public override string HeroId => TowerType.PatFusty;

    public override Dictionary<int, string> Descriptions1 => new()
    {
        { 3, "Occasionally lets out a roar, rallying nearby Monkeys to pop +1 layer for a short time." }
    };

    public override Dictionary<int, string> Descriptions2 => new()
    {
        { 3, "Occasionally lets out a roar, weakening nearby Bloons such that they take +1 damage for a short time." },
        { 14, "Weakening Roar increased radius and duration and increased damage Bloons take." }
    };

    public override void Apply1(TowerModel model)
    {
        var ability = AbilityModel(model);

        ability.RemoveBehavior<CreateSoundOnAbilityModel>();
        ability.GetBehavior<CreateEffectOnAbilityModel>().effectModel.scale = .5f;
        ability.Cooldown /= Factor;

        var buff = ability.GetBehavior<ActivateTowerDamageSupportZoneModel>();

        buff.lifespan /= Factor;
        buff.lifespanFrames /= Factor;
    }

    public override void Apply2(TowerModel model)
    {
        var ability = AbilityModel(model);
        var effect = ability.GetBehavior<CreateEffectOnAbilityModel>().effectModel;
        var buff = ability.GetBehavior<ActivateTowerDamageSupportZoneModel>();

        model.AddBehavior(new AttackHelper("WeakeningRoar")
        {
            Range = buff.range,
            AttackThroughWalls = true,
            CanSeeCamo = true,
            Weapon = new WeaponHelper("WeakeningRoar")
            {
                Animation = 3,
                Rate = ability.Cooldown / Factor,
                Behaviors = new WeaponBehaviorModel[]
                {
                    new EjectEffectModel("", effect.assetId, effect, effect.lifespan, effect.fullscreen, false, false,
                        false, false)
                },
                Projectile = new ProjectileHelper("WeakeningRoar")
                {
                    Radius = buff.range,
                    Pierce = 1000,
                    CanHitCamo = true,
                    Behaviors = new Model[]
                    {
                        new AgeModel("", .05f, 0, false, null),
                        new AddBonusDamagePerHitToBloonModel("", "WeakeningRoar", buff.lifespan / Factor,
                            buff.damageIncrease, 99999, true, false, false, "")
                    }
                }
            }
        });

        /*model.AddBehavior(new AttackModel("Roar", new[]
        {
            new WeaponModel("", 3, ability.Cooldown / Factor, new ProjectileModel(new PrefabReference { guidRef = "" },
                "Roar", buff.range, 0, 99999, 0, new Model[]
                {
                    new ProjectileFilterModel("", new[]
                    {
                        new FilterInvisibleModel("", false, false)
                    }),
                    new AgeModel("", .05f, 0, false, null),
                    new AddBonusDamagePerHitToBloonModel("", "WeakeningRoar", buff.lifespan / Factor,
                        buff.damageIncrease, 99999, true, false, false),
                    new DisplayModel("", new PrefabReference { guidRef = "" }, 0, DisplayCategory.Projectile)
                }, filters: new[]
                {
                    new FilterInvisibleModel("", false, false)
                }, collisionPasses: new[]
                {
                    0
                }
            ), behaviors: new[]
            {
                new EjectEffectModel("", effect.assetId, effect, effect.lifespan, effect.fullscreen, false, false,
                    false, false)
            }, emission: new SingleEmissionModel("", null))
        }, buff.range, new[]
        {
            new AttackFilterModel("", new[]
            {
                new FilterInvisibleModel("", false, false)
            })
        }, null, 0, 0, 0, true, false, 0, false, 0));
        */

        model.RemoveBehavior<PatBuffIndicatorModel>();
    }

    protected override void RemoveAbility(TowerModel model)
    {
        if (Mode2)
        {
            base.RemoveAbility(model);
        }
        else
        {
            TechBotify(model);
        }
    }
}