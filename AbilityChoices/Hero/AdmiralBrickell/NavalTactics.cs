using System;
using System.Collections.Generic;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Weapons;

namespace AbilityChoice.AbilityChoices.Hero.AdmiralBrickell;

public class NavalTactics : HeroAbilityChoice
{
    public override string HeroId => TowerType.AdmiralBrickell;

    public override Dictionary<int, string> Descriptions1 => new()
    {
        { 3, "Increases attack speed of Brickell and nearby water-based Monkeys." },
        { 5, "Nearby water-based Monkeys can hit all Bloon types except Camo." },
        { 8, "Water towers in radius gain extra pierce and can hit Camo bloons." },
        { 14, "Nearby water-based monkeys have further increased attack speed." },
        { 19, "Brickell's buffs affect all water based towers everywhere." }
    };

    public override Dictionary<int, string> Descriptions2 => new()
    {
        { 3, "Further increased attack speed." },
        { 5, "Increased popping power, and attacks can hit all Bloon types except Camo." },
        { 8, "Gains extra pierce." },
        { 14, "Further increased attack speed and pierce." },
        { 19, "Drastically increased attack speed and pierce." }
    };

    public override void Apply1(TowerModel model)
    {
        var ability = AbilityModel(model);

        var rateBuff = ability.GetDescendant<ActivateRateSupportZoneModel>();
        var camoBuff = ability.GetDescendant<ActivateVisibilitySupportZoneModel>();
        var damageBuff = ability.GetDescendant<ActivateTowerDamageSupportZoneModel>();

        var bonus = CalcAvgBonus(rateBuff.lifespan / ability.Cooldown, 1 / rateBuff.rateModifier);

        var isGlobal = model.tier >= 19;

        model.AddBehavior(new RateSupportModel("", 1 / bonus, rateBuff.isUnique, rateBuff.mutatorId, isGlobal,
                0, rateBuff.filters, rateBuff.buffLocsName, rateBuff.buffIconName)
            { appliesToOwningTower = rateBuff.canEffectThisTower });

        if (damageBuff != null)
        {
            model.AddBehavior(new DamageTypeSupportModel("", damageBuff.isUnique, damageBuff.mutatorId,
                damageBuff.immuneBloonProperties, damageBuff.filters, damageBuff.buffLocsName,
                damageBuff.buffIconName)
            {
                appliesToOwningTower = damageBuff.canEffectThisTower,
                isGlobal = isGlobal
            });
        }

        if (camoBuff != null)
        {
            model.AddBehavior(new VisibilitySupportModel("", camoBuff.isUnique, camoBuff.mutatorId, isGlobal,
                camoBuff.filters, camoBuff.buffLocsName, camoBuff.buffIconName)
            {
                appliesToOwningTower = camoBuff.canEffectThisTower
            });
        }
    }

    public override void Apply2(TowerModel model)
    {
        var ability = AbilityModel(model);

        var rateBuff = ability.GetDescendant<ActivateRateSupportZoneModel>();
        var damageBuff = ability.GetDescendant<ActivateTowerDamageSupportZoneModel>();
        var pierceBuff = ability.GetDescendant<ActivatePierceSupportZoneModel>();

        var factor = model.tier >= 19 ? 2.1f : model.tier >= 14 ? 1.1f : .6f;

        if (rateBuff != null)
        {
            model.GetDescendants<WeaponModel>()
                .ForEach(weaponModel => weaponModel.Rate *= (float) Math.Pow(rateBuff.rateModifier, factor));
        }

        if (damageBuff != null)
        {
            ability.GetDescendants<DamageModel>().ForEach(damageModel =>
            {
                damageModel.damage += damageBuff.damageIncrease * factor;
                damageModel.immuneBloonProperties =
                    damageModel.immuneBloonPropertiesOriginal = damageBuff.immuneBloonProperties;
            });
        }

        if (pierceBuff != null)
        {
            ability.GetDescendants<ProjectileModel>().ForEach(projectileModel =>
            {
                projectileModel.pierce += pierceBuff.pierceIncrease * (int) Math.Ceiling(factor);
            });
        }
    }
}