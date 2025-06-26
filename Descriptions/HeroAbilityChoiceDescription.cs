namespace AbilityChoice.Descriptions;

public class HeroAbilityChoiceDescription : AbilityChoiceDescription<HeroAbilityChoice>
{
    private readonly int level;

    // Having this constructor makes it not automatically load
    public HeroAbilityChoiceDescription(HeroAbilityChoice abilityChoice, int level, int mode, string description) :
        base(abilityChoice, mode, description)
    {
        this.level = level;
    }

    public override string LocalizationKey => $"{abilityChoice.HeroId} Level {level} Description";
}