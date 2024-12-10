using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;

namespace AbilityChoice.AbilityChoices.Military;

public class ZOMGTakedown : MOABTakedown
{
    public override string UpgradeId => UpgradeType.PirateLord;

    public override string Description1 =>
        "Greatly improved power and can shoot many grappling hooks at once, plundering extra cash from each MOAB-class Bloon taken down..";

    public override string Description2 =>
        "Greatly increased attack speed for all attacks, with further increased MOAB damage.";

    public override void Apply1(TowerModel model)
    {
        var abilityModel = AbilityModel(model);
        var hookAttack = abilityModel.GetDescendant<AttackModel>().Duplicate();
        hookAttack.weapons[0]!.Rate = abilityModel.Cooldown / 2;

        hookAttack.GetDescendants<TargetGrapplableModel>().ForEach(grapplableModel => grapplableModel.hooks /= 2);
        
        model.AddBehavior(hookAttack);
    }

    public override void Apply2(TowerModel model)
    {
        base.Apply2(model);
        
        if (!AbilityChoiceMod.MoreBalanced)
        {
            foreach (var projectileModel in model.GetDescendants<ProjectileModel>().ToList())
            {
                if (projectileModel.id == "Explosion")
                {
                    foreach (var damageModifierForTagModel in projectileModel.GetBehaviors<DamageModifierForTagModel>())
                    {
                        damageModifierForTagModel.damageAddative *= 2;
                    }
                }
                else if (projectileModel.GetDamageModel() != null)
                {
                    projectileModel.AddBehavior(new DamageModifierForTagModel("MoabDamage", "Moabs", 1.0f, 20, false,
                        false));
                    projectileModel.AddBehavior(new DamageModifierForTagModel("MoabDamage", "Ceramic", 1.0f, 5, false,
                        false));
                    projectileModel.hasDamageModifiers = true;
                }
            }
        }
    }
}