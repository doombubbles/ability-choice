using System;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Effects;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Weapons;

namespace AbilityChoice.AbilityChoices.Support;

public class TRexStomp : AbilityChoice
{
    public override string AbilityName => "T Rex STOMP";
    public override string UpgradeId => "Tyrannosaurus Rex"; // TODO UpgradeType when updated

    public override string Description1 =>
        "The Tyrannosaurus Rex's huge jaw and sharp teeth deal a ton of damage with each bite. " +
        "T Rex occasionally stomps, stunning Bloons.";

    public override string Description2 =>
        "The Tyrannosaurus Rex's huge jaw and sharp teeth deal a ton of damage and briefly stun bloons with each bite.";

    protected virtual float Factor => 5;

    public override void Apply1(TowerModel model)
    {
        var tRex = model.GetBehavior<BeastHandlerLeashModel>().towerModel;
        var ability = AbilityModel(model);
        var attackModel = ability.GetBehavior<ActivateAttackModel>().attacks[0].Duplicate();
        attackModel.name += "Stomp";

        var weapon = attackModel.weapons[0];
        weapon.Rate = ability.Cooldown / Factor;

        var proj = weapon.projectile;
        proj.radius = Math.Min(proj.radius, 200);

        proj.GetDamageModel().damage /= Factor;

        proj.GetBehavior<SlowModel>().Lifespan /= Factor;
        foreach (var slowModifierForTagModel in proj.GetBehaviors<SlowModifierForTagModel>())
        {
            slowModifierForTagModel.lifespanOverride /= Factor;
        }

        var effect = ability.GetBehavior<CreateEffectOnAbilityModel>().effectModel.Duplicate();
        var effectBehavior =
            new CreateEffectOnExhaustFractionModel("CreateEffectOnExhaustFractionModel_",
                CreatePrefabReference(""), effect, 0,
                Fullscreen.No, 1.0f, -1f, false);
        proj.AddBehavior(effectBehavior);

        tRex.AddBehavior(attackModel);
    }

    public override void Apply2(TowerModel model)
    {
        var tRex = model.GetBehavior<BeastHandlerLeashModel>().towerModel;
        var ability = AbilityModel(model);
        var proj = ability.GetDescendant<WeaponModel>().projectile.Duplicate();

        var mainAttack = tRex.GetAttackModel();
        var mainProjectile = mainAttack.GetDescendant<CreateProjectileOnContactModel>().projectile;

        var slow = proj.GetBehavior<SlowModel>();
        slow.Lifespan /= Factor * 2;
        mainProjectile.AddBehavior(slow);

        foreach (var slowModifierForTagModel in proj.GetBehaviors<SlowModifierForTagModel>())
        {
            slowModifierForTagModel.lifespanOverride /= Factor * 2;
            mainProjectile.AddBehavior(slowModifierForTagModel);
        }

        mainProjectile.collisionPasses = new[] { -1, 0 };
    }

    public override AbilityModel AbilityModel(TowerModel model) =>
        base.AbilityModel(model.GetBehavior<BeastHandlerLeashModel>()?.towerModel ?? model);

    public override void RemoveAbility(TowerModel model) =>
        base.RemoveAbility(model.GetBehavior<BeastHandlerLeashModel>().towerModel);
}