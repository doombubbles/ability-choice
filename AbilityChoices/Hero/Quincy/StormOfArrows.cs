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

namespace AbilityChoice.AbilityChoices.Hero.Quincy;

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
            FilterInBaseTowerIdModel.Create(new() { baseIds = [TowerType.DartMonkey] }),
            FilterInTowerTiersModel.Create(new()
            {
                path1MinTier = 0, path1MaxTier = 5,
                path2MinTier = 0, path2MaxTier = 5,
                path3MinTier = 3, path3MaxTier = 5
            }),
        };

        model.AddBehavior(RateSupportModel.Create(new()
        {
            multiplier = 1 - mult,
            isUnique = true,
            mutatorId = "QuincyRateBuff",
            isGlobal = true,
            filters = filters,
            buffLocsName = buffIcon.BuffLocsName,
            buffIconName = buffIcon.BuffIconName
        }));

        model.AddBehavior(RangeSupportModel.Create(new()
        {
            isUnique = true,
            multiplier = mult,
            mutatorId = "QuincyRangeBuff",
            filters = filters,
            isGlobal = true,
            buffLocsName = buffIcon.BuffLocsName,
            buffIconName = buffIcon.BuffIconName
        }));

        if (model.tier >= 20)
        {
            model.AddBehavior(DamageModifierSupportModel.Create(new()
            {
                isUnique = true,
                mutatorId = "QuincyDamageBuff",
                filters = filters,
                isGlobal = true,
                damageModifierModel = DamageModifierForTagModel.Create(new()
                {
                    tag = BloonTag.Moabs, damageMultiplier = 1 + mult
                })
            }));

            model.AddBehavior(DamageTypeSupportModel.Create(new()
            {
                isUnique = true,
                mutatorId = "QuincyDamageTypeBuff",
                immuneBloonProperties = BloonProperties.None,
                filters = filters,
                buffLocsName = buffIcon.BuffLocsName,
                buffIconName = buffIcon.BuffIconName
            }));
        }
    }
}