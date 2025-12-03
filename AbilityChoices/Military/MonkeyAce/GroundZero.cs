using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Effects;
using Il2CppAssets.Scripts.Models.GenericBehaviors;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Weapons.Behaviors;
namespace AbilityChoice.AbilityChoices.Military.MonkeyAce;

public class GroundZero : TowerAbilityChoice
{
    protected const int Factor = 7;
    public override string UpgradeId => UpgradeType.GroundZero;

    public override string Description1 =>
        "Bomb damage increased significantly. Occasionally drops mini Ground Zeros.";

    public override string Description2 =>
        "Bomb damage increased significantly. Shoots a continuous stream of bombs.";

    public override void Apply1(TowerModel model)
    {
        var pineapple = model.GetAttackModel("Pineapple");
        var ability = AbilityModel(model);
        var abilityAttack = ability.GetDescendant<AttackModel>().Duplicate();
        var abilityWeapon = abilityAttack.weapons[0];
        var effectModel = ability.GetBehavior<CreateEffectOnAbilityModel>().effectModel.Duplicate();
        effectModel.scale = .5f;
        effectModel.useCenterPosition = false;
        var sound = ability.GetBehavior<CreateSoundOnAbilityModel>().sound;

        var newAttack = pineapple.Duplicate();
        newAttack.name = "AttackModel_GroundZero";

        var weapon = newAttack.weapons[0];
        weapon.emission = abilityWeapon.emission;

        weapon.Rate = ability.Cooldown / Factor;
        abilityWeapon.projectile.GetDescendant<DamageModel>().damage /= Factor;
        abilityWeapon.projectile.radius = 100;
        if (abilityWeapon.projectile.HasBehavior(out SlowModel slowModel))
        {
            slowModel.lifespan /= Factor;
        }

        weapon.ejectY = 0;
        weapon.AddBehavior(new EjectEffectModel("", effectModel, -1,
            Fullscreen.No, false, true, false, false));

        var projectile = weapon.projectile;

        projectile.GetBehavior<CreateProjectileOnExhaustFractionModel>().projectile = abilityWeapon.projectile;
        projectile.RemoveBehavior<CreateEffectOnExhaustFractionModel>();
        projectile.GetBehavior<DisplayModel>().display = CreatePrefabReference("");

        weapon.AddBehavior(CreateSoundOnProjectileCreatedModel.Create(new()
        {
            sound1 = sound, sound2 = sound, sound3 = sound, sound4 = sound, sound5 = sound
        }));

        model.AddBehavior(newAttack);
    }

    public override void Apply2(TowerModel model)
    {
        var pineapple = model.GetAttackModel("Pineapple");
        var emissionOverTime = pineapple.GetDescendant<EmissionOverTimeModel>();
        var pineappleWeapon = pineapple.weapons[0];
        pineappleWeapon.Rate = emissionOverTime.count * emissionOverTime.timeBetween;
        pineappleWeapon.RemoveBehavior<CheckAirUnitOverTrackModel>();
    }
}