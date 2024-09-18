using System.Collections.Generic;
using Il2CppAssets.Scripts.Models.GeraldoItems;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;

namespace AbilityChoice.AbilityChoices.Hero.Geraldo;

public class TubeOfAmazoGlue : GeraldoAbilityChioce
{
    public override Dictionary<int, string> Descriptions1 => new();

    public override Dictionary<int, string> Descriptions2 => new()
    {
        {
            0,
            "Truly amazing! Apply a tube of this modern marvel anywhere on the track and watch the next bunch of Bloons slow right down. Lasts until used up."
        }
    };

    protected override int CostMult => 2;

    protected override void Apply(GeraldoItemModel geraldoItem)
    {
        geraldoItem.GetDescendant<AgeModel>().rounds = 0;
    }
}