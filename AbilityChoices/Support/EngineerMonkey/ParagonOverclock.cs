using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Weapons;
using Il2CppAssets.Scripts.Models.Towers.Weapons.Behaviors;

namespace AbilityChoice.AbilityChoices.Support.EngineerMonkey;

public class ParagonOverclock : Ultraboost
{
    public override string UpgradeId => UpgradeType.EngineermonkeyParagon;

    public override string Description1 =>
        "Monkey engineering developed to perfection. Can Overclock other Paragons (permanently, one at a time), and creates three Mega Sentries that can build their own sentries. Mega Sentries create small explosions periodically instead of on death.";

    public override string Description2 =>
        "Monkey engineering developed to perfection. Nearby Paragons are partially overclocked, and creates three Mega Sentries that can build their own sentries. Mega Sentries create small explosions periodically instead of on death.";

    private const float ParagonMultiplier = .3f;

    public override void Apply2(TowerModel model)
    {
        base.Apply2(model);

        var overclock = AbilityModel(model).GetBehavior<OverclockModel>();

        model.AddBehavior(RangeSupportModel.Create(new()
        {
            name = nameof(ParagonOverclock),
            isUnique = true,
            multiplier = 1 / CalcAvgBonus(ParagonMultiplier, 1 / overclock.rateModifier),
            additive = CalcAvgBonus(ParagonMultiplier, overclock.villageRangeModifier),
            mutatorId = overclock.mutatorId,
            buffLocsName = overclock.buffLocsName,
            buffIconName = overclock.buffIconName,
            showBuffIcon = true,
            onlyAffectParagon = true
        }));
    }

    private const int Factor = 10;

    protected override void ApplyBoth(TowerModel model)
    {
        base.ApplyBoth(model);

        var ability = model.GetBehaviors<AbilityModel>().First(a => a.displayName == "SentryParagonSpawner");

        foreach (var towerModel in model.GetDescendant<CreateSequencedTypedTowerCurrentIndexModel>().towers)
        {
            var projectile = towerModel.GetBehavior<CreateProjectileOnTowerDestroyModel>().projectileModel;
            projectile.GetBehavior<AgeModel>().Lifespan = .02f;

            var explosion = projectile.GetDescendant<CreateProjectileOnExpireModel>().projectile;
            explosion.GetDamageModel().damage /= Factor;

            towerModel.RemoveBehavior<CreateProjectileOnTowerDestroyModel>();
            towerModel.RemoveBehavior<CreateSoundOnSellModel>();

            towerModel.AddBehavior(AttackModel.Create(new()
            {
                name = "Explosion",
                CanSeeCamo = true,
                range = explosion.radius,
                weapon = WeaponModel.Create(new()
                {
                    projectile = projectile,
                    rate = ability.Cooldown * 3 / Factor
                }),
                targetProvider = TargetCloseModel.Create(new() { isOnSubTower = true })
            }));

        }
    }
}