using Il2CppAssets.Scripts.Models.CorvusSpells;
using Il2CppAssets.Scripts.Models.Towers;

namespace AbilityChoice.AbilityChoices.Hero.Corvus;

public class Spear : CorvusAbilityChoice
{
    protected override int Order => 0;
    
    public override void Apply1(TowerModel model)
    {
        var spell = ContinuousSpell(model);
        spell.ongoingManaCost += spell.initialManaCost / ContinuousFactor;
        spell.initialManaCost = spell.ongoingManaCost;
    }

    public override void Apply2(TowerModel model)
    {
        var spell = ContinuousSpell(model).Cast<SpearModel>();

        var factor = (spell.ongoingManaCost + spell.initialManaCost / (float) ContinuousFactor) / spell.ongoingManaCost;

        spell.initialManaCost = spell.ongoingManaCost;

        spell.attack.weapons[0]!.Rate *= factor;
    }
}