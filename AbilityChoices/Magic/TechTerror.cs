using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2CppAssets.Scripts.Models.Effects;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Weapons.Behaviors;
using Il2CppAssets.Scripts.Unity;

namespace AbilityChoice.AbilityChoices.Magic;

public class TechTerror : TowerAbilityChoice
{
    private const int Factor = 10;
    public override string UpgradeId => UpgradeType.TechTerror;

    public override string Description1 => "Frequently annihilates nearby Bloons.";
    public override string Description2 => "Nanobot plasma seeks out and destroys Bloons with strong Crits";

    public override void Apply1(TowerModel model)
    {
        var ability = AbilityModel(model);
        var abilityAttack = ability.GetBehavior<ActivateAttackModel>().attacks[0].Duplicate();
        var abilityWeapon = abilityAttack.weapons[0];

        var effect = ability.GetBehavior<CreateEffectOnAbilityModel>().effectModel;
        var effectBehavior =
            new CreateEffectOnExhaustFractionModel("", CreatePrefabReference(""), effect, 0,
                Fullscreen.No, 1.0f, -1f, false);
        abilityWeapon.projectile.AddBehavior(effectBehavior);
        abilityWeapon.projectile.GetDamageModel().damage /= Factor;
        abilityWeapon.rate = ability.Cooldown / Factor;

        abilityAttack.range = abilityWeapon.projectile.radius - 10;
        abilityAttack.fireWithoutTarget = false;

        model.AddBehavior(abilityAttack);
    }

    public override void Apply2(TowerModel model)
    {
        model.GetDescendants<CritMultiplierModel>().ForEach(multiplierModel =>
        {
            multiplierModel.damage *= AbilityChoiceMod.MoreBalanced ? 1.5f : 5;
        });

        var retarget = Game.instance.model.GetTower(TowerType.BoomerangMonkey, 4)
            .GetDescendant<RetargetOnContactModel>().Duplicate();

        retarget.expireIfNoTargetFound = false;

        model.GetDescendants<ProjectileModel>()
            .ToList()
            .Where(projectileModel => projectileModel.HasBehavior<DamageModel>())
            .Do(projectileModel => projectileModel.AddBehavior(retarget.Duplicate()));
    }
}