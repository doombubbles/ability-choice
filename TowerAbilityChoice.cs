using System.Collections.Generic;
using BTD_Mod_Helper.Api.Enums;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Unity;
using Il2CppNinjaKiwi.Common;

namespace AbilityChoice;

public abstract class TowerAbilityChoice : AbilityChoice
{
    public static readonly Dictionary<string, TowerAbilityChoice> Cache = new();

    public abstract string Description1 { get; }
    public virtual string Description2 => "";

    public abstract string UpgradeId { get; }

    protected string DefaultDescription { get; private set; }
    protected TowerModel BaseTowerModel { get; private set; }

    protected sealed override bool HasMode2 => !string.IsNullOrEmpty(Description2);

    public sealed override IEnumerable<ModContent> Load()
    {
        yield return this;
        yield return new TowerAbilityChoiceDescription(this, 1, Description1);

        if (HasMode2)
        {
            yield return new TowerAbilityChoiceDescription(this, 2, Description2);
        }
    }

    public override void Register()
    {
        base.Register();

        BaseTowerModel = GetAffected(Game.instance.model).First();
        Cache[UpgradeId] = this;
        DefaultDescription = LocalizationManager.Instance.textTable[UpgradeId + " Description"];
    }

    public override IEnumerable<TowerModel> GetAffected(GameModel gameModel) =>
        gameModel.towers.Where(model => model.appliedUpgrades.Contains(UpgradeId))
            .Where(AppliesTo)
            .OrderBy(model => model.appliedUpgrades.Length);

    public static string IconForMode(int mode) => mode switch
    {
        1 => VanillaSprites.NotifyRed,
        2 => VanillaSprites.NotifyBlue,
        _ => VanillaSprites.NotificationYellow
    };
}