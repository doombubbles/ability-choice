using HarmonyLib;
using Il2CppAssets.Scripts.Unity.UI_New.HeroInGame;

namespace AbilityChoice.Patches;

[HarmonyPatch(typeof(HeroInGameScreen), nameof(HeroInGameScreen.Open))]
internal static class HeroInGameScreen_Open
{
    [HarmonyPostfix]
    private static void Postfix(HeroInGameScreen __instance)
    {
        HeroAbilityChoiceInfo.Setup(__instance.heroId, __instance.heroUpgrades);
    }
}