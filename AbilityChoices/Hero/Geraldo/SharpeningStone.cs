using System.Collections.Generic;
using Il2CppAssets.Scripts.Models.GeraldoItems;

namespace AbilityChoice.AbilityChoices.Hero.Geraldo;

public class SharpeningStone : GeraldoAbilityChioce
{
    public override Dictionary<int, string> Descriptions1 => new();

    public override Dictionary<int, string> Descriptions2 => new()
    {
        {
            0,
            "Sharpen those darts, friend! Apply this to your darts and blades and they'll be sharper than ever before. Does not run out."
        }
    };

    protected override int CostMult => 5;

    protected override void Apply(GeraldoItemModel geraldoItem)
    {
        geraldoItem.GetDescendant<SharpeningStoneBehaviorModel>().rounds = -1;
    }
}