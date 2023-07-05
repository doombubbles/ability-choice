using System.Collections.Generic;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.GeraldoItems;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;

namespace AbilityChoice.AbilityChoices.Hero;

public class GenieBottle : GeraldoAbilityChioce
{
    public override Dictionary<int, string> Descriptions1 => new()
    {
        {
            12,
            "New Item! Genie Bottle - summons a plasma-powered Genie Monkey. Creepy Idol can now apply an occasional short stun to passing visible MOAB-class Bloons."
        }
    };

    public override Dictionary<int, string> Descriptions2 => new()
    {
        {
            0,
            "Summon a Genie Monkey from this brilliant blue bottle and rejoice, it lasts forever! (no wishes included with purchase)."
        },
        {
            18,
            "Summon a Genie Monkey from this brilliant gold bottle  and rejoice, it lasts forever! (no wishes included with purchase). Extra attack and more damage to MOABs now included."
        }
    };

    protected override int CostMult => 15;

    protected override void Apply(GeraldoItemModel geraldoItem)
    {
    }

    public override void Apply(GameModel gameModel)
    {
        base.Apply(gameModel);

        foreach (var towerModel in gameModel.GetTowersWithBaseId(TowerType.GenieBottle))
        {
            towerModel.GetDescendant<TowerExpireModel>().rounds = -1;
        }
    }
}