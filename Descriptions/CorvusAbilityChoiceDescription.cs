namespace AbilityChoice.Descriptions;

public class CorvusAbilityChoiceDescription : AbilityChoiceDescription<CorvusAbilityChoice>
{
    private bool lvl20;
    
    // Having this constructor makes it not automatically load
    public CorvusAbilityChoiceDescription(CorvusAbilityChoice abilityChoice, int mode, string description, bool lvl20 = false) :
        base(abilityChoice, mode, description)
    {
        this.lvl20 = lvl20;
    }

    public override string LocalizationKey => $"{abilityChoice.Name} description" + (lvl20 ? " level 20" : "");
}