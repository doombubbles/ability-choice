using System.Collections.Generic;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.GeraldoItems;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;

namespace AbilityChoice.AbilityChoices.Hero.Geraldo;

public class BottleHotSauce : GeraldoAbilityChioce
{
    public override Dictionary<int, string> Descriptions1 => new()
    {
        {
            8,
            "New Item! Bottle of 'Gerry's Fire' - 2.8 million Bloonville units of pure hot sauce fire! Gives a Monkey, Hero, or minion a fiery attack permanently."
        }
    };

    public override Dictionary<int, string> Descriptions2 => new()
    {
        {
            0,
            "2.8 million Bloonville units. Whoever tastes this special sauce will be spewing fire permanently!"
        },
        {
            16,
            "2.8 million Bloonville units. Whoever tastes this special sauce will be spewing fire permanently! Now with extra burny-ness."
        }
    };

    protected override int CostMult => 5;

    protected override void Apply(GeraldoItemModel geraldoItem)
    {
    }

    public override void Apply(GameModel gameModel)
    {
        base.Apply(gameModel);

        foreach (var towerModel in gameModel.GetTowersWithBaseId(TowerType.HotSauceCreature))
        {
            towerModel.GetDescendant<TowerExpireModel>().rounds = -1;
        }
    }
}