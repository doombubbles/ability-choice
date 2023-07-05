using AbilityChoice.Components;
using HarmonyLib;
using Il2CppAssets.Scripts.Unity.UI_New.Main.HeroSelect;

namespace AbilityChoice.Patches;

[HarmonyPatch(typeof(HeroUpgradeDetails), nameof(HeroUpgradeDetails.BindDetails))]
internal static class HeroUpgradeDetails_BindDetails
{
    [HarmonyPostfix]
    private static void Postfix(HeroUpgradeDetails __instance)
    {
        HeroAbilityChoiceInfo.Setup(__instance.selectedHeroId, __instance.heroUpgrades);
    }
}