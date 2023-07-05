using AbilityChoice.Components;
using HarmonyLib;
using Il2CppAssets.Scripts.Unity.UI_New.Upgrade;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace AbilityChoice.Patches;

[HarmonyPatch(typeof(UpgradeScreen), nameof(UpgradeScreen.PopulatePath))]
internal static class UpgradeScreen_PopulatePath
{
    [HarmonyPostfix]
    private static void Postfix(Il2CppReferenceArray<UpgradeDetails> pathUpgrades)
    {
        TowerAbilityChoiceInfo.Setup(pathUpgrades);
    }
}