using Il2CppAssets.Scripts.Models.CorvusSpells;
using Il2CppAssets.Scripts.Models.Towers;

namespace AbilityChoice.AbilityChoices.Hero.Corvus;

public class Aggression : CorvusAbilityChoice
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

        var aggression = spell.Cast<AggressionModel>();

        aggression.sizeMultiplier = CalcAvgBonus(1 / factor, aggression.sizeMultiplier);
        aggression.rehitCooldownMultiplier = 1 / CalcAvgBonus(1 / factor, 1 / aggression.rehitCooldownMultiplier);
    }
}