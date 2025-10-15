using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;

namespace AbilityChoice.AbilityChoices.Primary.BombShooter;

public class ISABMissile : TowerAbilityChoice
{
    public override string AbilityName => "Indomitable Surface-to-Air Ballistic Missile";

    public override string UpgradeId => UpgradeType.BombshooterParagon;

    public override string Description1 =>
        "Cluster explosions, anti-MOAB explosions, Bloon stunning explosions, knockback explosions. Automatic frequent ISAB Missiles! BOMB has it all!";

    public const int Factor = 3;

    public override void Apply1(TowerModel model)
    {
        var abilityModel = AbilityModel(model);

        var newAttack = abilityModel.GetDescendant<AttackModel>().Duplicate();

        var newAttackWeapon = newAttack.weapons[0]!;
        newAttackWeapon.projectile.GetDescendants<DamageModel>().ForEach(damage => damage.damage /= Factor);

        newAttackWeapon.Rate = abilityModel.Cooldown / Factor;

        model.AddBehavior(newAttack);
    }

}