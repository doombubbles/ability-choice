using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.Helpers;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Effects;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Weapons.Behaviors;
using Il2CppAssets.Scripts.Simulation.SMath;

namespace AbilityChoice.AbilityChoices.Paragon;

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

        proj.AddBehavior(new CreateSoundOnProjectileExpireModel("", finalStrike.nukeLaunchSoundModel,
            finalStrike.nukeLaunchSoundModel, finalStrike.nukeLaunchSoundModel, finalStrike.nukeLaunchSoundModel,
            finalStrike.nukeLaunchSoundModel, 0));

        model.AddBehavior(new AttackHelper("FinalStrike")
        {
            CanSeeCamo = true,
            Range = 2000,
            AttackThroughWalls = true,
            AddToSharedGrid = false,
            TargetProvider = new TargetStrongModel("", false, false),
            Weapon = new WeaponHelper
            {
                Projectile = proj.Duplicate(),
                Rate = ability.Cooldown / Factor / 3,
                Eject = new Vector3(finalStrike.throwOffsetX, finalStrike.throwOffsetY, finalStrike.throwOffsetZ),
                Emission = finalStrike.emissionModel,
                Behaviors =
                [
                    new EjectEffectModel("", finalStrike.launchEffectModel, 1, Fullscreen.No, false, false, false,
                        false),
                    new EjectEffectModel("", finalStrike.launchEjectEffectModel, 1, Fullscreen.No, false, true, false,
                        false)
                ]
            }
        });
    }


    public override void Apply2(TowerModel model)
    {
        var ability = AbilityModel(model);
        var finalStrike = ability.GetBehavior<FinalStrikeModel>();

        var createProj = finalStrike.GetDescendant<CreateProjectilesOnTrackOnExpireModel>().projectile;
        var emission = new SingleEmissionModel("", null);

        createProj.GetDescendant<AgeModel>().Lifespan /= Factor;

        model.GetAttackModel(1).weapons[0]!.projectile.AddBehavior(
            new CreateProjectilesOnTrackOnExpireModel("", createProj.Duplicate(), emission, false, 1, 60));

        createProj.GetDescendant<ClearHitBloonsModel>().interval *= Factor / 2;

        model.GetAttackModel().weapons[0]!.projectile.AddBehavior(new CreateProjectileOnExhaustFractionModel("",
            createProj.Duplicate(), emission, 1, -1, false, false, false));

    }
}