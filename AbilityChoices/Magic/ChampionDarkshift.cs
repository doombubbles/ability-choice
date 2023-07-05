using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;

namespace AbilityChoice.AbilityChoices.Magic;

public class ChampionDarkshift : TowerAbilityChoice
{
    public override string UpgradeId => UpgradeType.DarkChampion;
    public override string AbilityName => Name;

    public override string Description1 =>
        "Champion's dark blades excel at puncturing and ruining all Bloon types. Further increased range.";

    protected override void Apply1(TowerModel model)
    {
        model.IncreaseRange(20);
    }
}