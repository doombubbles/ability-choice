using System.Collections.Generic;
using Il2CppAssets.Scripts.Models.GeraldoItems;

namespace AbilityChoice.AbilityChoices.Hero;

public class Fertilizer : GeraldoAbilityChioce
{
    public override Dictionary<int, string> Descriptions1 => new()
    {
        {
            9,
            "New Item! Fertilizer - targeted Banana Farms that produce visible bananas increase production and get a cash bonus. Geraldo's main attack blast radius increases."
        }
    };

    public override Dictionary<int, string> Descriptions2 => new();

    protected override int CostMult => 15;

    protected override void Apply(GeraldoItemModel geraldoItem)
    {
        geraldoItem.GetDescendant<FertilizerBehaviorModel>().rounds = -1;
    }
}