using System.Collections.Generic;
using BTD_Mod_Helper.Extensions;
using Il2Cpp;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Weapons;
using Il2CppAssets.Scripts.Models.Towers.Weapons.Behaviors;

namespace AbilityChoice.AbilityChoices.Hero.Silas;

public class Frostbite : HeroAbilityChoice
{
    public override string HeroId => TowerType.Silas;

    public override Dictionary<int, string> Descriptions1 => new()
    {
        {
            3,
            "Periodically Silas can hit all Bloons types for a short duration, doing extra damage to Frozen Bloons based on remaining freeze time."
        },
        {
            9,
            "Frozen Bloons take more damage from Frostbite attacks. Ice Fragment damage & pierce increased."
        },
        {
            15,
            "Frozen Bloons take more damage from all special attacks, and special attacks are more frequent."
        },
        {
            20,
            "Frozen Bloons take even more damage from Frostbite attacks. Silas creates Ice Walls more often. Monkeys that freeze Bloons freeze them yet longer again."
        }
    };

    public override Dictionary<int, string> Descriptions2 => new()
    {
        {
            3,
            "Every 4th attack can hit all Bloons types and does extra damage to Frozen Bloons based on remaining freeze time."
        },
        {
            9,
            "Frozen Bloons take more damage from Frostbite attacks. Ice Fragment damage & pierce increased."
        },
        {
            15,
            "Frozen Bloons take more damage from all special attacks, and special attacks are more frequent."
        },
        {
            20,
            "Frozen Bloons take even more damage from Frostbite attacks. Silas creates Ice Walls more often. Monkeys that freeze Bloons freeze them yet longer again."
        }
    };

    private const int Factor = 3;

    public override void Apply1(TowerModel model)
    {
        var ability = AbilityModel(model);
        ability.Cooldown /= Factor;

        ability.RemoveBehavior<CreateSoundOnAbilityModel>();

        ability.GetBehavior<ChangeDamageTypeModel>().lifespanFrames /= Factor;
        ability.GetBehavior<MutateProjectileOnAbilityModel>().lifespanFrames /= Factor;
    }

    public override void Apply2(TowerModel model)
    {
        var ability = AbilityModel(model);

        var interval = model.tier >= 15 ? 3 : 4;

        var weapon = model.GetAttackModel().GetChild<WeaponModel>();

        var projectile = weapon.projectile.Duplicate();

        projectile.GetDescendants<DamageModel>()
            .ForEach(damageModel => damageModel.immuneBloonProperties = BloonProperties.None);

        ability.GetBehaviors<MutateProjectileOnAbilityModel>().ForEach(mutate =>
        {
            projectile.GetDamageModel().damage += mutate.damageIncrease;

            projectile.AddBehavior(mutate.projectileBehaviorModel);

            projectile.hasDamageModifiers = true;
        });

        var alt = new AlternateProjectileModel("", projectile, null, interval);

        weapon.AddBehavior(alt);
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