namespace AbilityChoice.Descriptions;

public class GeraldoAbilityChoiceDescription : AbilityChoiceDescription<GeraldoAbilityChioce>
{
    private readonly int level;

    // Having this constructor makes it not automatically load
    public GeraldoAbilityChoiceDescription(GeraldoAbilityChioce abilityChoice, int level, string description) :
        base(abilityChoice, 1, description)
    {
        this.level = level;
    }

    public override string LocalizationKey => $"{abilityChoice.GeraldoItem().locsId} description" +
                                              (level > 0 ? $" level {level}" : "");
}