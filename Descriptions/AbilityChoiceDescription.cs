using BTD_Mod_Helper.Api.Data;
namespace AbilityChoice.Descriptions;

public abstract class AbilityChoiceDescription<T> : ModTextOverride where T : AbilityChoice
{
    protected readonly T abilityChoice;
    protected readonly int mode;

    // Having this constructor makes it not automatically load
    protected AbilityChoiceDescription(T abilityChoice, int mode, string description)
    {
        this.abilityChoice = abilityChoice;
        this.mode = mode;
        TextValue = description;
    }

    public sealed override string Name => abilityChoice.Name;
    public sealed override string TextValue { get; }

    public override bool Active => abilityChoice.Mode == mode;
}