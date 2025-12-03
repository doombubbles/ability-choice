using System.Collections.Generic;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.Helpers;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using Il2CppAssets.Scripts.Models.Effects;
using Il2CppAssets.Scripts.Models.GenericBehaviors;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions;
using Il2CppAssets.Scripts.Models.Towers.Filters;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Weapons;
using Il2CppAssets.Scripts.Models.Towers.Weapons.Behaviors;
using Il2CppAssets.Scripts.Unity;

namespace AbilityChoice.AbilityChoices.Hero.CaptainChurchill;

public class MOABBarrage : HeroAbilityChoice
{
    public override string HeroId => TowerType.CaptainChurchill;

    public override Dictionary<int, string> Descriptions1 => new()
    {
        {
            10,
            "Frequently launches a wave of shells at up to 10 MOAB-class Bloons for massive damage."
        },
        {
            20,
            "MOAB Barrage shells and Main Gun do massively increased damage per hit, and barrages happen more often."
        }
    };

    public override Dictionary<int, string> Descriptions2 => new()
    {
        { 10, "Constantly barrages the strongest MOAB-class Bloon on screen with shells that deal massive damage." },
        { 20, "MOAB Barrage shells and Main Gun do massively increased damage per hit, and barrages fire even faster." }
    };

    public override void Apply1(TowerModel model)
    {
        var ability = AbilityModel(model);

        var moabBarrage = ability.GetBehavior<MoabBarrageModel>();
        var dot = moabBarrage.GetDescendant<MoabBarrageBloonBehaviorModel>();

        var newDot = dot.Duplicate();
        newDot.numOfMissiles = 1;

        var attack = Game.instance.model.GetHeroWithNameAndLevel(TowerType.Gwendolin, 10).GetAbility(1)
            .GetDescendant<ActivateAttackModel>().attacks[1].Duplicate();
        attack.fireWithoutTarget = false;
        attack.AddBehavior(new AttackFilterModel("", new[] { new FilterWithTagModel("", BloonTag.Moabs, false) }));

        var weapon = attack.GetChild<WeaponModel>();
        weapon.Rate = ability.Cooldown / dot.numOfMissiles;
        weapon.fireWithoutTarget = false;

        var effect = ability.GetBehavior<CreateEffectOnAbilityModel>().effectModel;
        effect.lifespan = .2f;
        weapon.AddBehavior(new EjectEffectModel("", effect, .2f, Fullscreen.No, false, false, false,
            false));

        var projectile = weapon.projectile;
        projectile.pierce = moabBarrage.targets;
        projectile.filters = projectile.GetBehavior<ProjectileFilterModel>().filters =
            projectile.filters.Take(1).ToArray();
        projectile.RemoveBehavior<DamageModel>();

        var addBehavior = projectile.GetBehavior<AddBehaviorToBloonModel>();
        addBehavior.mutationId = moabBarrage.mutatorId;
        addBehavior.overlayType = "";
        addBehavior.behaviors = new[] { newDot };


        model.AddBehavior(attack);
    }

    // Red is better against 5-10 Moabs, Blue is better for < 5
    private const float Factor = .5f;

    public override void Apply2(TowerModel model)
    {
        var ability = AbilityModel(model);

        var moabBarrage = ability.GetBehavior<MoabBarrageModel>();
        var dot = moabBarrage.GetDescendant<MoabBarrageBloonBehaviorModel>();
        var effect = ability.GetBehavior<CreateEffectOnAbilityModel>().effectModel;
        effect.lifespan = .2f;

        var perSecond = Factor * dot.numOfMissiles * moabBarrage.targets / ability.Cooldown;

        dot.numOfMissiles = 1;
        dot.Interval = .01f;
        dot.InitialDelay = 0;
        dot.randomDelayMax = dot.randomDelayMaxFrames = 0;

        model.AddBehavior(new AttackHelper(Name)
        {
            Range = 9999,
            AttackThroughWalls = true,
            Filters = [FilterMoabModel.Create()],
            Behaviors = [TargetStrongModel.Create()],
            CanSeeCamo = true,
            Weapon = new WeaponHelper
            {
                Animation = 3,
                Rate = 1 / perSecond,
                Emission = SingleEmissionModel.Create(),
                Behaviors =
                [
                    EjectEffectModel.Create(new() { effectModel = effect, lifespan = .2f })
                ],
                Projectile = new ProjectileHelper(Name)
                {
                    MaxPierce = 1,
                    Behaviors =
                    [
                        AddBehaviorToBloonModel.Create(new()
                        {
                            mutationId = moabBarrage.mutatorId, lifespan = .05f, layers = 1, behaviors = [dot],
                            isUnique = true
                        }),
                        InstantModel.Create(),
                        AgeModel.Create(new() { lifespan = .05f }),
                        DisplayModel.Create()
                    ],
                    Filters = [FilterAllExceptTargetModel.Create()]
                }
            }
        });
    }
}