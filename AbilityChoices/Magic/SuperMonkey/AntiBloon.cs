using BTD_Mod_Helper.Api.Enums;
namespace AbilityChoice.AbilityChoices.Magic.SuperMonkey;

public class AntiBloon : TechTerror
{
    public override string UpgradeId => UpgradeType.TheAntiBloon;
    public override string AbilityName => "The Anti-Bloon";
    public override string BackUpAbilityName => "";

    public override string Description1 => "Frequently eradicates nearby Bloons.";
    public override string Description2 => "Nanobot plasma seeks out and destroys Bloons with strong Crits";
}