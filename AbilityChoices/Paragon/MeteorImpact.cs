using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.Helpers;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Weapons.Behaviors;
using Il2CppAssets.Scripts.Simulation.SMath;

namespace AbilityChoice.AbilityChoices.Paragon;

public class MeteorImpact : TowerAbilityChoice
{
    public override string UpgradeId => UpgradeType.TackshooterParagon;

    public override string Description1 =>
        "Spinning blades of Bloon immolation! Occasionally launches a Meteor at the strongest Bloon, sometimes shoots jets of flame with increased projectile lifespan.";

    public override string Description2 =>
        "Spinning blades of Bloon immolation! Constantly shoots homing fireballs, sometimes shoots jets of flame with increased projectile lifespan.";

    private const int Factor = 5;

    public override void Apply1(TowerModel model)
    {
        var ability = AbilityModel(model);
        var attack = ability.GetDescendant<AttackModel>();
        attack.GetDescendants<DamageModel>().ForEach(damageModel => damageModel.damage /= Factor);
        ability.Cooldown /= Factor;

        attack.GetDescendant<CreateEffectOnExpireModel>().effectModel.scale /= 2f;
        attack.GetDescendant<CreateProjectileOnExpireModel>().projectile.radius /= 2f;

        attack.weapons[0]!.Rate = ability.Cooldown;
        model.AddBehavior(attack);
    }

    public override void Apply2(TowerModel model)
    {
        var ability = AbilityModel(model);
        var attack = ability.GetDescendant<AttackModel>();
        attack.GetDescendants<DamageModel>().ForEach(damageModel => damageModel.damage /= Factor);

        var createProj = attack.GetDescendant<CreateProjectileOnExhaustFractionModel>();

        var proj2 = createProj.projectile.Duplicate();

        createProj.projectile.RemoveBehavior<ClearHitBloonsModel>();

        proj2.pierce *= 100;
        proj2.GetDescendant<DamageModel>().damage /= 2;

        model.GetAttackModel().AddWeapon(new WeaponHelper("Fireballs")
        {
            Animation = -1,
            Eject = new Vector3(0, 0, 23),
            Rate = ability.Cooldown / (Factor * Factor),
            Emission = createProj.emission.Duplicate(),
            Projectile = createProj.projectile,
            Behaviors =
            [
                new AlternateProjectileModel("", proj2, createProj.emission.Duplicate(), Factor)
            ]
        });
    }

    protected override void ApplyBoth(TowerModel model)
    {
        var ability = model.GetAbilities().First(a => a.displayName == "Eruption");
        ability.RemoveBehavior<CreateSoundOnAbilityModel>();
        ability.Cooldown /= Factor;
        ability.GetDescendant<ActivateAttackModel>().Lifespan /= Factor;

        TechBotify(model, ability);
    }
}