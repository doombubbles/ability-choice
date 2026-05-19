using System.Collections.Generic;
using BTD_Mod_Helper.Api.Helpers;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
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

        model.AddBehavior(AttackModel.Create(new()
        {
            name = "WeakeningRoar",
            range = buff.range,
            attackThroughWalls = true,
            CanSeeCamo = true,
            weapon = WeaponModel.Create(new()
            {
                name = "WeakeningRoar",
                animation = 3,
                rate = ability.Cooldown / Factor,
                behaviors =
                [
                    EjectEffectModel.Create(new() { effectModel = effect }),
                ],
                projectile = ProjectileModel.Create(new()
                {
                    id = "WeakeningRoar",
                    radius = buff.range,
                    pierce = 1000,
                    CanHitCamo = true,
                    behaviors =
                    [
                        AgeModel.Create(new() { lifespan = .05f }),
                        AddBonusDamagePerHitToBloonModel.Create(new()
                        {
                            mutationId = "WeakeningRoar",
                            lifespan = buff.lifespan / Factor,
                            perHitDamageAddition = buff.damageIncrease,
                            isUnique = true,
                        })
                    ]
                })
            })
        }));

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