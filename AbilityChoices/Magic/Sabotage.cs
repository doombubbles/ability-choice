using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using Il2Cpp;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Weapons.Behaviors;

namespace AbilityChoice.AbilityChoices.Magic;

public class Sabotage : TowerAbilityChoice
{
    public override string UpgradeId => UpgradeType.BloonSabotage;

    public override string Description1 => "All Bloons move at partially reduced speed.";
    public override string Description2 => "Ninja’s attacks have more range and slow Bloons to half speed.";

    public override void Apply1(TowerModel model)
    {
        var abilityModel = AbilityModel(model);
        var slow = abilityModel.GetDescendant<SlowMinusAbilityDurationModel>();

        var mult = CalcAvgBonus(slow.Lifespan / abilityModel.Cooldown, slow.multiplier);

        var slowAttack = abilityModel.GetDescendant<AttackModel>().Duplicate();
        var slowWeapon = slowAttack.weapons[0];
        slowWeapon.fireBetweenRounds = false;
        slowWeapon.AddBehavior(new WeaponRateMinModel("", 2));
        var slowingProjectile = slowAttack.weapons[0].projectile;
        slowingProjectile.RemoveBehavior<SlowMinusAbilityDurationModel>();
        slowingProjectile.GetBehavior<AgeModel>().Lifespan = 2;

        var slowBehavior = new SlowModel("Sabotage", mult, 2, slow.mutationId, 999, "", true,
            false, null, false, false, false, 0);
        slowingProjectile.AddBehavior(slowBehavior);

        model.AddBehavior(slowAttack);
    }

    public override void Apply2(TowerModel model)
    {
        model.IncreaseRange(10);
        var ability = AbilityModel(model);
        var abilityAttack = ability.GetBehavior<ActivateAttackModel>().attacks[0].Duplicate();
        var abilityWeapon = abilityAttack.weapons[0];
        var slowMutator = abilityWeapon.projectile.GetBehavior<SlowMinusAbilityDurationModel>().Mutator;

        var dontSlowBadBehavior = abilityWeapon.projectile.GetBehavior<SlowModifierForTagModel>();

        var slowBehavior = new SlowModel("Sabotage", 0.5f, 2f, slowMutator.mutationId, 999, "", true, false, null,
            false, false, false, 0) {mutator = slowMutator};


        foreach (var weaponModel in model.GetWeapons())
        {
            if (weaponModel.projectile.GetDamageModel().IsType(out DamageModel damageModel))
            {
                weaponModel.projectile.AddBehavior(slowBehavior);
                weaponModel.projectile.AddBehavior(dontSlowBadBehavior);
                weaponModel.projectile.pierce += 5;

                damageModel.immuneBloonProperties = BloonProperties.None;
            }
        }
    }
}