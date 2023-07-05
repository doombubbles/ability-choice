using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Filters;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace AbilityChoice.AbilityChoices.Military;

public class MOABTakedown : TowerAbilityChoice
{
    private const int MoabWorthMultiplier = 6;
    public override string UpgradeId => UpgradeType.MonkeyPirates;

    public override string Description1 =>
        "Adds 2 new cannons to the ship and cannons attacks do more damage. Also gains a hook attack which can periodically take-down MOABs or DDTs.";

    public override string Description2 =>
        "Adds 2 new cannons to the ship and cannons attacks do more damage, further increased against MOABs and Ceramics.";

    protected virtual bool FilterBFBs => true;

    protected override void Apply1(TowerModel model)
    {
        var abilityModel = AbilityModel(model);
        var hookAttack = abilityModel.GetDescendant<AttackModel>().Duplicate();
        hookAttack.weapons[0].Rate = abilityModel.Cooldown / MoabWorthMultiplier;
        if (FilterBFBs)
        {
            var filter = hookAttack.GetDescendant<AttackFilterModel>();
            var bfbFilter = new FilterOutTagModel("FilterOutTagModel_Grapple", "Bfb", new Il2CppStringArray(0));
            filter.filters = filter.filters.AddTo(bfbFilter);
            filter.AddChildDependant(bfbFilter);
        }

        model.AddBehavior(hookAttack);
    }

    protected override void Apply2(TowerModel model)
    {
        foreach (var projectileModel in model.GetDescendants<ProjectileModel>()
                     .ToList()
                     .Where(projectileModel => projectileModel.id == "Explosion"))
        {
            projectileModel.AddBehavior(new DamageModifierForTagModel("MoabDamage", "Moabs", 1.0f,
                AbilityChoiceMod.MoreBalanced ? 10 : 20, false, false));
            projectileModel.AddBehavior(new DamageModifierForTagModel("MoabDamage", "Ceramic", 1.0f,
                AbilityChoiceMod.MoreBalanced ? 5 : 10, false, false));
            projectileModel.hasDamageModifiers = true;
        }
    }
}