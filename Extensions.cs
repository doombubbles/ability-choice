using Assets.Scripts.Unity.UI_New.Upgrade;

namespace AbilityChoice;

public static class Extensions
{
    public static AbilityChoice AbilityChoice(this UpgradeDetails upgradeDetails)
    {
        var name = upgradeDetails.upgrade?.name ?? "";
        global::AbilityChoice.AbilityChoice.Cache.TryGetValue(name, out var abilityChoice);
        return abilityChoice;
    }
}