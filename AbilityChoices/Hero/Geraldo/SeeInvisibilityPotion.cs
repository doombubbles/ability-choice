using System.Collections.Generic;
using Il2CppAssets.Scripts.Models.GeraldoItems;

namespace AbilityChoice.AbilityChoices.Hero.Geraldo;

public class SeeInvisibilityPotion : GeraldoAbilityChioce
{
    public override Dictionary<int, string> Descriptions1 => new()
    {
        {
            3,
            "New Item! See Invisibility Potion - grants permanent camo detection to a Monkey Tower or Hero. Geraldo's attack range increases."
        },
        {
            14,
            "See Invisibility Potion grants additional range."
        }
    };

    public override Dictionary<int, string> Descriptions2 => new()
    {
        {
            0,
            "Whatever Monkey you gift with this extremely efficacious elixir will be able to see Camo Bloons with ease! Effects will not wear off."
        },
        {
            14,
            "Whatever Monkey you gift with this extremely efficacious elixir will be able to see Camo Bloons with ease! Effects will not wear off. Now gives increased range!"
        },
        {
            19,
            "Whatever Monkey you gift with this extremely efficacious elixir will be able to see Camo Bloons with ease and do more damage to them! Effects will not wear off."
        }
    };

    protected override int CostMult => 5;

    protected override void Apply(GeraldoItemModel geraldoItem)
    {
        var potion = geraldoItem.GetDescendant<SeeInvisibilityPotionBehaviorModel>();

        potion.rounds = -1;
        potion.upgradedRangeScale += .1f;
        potion.roundsUpgradedTo = -1;
    }
}