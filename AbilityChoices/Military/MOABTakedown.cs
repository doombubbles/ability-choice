using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;

namespace AbilityChoice.AbilityChoices.Military;

public class MOABTakedown : TowerAbilityChoice
{
    public override string UpgradeId => UpgradeType.MonkeyPirates;

    public override string Description1 =>
        "Adds 2 new cannons to the ship and cannons attacks do more damage. Also gains a hook attack which periodically harpoons a MOAB class Bloon and brings it down.";

    public override string Description2 =>
        "Adds 2 new cannons to the ship and cannons attacks do more damage. Damage bonuses to MOABs and Ceramics increased.";

    public override void Apply1(TowerModel model)
    {
        var abilityModel = AbilityModel(model);
        var hookAttack = abilityModel.GetDescendant<AttackModel>().Duplicate();
        hookAttack.weapons[0]!.Rate = abilityModel.Cooldown;
        
        model.AddBehavior(hookAttack);
    }

    public override void Apply2(TowerModel model)
    {
        if (AbilityChoiceMod.MoreBalanced)
        {
            model.GetDescendants<DamageModifierForTagModel>().ForEach(modifier =>
            {
                if (modifier.tag == BloonTag.Moabs)
                {
                    modifier.damageAddative *= 1.5f;
                }
            });
        }
        else
        {
            foreach (var projectileModel in model.GetDescendants<ProjectileModel>()
                         .ToList()
                         .Where(projectileModel => projectileModel.id == "Explosion"))
            {
                projectileModel.AddBehavior(new DamageModifierForTagModel("MoabDamage", "Moabs", 1.0f,
                    20, false, false));
                projectileModel.AddBehavior(new DamageModifierForTagModel("MoabDamage", "Ceramic", 1.0f,
                    10, false, false));
                projectileModel.hasDamageModifiers = true;
            }
        }
    }
}