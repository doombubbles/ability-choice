﻿using Assets.Scripts.Models.Towers;
using Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Assets.Scripts.Models.Towers.Projectiles;
using Assets.Scripts.Models.Towers.Projectiles.Behaviors;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;

namespace AbilityChoice.AbilityChoices.Primary;

public class GlueStrike : AbilityChoice
{
    public override string UpgradeId => UpgradeType.GlueStrike;

    public override string Description1 => "Periodically glues all bloons on screen for a short duration, making them take increased damage.";

    public override string Description2 =>
        "Glue weakens Bloons, making them take increased damage and be vulnerable to Sharp sources.";

    protected virtual float Factor => 5;
    
    public override void Apply1(TowerModel model)
    {
        var ability = AbilityModel(model);

        var activateAttackModel = ability.GetBehavior<ActivateAttackModel>();
        var abilityAttack = activateAttackModel.attacks[0].Duplicate();
        var abilityWeapon = abilityAttack.weapons[0];
        abilityWeapon.Rate = ability.Cooldown / Factor;
        var proj = abilityWeapon.projectile;
        
        proj.GetBehavior<SlowModel>().Lifespan /= Factor;
        proj.GetBehavior<RemoveDamageTypeModifierModel>().lifespan /= Factor;
        proj.GetBehavior<AddBonusDamagePerHitToBloonModel>().lifespan /= Factor;
        
        model.AddBehavior(abilityAttack);
    }

    public override void Apply2(TowerModel model)
    {
        var abilityModel = AbilityModel(model);
        var damageBoost = abilityModel.GetDescendant<AddBonusDamagePerHitToBloonModel>();
        var sharpWeak = abilityModel.GetDescendant<RemoveDamageTypeModifierModel>();
        var abilitySlow = abilityModel.GetDescendant<SlowModel>();

        foreach (var projectileModel in model.GetWeapon().GetDescendants<ProjectileModel>().ToList())
        {
            var slowModel = projectileModel.GetBehavior<SlowModel>();
            slowModel.lifespan = abilitySlow.lifespan;
            slowModel.layers = abilitySlow.layers;
            slowModel.multiplier = abilitySlow.multiplier;

            if (damageBoost != null)
            {
                projectileModel.AddBehavior(damageBoost.Duplicate());
            }

            if (sharpWeak != null)
            {
                projectileModel.AddBehavior(sharpWeak.Duplicate());
            }
        }
    }
}