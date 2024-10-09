using System;
using Il2CppAssets.Scripts.Models.CorvusSpells;
using Il2CppAssets.Scripts.Models.Towers;

namespace AbilityChoice.AbilityChoices.Hero.Corvus;

public class Haste : CorvusAbilityChoice
{
    protected override int Order => 1;
    
    public override void Apply1(TowerModel model)
    {
        var spell = InstantSpell(model);

        spell.initialManaCost = (int) Math.Round(spell.initialManaCost / spell.duration);

        spell.duration = 1;
        spell.cooldown = 0;
    }

    public override void Apply2(TowerModel model)
    {
        var spell = InstantSpell(model);

        var factor = spell.duration / (spell.duration + spell.cooldown);

        spell.initialManaCost = (int) (factor * (spell.initialManaCost / spell.duration));

        spell.duration = 1;
        spell.cooldown = 0;

        var haste = spell.Cast<HasteModel>();

        haste.orbitSpeedMultiplier = CalcAvgBonus(factor, haste.orbitSpeedMultiplier);
        haste.turnSpeedMultiplier = CalcAvgBonus(factor, haste.turnSpeedMultiplier);
    }
}