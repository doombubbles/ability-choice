using System;
using System.Collections.Generic;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.GeraldoItems;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Unity;

namespace AbilityChoice;

public abstract class GeraldoAbilityChioce : HeroAbilityChoice
{
    public sealed override string HeroId => TowerType.Geraldo;

    protected abstract int CostMult { get; }

    protected sealed override bool HasMode2 => false;

    public abstract override Dictionary<int, string> Descriptions2 { get; }

    public GeraldoItemModel GeraldoItem(GameModel gameModel = null) =>
        (gameModel ?? Game.instance.model).GetGeraldoItemWithName(Name);

    public override bool AppliesTo(TowerModel towerModel) =>
        Math.Clamp(GeraldoItem().levelUnlockedAt, 1, 20) == towerModel.tier;

    public override IEnumerable<ModContent> Load()
    {
        yield return this;

        foreach (var (level, description) in Descriptions1)
        {
            yield return new HeroAbilityChoiceDescription(this, level, 1, description);
        }

        foreach (var (level, description) in Descriptions2)
        {
            yield return new GeraldoAbilityChoiceDescription(this, level, description);
        }
    }

    public sealed override void Apply1(TowerModel model)
    {
    }

    public sealed override void Apply2(TowerModel model)
    {
    }

    public override void Apply(GameModel gameModel)
    {
        var geraldoItem = GeraldoItem(gameModel);

        geraldoItem.cost *= CostMult;
        Apply(geraldoItem);
    }

    protected abstract void Apply(GeraldoItemModel geraldoItem);
}