using System.Collections.Generic;
using BTD_Mod_Helper.Extensions;
using Il2Cpp;
using Il2CppAssets.Scripts.Models.GenericBehaviors;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Weapons;
using Il2CppAssets.Scripts.Models.Towers.Weapons.Behaviors;

namespace AbilityChoice.AbilityChoices.Hero;

public class ArmorPiercingShells : HeroAbilityChoice
{
    public override string HeroId => TowerType.CaptainChurchill;

    public override Dictionary<int, string> Descriptions1 => new()
    {
        {
            3,
            "Every third shot becomes an Armor Piercing Shell that can pop Black Bloons and does extra damage to Ceramic Bloons."
        },
        {
            13,
            "Armor Piercing Shells happen every other shot, pop 2 layers of Bloon and do extra damage to Ceramic and higher."
        },
        {
            17,
            "Armor Piercing Shells do even more damage."
        }
    };


    public override void Apply1(TowerModel model)
    {
        var ability = AbilityModel(model);

        var interval = model.tier >= 13 ? 2 : 3;

        var weapon = model.GetAttackModel().GetChild<WeaponModel>();

        var projectile = weapon.projectile.Duplicate();

        projectile.GetDescendants<DamageModel>()
            .ForEach(damageModel => damageModel.immuneBloonProperties = BloonProperties.None);

        ability.GetBehaviors<MutateProjectileOnAbilityModel>().ForEach(mutate =>
        {
            var target = projectile.GetDescendants<ProjectileModel>().ToList()
                .First(p => p.id == mutate.projectileModel.id);

            target.GetDamageModel().damage += mutate.damageIncrease;

            target.AddBehavior(mutate.projectileBehaviorModel);

            target.hasDamageModifiers = true;
        });

        projectile.display = projectile.GetBehavior<DisplayModel>().display =
            ability.GetBehavior<ChangeProjectileDisplayModel>().displayPath.assetPath;

        projectile.GetDescendant<CreateProjectileOnExhaustPierceModel>().count += ability
            .GetBehavior<MutateCreateProjectileOnExhaustPierceOnAbilityModel>().countIncrease;

        var alt = new AlternateProjectileModel("", projectile, null, interval);

        weapon.AddBehavior(alt);
    }
}