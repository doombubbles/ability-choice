using System.Collections.Generic;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.TowerFilters;

namespace AbilityChoice.AbilityChoices.Hero.StrikerJones;

public class ArtilleryCommand : HeroAbilityChoice
{
    public override string HeroId => TowerType.StrikerJones;

    public override Dictionary<int, string> Descriptions1 => new()
    {
        { 10, "All Bomb Shooters and Mortar Monkeys have 50% reduced ability cooldowns." },
        { 20, "All Bomb Shooters and Mortar Monkeys have increased damage and pops per shot." }
    };

    public override Dictionary<int, string> Descriptions2 => new()
    {
        { 10, "All Bomb Shooters and Mortar Monkeys deal 25% increased damage to Stunned bloons." },
        { 20, "All Bomb Shooters and Mortar Monkeys have increased damage and pops per shot." }
    };


    public override void Apply1(TowerModel model)
    {
        model.AddBehavior(AbilityCooldownScaleSupportModel.Create(new()
        {
            abilityCooldownSpeedScale = 2.0f,
            isGlobal = true,
            filters = Filters,
            buffLocsName = "ArtilleryCommanderBuff",
            buffIconName = "BuffIconStrikerJones",
            onlyShowBuffIfMutated = true
        }));
    }

    private static TowerFilterModel[] Filters => new TowerFilterModel[]
    {
        FilterInBaseTowerIdModel.Create(new() { baseIds = [TowerType.BombShooter, TowerType.MortarMonkey] })
    };

    public override void Apply2(TowerModel model)
    {
        model.AddBehavior(DamageModifierSupportModel.Create(new()
        {
            isUnique = true,
            mutatorId = Name + "StunDamage",
            filters = Filters,
            isGlobal = true,
            damageModifierModel = DamageModifierForBloonStateModel.Create(new()
            {
                bloonState = "Stun",
                damageMultiplier = 1.25f
            })
        }));
    }

    protected override void ApplyBoth(TowerModel model)
    {
        if (model.tier < 20) return;

        var ability = AbilityModel(model);

        var bonus = CalcAvgBonus(
            ability.GetDescendant<ArtilleryCommandModel>().buffFrames / (float) ability.cooldownFrames, 2);

        model.AddBehavior(PierceSupportModel.Create(new()
        {
            name = "MULT",
            isUnique = true,
            pierce = bonus,
            mutatorId = Name + "Pierce",
            filters = Filters,
            isGlobal = true,
            buffLocsName = "ArtilleryCommanderBuff",
            buffIconName = "BuffIconStrikerJones"
        }));

        model.AddBehavior(DamageSupportModel.Create(new()
        {
            isUnique = true,
            increase = bonus - 1,
            mutatorId = Name + "Damage",
            filters = Filters,
            isGlobal = true,
            buffLocsName = "ArtilleryCommanderBuff",
            buffIconName = "BuffIconStrikerJones"
        }));
    }
}