using BTD_Mod_Helper.Api.Enums;

namespace AbilityChoice.AbilityChoices.Support;

public class HomelandDefense : CallToArms
{
    public override string UpgradeId => UpgradeType.HomelandDefense;

    public override string AbilityName => "Homeland Defense";

    public override string Description1 => "Permanent global attack speed / pierce buff.";

    protected override bool IsGlobal => true;
}