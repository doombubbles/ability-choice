using BTD_Mod_Helper.Api.Enums;
namespace AbilityChoice.AbilityChoices.Support.MonkeyVillage;

public class HomelandDefense : CallToArms
{
    public override string UpgradeId => UpgradeType.HomelandDefense;

    public override string AbilityName => UpgradeType.HomelandDefense;

    public override string Description1 => "Permanent global moderate attack speed / pierce buff.";

    public override string Description2 => "Permanent global strong attack speed buff.";

    protected override bool IsGlobal => true;
}