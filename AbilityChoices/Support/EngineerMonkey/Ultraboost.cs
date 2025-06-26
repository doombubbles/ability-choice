using BTD_Mod_Helper.Api.Enums;
namespace AbilityChoice.AbilityChoices.Support.EngineerMonkey;

public class Ultraboost : Overclock
{
    public override string UpgradeId => UpgradeType.Ultraboost;

    public override string Description2 => "All towers in range are fully overclocked.";

    protected override float Multiplier => 1f;
}