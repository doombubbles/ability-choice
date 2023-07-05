using System.Collections.Generic;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;

namespace AbilityChoice.AbilityChoices.Hero;

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

    protected override void Apply1(TowerModel model)
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

    protected override void Apply2(TowerModel model)
    {
        // TODO naval tactics 2
    }
}