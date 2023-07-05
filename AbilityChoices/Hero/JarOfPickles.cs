using System.Collections.Generic;
using Il2CppAssets.Scripts.Models.GeraldoItems;

namespace AbilityChoice.AbilityChoices.Hero;

public class JarOfPickles : GeraldoAbilityChioce
{
    public override Dictionary<int, string> Descriptions1 => new();

    public override Dictionary<int, string> Descriptions2 => new()
    {
        { 0, "Try these pickles on one of your Monkey Towers and it will hit harder, but shoot slower, permanently." },
        {
            11,
            "Try these pickles on one of your Monkey Towers and it will hit harder, but shoot slower, permanently. Now with increased damage to Fortified Bloons!"
        },
        {
            16,
            "Try these pickles on one of your Monkey Towers and it will hit harder, but shoot slower, permanently. Now with EVEN MORE increased damage to Fortified Bloons!"
        }
    };

    protected override int CostMult => 10;

    protected override void Apply(GeraldoItemModel geraldoItem)
    {
        geraldoItem.GetDescendant<JarOfPicklesBehaviorModel>().rounds = -1;
    }
}