using Il2CppAssets.Scripts.Models.Towers;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;

namespace AbilityChoice.AbilityChoices.Magic;

public class ChampionDarkshift : AbilityChoice
{
    public override string UpgradeId => UpgradeType.DarkChampion;
    public override string AbilityName => Name;

    public override string Description1 => "Champion's dark blades excel at puncturing and ruining all Bloon types. Further increased range.";
        
    public override void Apply1(TowerModel model)
    {
        model.IncreaseRange(20);
    }
}