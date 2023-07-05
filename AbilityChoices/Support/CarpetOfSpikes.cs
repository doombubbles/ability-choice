using BTD_Mod_Helper.Api.Enums;
using Il2CppAssets.Scripts.Models.Towers;

namespace AbilityChoice.AbilityChoices.Support;

public class CarpetOfSpikes : SpikeStorm
{
    public override string UpgradeId => UpgradeType.CarpetOfSpikes;
    public override string AbilityName => UpgradeType.SpikeStorm;

    public override string Description1 =>
        "Regularly sets a carpet of spikes over the whole track alongside the stream.";

    public override string Description2 =>
        "Regularly sets a carpet of spikes over the whole track alongside the accelerated production.";

    protected override void ApplyBoth(TowerModel model)
    {
        var ability = AbilityModel(model);
        ability.CooldownSpeedScale = -1;
    }
}