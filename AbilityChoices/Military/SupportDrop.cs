using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using Il2Cpp;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Filters;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;

namespace AbilityChoice.AbilityChoices.Military;

public class SupportDrop : TowerAbilityChoice
{
    public override string UpgradeId => UpgradeType.SupportChinook;

    public override string Description1 =>
        "Occasionally drops lives and cash crates. Can pick up and redeploy most Monkey types.";

    public override string Description2 =>
        "Downdraft attack is stronger and damages Moab class bloons. Can pick up and redeploy most Monkey types.";

    protected override void Apply1(TowerModel model)
    {
        TechBotify(model);
    }

    protected override void Apply2(TowerModel model)
    {
        var downDraft = model.GetAttackModel("Downdraft")!;
        var projectileModel = downDraft.GetDescendant<ProjectileModel>();
        var windModel = projectileModel.GetBehavior<WindModel>();
        windModel.distanceMin = windModel.distanceMax;
        projectileModel.AddBehavior(new DamageModel("DamageModel_", 1, 0, true, false, true,
            BloonProperties.None, BloonProperties.None));
        projectileModel.AddBehavior(new DamageModifierForTagModel("DamageModifierForTagModel_", "Moabs", 1, 9,
            false, true));
        projectileModel.hasDamageModifiers = true;

        var filter = projectileModel.GetBehavior<ProjectileFilterModel>();
        filter.filters = filter.filters
            .Where(filterModel => !(filterModel.Is(out FilterOutTagModel f) && f.tag == BloonTag.Moabs))
            .ToIl2CppReferenceArray();
    }

    protected override void RemoveAbility(TowerModel model)
    {
        if (Mode2)
        {
            base.RemoveAbility(model);
        }
    }
}