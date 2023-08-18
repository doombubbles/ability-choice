using System.Collections.Generic;
using AbilityChoice.Displays;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using Il2Cpp;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.TowerFilters;

namespace AbilityChoice.AbilityChoices.Hero;

public class StormOfArrows : HeroAbilityChoice
{
    public override string AbilityName => "Storm of Arrows";

    public override string HeroId => TowerType.Quincy;

    public override Dictionary<int, string> Descriptions1 => new()
    {
        { 10, "Quincy occasionally fires a barrage of arrows at Bloons anywhere on screen." },
        { 18, "Quincy attacks faster and barrages are more frequent with 50% more arrows." },
        { 20, "Quincy attacks even faster. Arrow barrages deal more damage and fire even more arrows." }
    };

    public override Dictionary<int, string> Descriptions2 => new()
    {
        { 10, "Quincy's attacks do increased damage. 15% increased attack speed and range for all Crossbow Monkeys." },
        { 18, "Quincy attacks faster. 30% increased attack speed and range for all Crossbow Monkeys." },
        {
            20,
            "Quincy attacks even faster. Crossbow Monkeys deal 30% more damage to MOAB-Class Bloons and can damage all Bloon types."
        }
    };

    public override void Apply1(TowerModel model)
    {
        var abilityModel = AbilityModel(model);

        var attackModel = abilityModel.GetDescendant<AttackModel>().Duplicate();
        var weapon = attackModel.weapons[0];

        var mainProj = attackModel.GetDescendant<ProjectileModel>();
        var realProj = mainProj.GetDescendant<ProjectileModel>();
        var damage = realProj.GetBehavior<DamageModel>();
        var age = realProj.GetBehavior<AgeModel>();
        var effect = mainProj.GetDescendant<CreateEffectOnExhaustFractionModel>().effectModel;

        var factor = age.Lifespan * 2;

        effect.lifespan = age.Lifespan = 1;
        damage.damage /= 2;

        weapon.Rate = abilityModel.Cooldown / factor;

        realProj.scale = effect.scale = 0.5f;

        model.AddBehavior(attackModel);
    }

    public override void Apply2(TowerModel model)
    {
        var mult = model.tier >= 18 ? .3f : .15f;
        var buffIcon = GetInstance<BuffIconQuincy>();

        var filters = new TowerFilterModel[]
        {
            new FilterInBaseTowerIdModel("", new[] { TowerType.DartMonkey }),
            new FilterInTowerTiersModel("", 0, 5, 0, 5, 3, 5),
        };

        model.AddBehavior(new RateSupportModel("", 1 - mult, true, "QuincyRateBuff", true, 0,
            filters, buffIcon.BuffLocsName, buffIcon.BuffIconName));

        model.AddBehavior(new RangeSupportModel("", true, mult, 0, "QuincyRangeBuff",
            filters, true, buffIcon.BuffLocsName, buffIcon.BuffIconName));

        if (model.tier >= 20)
        {
            model.AddBehavior(new DamageModifierSupportModel("", true, "QuincyDamageBuff", filters, true,
                new DamageModifierForTagModel("", BloonTag.Moabs, 1 + mult, 0, false, false)));

            model.AddBehavior(new DamageTypeSupportModel("", true, "QuincyDamageTypeBuff", BloonProperties.None,
                filters, buffIcon.BuffLocsName, buffIcon.BuffIconName));
        }
    }
}