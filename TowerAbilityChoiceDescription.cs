namespace AbilityChoice;

public class TowerAbilityChoiceDescription : AbilityChoiceDescription<TowerAbilityChoice>
{
    public TowerAbilityChoiceDescription(TowerAbilityChoice abilityChoice, int mode, string description) : base(
        abilityChoice, mode, description)
    {
    }

    public override string LocalizationKey => abilityChoice.UpgradeId + " Description";
}