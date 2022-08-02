using BTD_Mod_Helper.Api.Enums;

namespace AbilityChoice.AbilityChoices.Military;

public class PreemptiveStrike : FirstStrikeCapability
{
    public override string UpgradeId => UpgradeType.PreEmptiveStrike;
    public override string AbilityName => "First Strike Capability";


    public override string Description1 => $"{DefaultDescription} ({base.Description1})";
    public override string Description2 => $"{DefaultDescription} ({base.Description2})";
}