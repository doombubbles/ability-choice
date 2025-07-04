﻿using System.Collections.Generic;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;

namespace AbilityChoice.AbilityChoices.Hero.Benjamin;

public class Biohack : HeroAbilityChoice
{
    public override string HeroId => TowerType.Benjamin;

    public override Dictionary<int, string> Descriptions1 => new()
    {
        {
            3,
            "Periodically Biohacks the 4 closest Monkeys, making them pop an extra layer for a few attacks but then skipping their next."
        },
        { 13, "Biohack increases bonus damage and affects 6 Monkeys at a time." },
        { 19, "Biohack lasts for more attacks and affected Monkeys pop 3 extra layers instead of 2." }
    };

    public override Dictionary<int, string> Descriptions2 => new()
    {
        { 3, "Biohacks the closest other tower, letting them pop an extra layer per attack." },
        { 13, "Biohack increases bonus damage." },
        { 19, "Biohack makes the affected monkey pop 3 extra layers instead of 2." }
    };

    private const int Factor = 5;

    public override void Apply1(TowerModel model)
    {
        var ability = AbilityModel(model);

        ability.Cooldown /= Factor;

        var biohack = ability.GetDescendant<BiohackModel>();

        biohack.lifespan /= Factor;
        biohack.lifespanFrames /= Factor;

        ability.GetDescendants<DelayedShutoffModel>().ForEach(shutoffModel =>
        {
            shutoffModel.delay /= Factor;
            shutoffModel.delayFrames /= Factor;

            shutoffModel.shutoffTime /= Factor;
            shutoffModel.shutoffTimeFrames /= Factor;

            shutoffModel.effectModel.lifespan /= Factor;
        });

        biohack.effectModel.lifespan /= Factor;
    }

    public override void Apply2(TowerModel model)
    {
        var ability = AbilityModel(model);


        var biohack = ability.GetDescendant<BiohackModel>();

        ability.Cooldown = biohack.lifespan;
        biohack.affectedCount = 1;

        ability.GetDescendants<DelayedShutoffModel>().ForEach(shutoffModel =>
        {
            shutoffModel.shutoffTime = shutoffModel.shutoffTimeFrames = 0;
            shutoffModel.effectModel = null;
        });

        ability.animation = -1;
    }

    protected override void ApplyBoth(TowerModel model)
    {
        var ability = AbilityModel(model);
        ability.RemoveBehaviors<CreateEffectOnAbilityModel>();
        ability.RemoveBehaviors<CreateSoundOnAbilityModel>();
    }

    protected override void RemoveAbility(TowerModel model)
    {
        TechBotify(model);
    }
}