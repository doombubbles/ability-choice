using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Weapons;
using Il2CppAssets.Scripts.Models.Towers.Weapons.Behaviors;
using Il2CppAssets.Scripts.Simulation.SMath;
namespace AbilityChoice.AbilityChoices.Military.MonkeySub;

public class FinalStrike : TowerAbilityChoice
{
    public override string UpgradeId => UpgradeType.MonkeysubParagon;

    public override string Description1 =>
        "Unfathomable range, power, and synergies with other Monkey Towers and Heroes put all Bloons under assault. Occasionally fires a devastating radioactive missile at the strongest Bloon.";

    public override string Description2 =>
        "Unfathomable range, power, and synergies with other Monkey Towers and Heroes put all Bloons under assault. Attacks leave behind radioactive fallout.";

    private const float Factor = 10;

    public override void Apply1(TowerModel model)
    {
        var ability = AbilityModel(model);
        var finalStrike = ability.GetBehavior<FinalStrikeModel>();

        var proj = finalStrike.projectileModel;
        proj.GetDescendants<DamageModel>().ForEach(damageModel => damageModel.damage /= Factor);
        proj.GetDescendants<SlowModel>().ForEach(slowModel => slowModel.Lifespan /= Factor);
        proj.GetDescendants<DamageModifierForTagModel>().ForEach(tagModel => tagModel.damageAddative /= Factor);
        proj.GetDescendants<AgeModel>().ForEach(ageModel =>
        {
            if (ageModel.Lifespan > 10)
            {
                ageModel.Lifespan /= Factor;
            }
        });

        proj.AddBehavior(CreateSoundOnProjectileExpireModel.Create(new()
        {
            sound = finalStrike.nukeLaunchSoundModel
        }));

        model.AddBehavior(AttackModel.Create(new()
        {
            name = "FinalStrike",
            CanSeeCamo = true,
            range = 2000,
            attackThroughWalls = true,
            addsToSharedGrid = false,
            targetProvider = TargetStrongModel.Create(),
            weapon = WeaponModel.Create(new()
            {
                projectile = proj.Duplicate(),
                rate = ability.Cooldown / Factor / 3,
                eject = new Vector3(finalStrike.throwOffsetX, finalStrike.throwOffsetY, finalStrike.throwOffsetZ),
                emission = finalStrike.emissionModel,
                behaviors =
                [
                    EjectEffectModel.Create(new()
                    {
                        effectModel = finalStrike.launchEffectModel,
                        lifespan = 1
                    }),
                    EjectEffectModel.Create(new()
                    {
                        effectModel = finalStrike.launchEjectEffectModel,
                        lifespan = 1,
                        useEjectPoint = true
                    })
                ]
            })
        }));
    }


    public override void Apply2(TowerModel model)
    {
        var ability = AbilityModel(model);
        var finalStrike = ability.GetBehavior<FinalStrikeModel>();

        var createProj = finalStrike.GetDescendant<CreateProjectilesOnTrackOnExpireModel>().projectile;
        var emission = SingleEmissionModel.Create();

        createProj.GetDescendant<AgeModel>().Lifespan /= Factor;

        model.GetAttackModel(1).weapons[0]!.projectile.AddBehavior(CreateProjectilesOnTrackOnExpireModel.Create(new()
        {
            projectile = createProj.Duplicate(),
            emission = emission,
            count = 1,
            range = 60
        }));

        createProj.GetDescendant<ClearHitBloonsModel>().interval *= Factor / 2;

        model.GetAttackModel().weapons[0]!.projectile.AddBehavior(CreateProjectileOnExhaustFractionModel.Create(new()
        {
            projectile = createProj.Duplicate(),
            emission = emission,
            fraction = 1,
            durationfraction = -1
        }));

    }
}