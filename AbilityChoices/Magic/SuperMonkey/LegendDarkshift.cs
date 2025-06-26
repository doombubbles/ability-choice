using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
namespace AbilityChoice.AbilityChoices.Magic.SuperMonkey;

public class LegendDarkshift : TowerAbilityChoice
{
    public override string UpgradeId => UpgradeType.LegendOfTheNight;
    public override string AbilityName => nameof(ChampionDarkshift);

    public override string Description1 => "We turn to him, when all hope is lost... and he's got a bit more range";

    public override void Apply1(TowerModel model)
    {
        model.IncreaseRange(30);
    }
}