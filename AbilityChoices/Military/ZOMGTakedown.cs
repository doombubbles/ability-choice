using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;

namespace AbilityChoice.AbilityChoices.Military;

public class ZOMGTakedown : MOABTakedown
{
    public override string UpgradeId => UpgradeType.PirateLord;

    public override string Description1 =>
        "Greatly increased attack speed for all attacks, and hook attack is faster, and can also pull in BFBs.";

    public override string Description2 =>
        "Greatly increased attack speed for all attacks, with further increased MOAB and Ceramic damage.";

    protected override bool FilterBFBs => false;

    protected override void Apply2(TowerModel model)
    {
        base.Apply2(model);
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