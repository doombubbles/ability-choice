﻿using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.CorvusSpells;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Weapons;

namespace AbilityChoice.AbilityChoices.Hero.Corvus;

public class Malevolence : CorvusAbilityChoice
{
    protected override int Order => 1;
    
    public override void Apply1(TowerModel model)
    {
        var spell = ContinuousSpell(model);
        spell.ongoingManaCost += spell.initialManaCost / ContinuousFactor;
        spell.initialManaCost = spell.ongoingManaCost;
    }

    public override void Apply2(TowerModel model)
    {
        var spell = ContinuousSpell(model);

        var factor = (spell.ongoingManaCost + spell.initialManaCost / (float) ContinuousFactor) / spell.ongoingManaCost;

        spell.initialManaCost = spell.ongoingManaCost;

        spell.GetDescendant<WeaponModel>().Rate *= factor;
    }
}