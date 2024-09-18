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
        model.AddBehavior(new AbilityCooldownScaleSupportModel("", false, 2.0f, false, true,
            Filters, "ArtilleryCommanderBuff", "BuffIconStrikerJones", true));
    }

    private static TowerFilterModel[] Filters => new TowerFilterModel[]
    {
        new FilterInBaseTowerIdModel("", new[] { TowerType.BombShooter, TowerType.MortarMonkey })
    };

    public override void Apply2(TowerModel model)
    {
        model.AddBehavior(new DamageModifierSupportModel("", true, Name + "StunDamage", Filters, true,
            new DamageModifierForBloonStateModel("", "Stun", 1.25f, 0, false, false, false)));
    }

    protected override void ApplyBoth(TowerModel model)
    {
        if (model.tier < 20) return;

        var ability = AbilityModel(model);

        var bonus = CalcAvgBonus(
            ability.GetDescendant<ArtilleryCommandModel>().buffFrames / (float) ability.cooldownFrames, 2);

        model.AddBehavior(new PierceSupportModel("MULT", true, bonus, Name + "Pierce", Filters, true,
            "ArtilleryCommanderBuff",
            "BuffIconStrikerJones"));

        model.AddBehavior(new DamageSupportModel("", true, bonus - 1, Name + "Damage", Filters, true, false, 0)
        {
            buffLocsName = "ArtilleryCommanderBuff",
            buffIconName = "BuffIconStrikerJones"
        });
    }
}