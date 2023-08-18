using System.Collections.Generic;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Weapons;
using Il2CppAssets.Scripts.Models.Towers.Weapons.Behaviors;

namespace AbilityChoice.AbilityChoices.Hero;

public class PsychicBlast : HeroAbilityChoice
{
    public override string HeroId => TowerType.Psi;

    public override Dictionary<int, string> Descriptions1 => new()
    {
        { 3, "Psi unleashes waves of power that stuns nearby Bloons for a short time." },
        { 7, "Psychic Blast pulses more often. Psi can now destroy Ceramic Bloons." },
        { 12, "Psychic Blast pulses even more often." }
    };

    public override Dictionary<int, string> Descriptions2 => new()
    {
        { 3, "Emits small waves of power around Psi's targets, briefly stunning nearby Bloons." },
        { 7, "Psychic Blasts hit more Bloons. Psi can now destroy Ceramic Bloons." },
        { 12, "Psychic Blasts hit even more Bloons." }
    };

    private const int Factor = 4;

    public override void Apply1(TowerModel model)
    {
        var ability = AbilityModel(model);

        var activate = ability.GetBehavior<ActivateAttackModel>();

        var amount = (activate.Lifespan + 1.95f) / 2f;

        var attack = activate.GetChild<AttackModel>();
        attack.fireWithoutTarget = false;


        var weapon = attack.GetChild<WeaponModel>();
        weapon.Rate = ability.Cooldown / Factor / amount;
        weapon.RemoveBehavior<CreateSoundOnProjectileCreatedModel>();

        var projectile = weapon.projectile;

        projectile.GetBehavior<SlowModel>().Lifespan /= Factor;
        projectile.GetBehavior<SlowModifierForTagModel>().lifespanOverride /= Factor;

        attack.range = weapon.projectile.radius;

        model.AddBehavior(attack);
    }

    public override void Apply2(TowerModel model)
    {
        var ability = AbilityModel(model);

        var activate = ability.GetBehavior<ActivateAttackModel>();

        var amount = (activate.Lifespan + 1.95f) / 2f;
        var scale = .2f + amount * .1f;

        var effect = ability.GetDescendant<EjectEffectModel>().effectModel;

        effect.scale *= scale;
        effect.lifespan /= Factor;

        var projectile = ability.GetDescendant<ProjectileModel>();
        projectile.pierce *= scale;
        projectile.radius /= Factor;
        projectile.GetBehavior<SlowModel>().Lifespan /= Factor;
        projectile.GetBehavior<SlowModifierForTagModel>().lifespanOverride /= Factor;

        model.GetDescendants<ProjectileModel>().ForEach(proj =>
        {
            if (proj.id != "BaseProjectile") return;

            proj.AddBehavior(new CreateProjectileOnContactModel(Name, projectile.Duplicate(),
                new SingleEmissionModel("", null), false, false, false));
            proj.AddBehavior(new CreateEffectOnContactModel(Name, effect.Duplicate()));
        });
    }
}