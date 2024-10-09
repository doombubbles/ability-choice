using System.Collections.Generic;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;

namespace AbilityChoice.AbilityChoices.Hero.Corvus;

public class SpiritWalk : HeroAbilityChoice
{
    public override string HeroId => TowerType.Corvus;

    public override Dictionary<int, string> Descriptions1 => new()
    {
        {7, "Corvus gains significantly increased range. Learns: Ancestral Might, Malevolence"}
    };

    protected override int Order => 1;

    public override void Apply1(TowerModel model)
    {
        model.range *= 1.5f;
        foreach (var attackModel in model.GetAttackModels())
        {
            attackModel.range *= 1.5f;
        }
    }
}