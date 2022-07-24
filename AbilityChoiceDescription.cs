using BTD_Mod_Helper.Api.Data;

namespace AbilityChoice;

public class AbilityChoiceDescription : ModTextOverride
{
    private readonly AbilityChoice abilityChoice;

    public override string Name => abilityChoice.Name;

    public override string LocalizationKey => abilityChoice.UpgradeId + " Description";

    public override string TextValue => abilityChoice.CurrentDescription;

    public override bool Active => abilityChoice.Enabled;

    // Having this constructor makes it not automatically load
    public AbilityChoiceDescription(AbilityChoice abilityChoice)
    {
        this.abilityChoice = abilityChoice;
    }
}