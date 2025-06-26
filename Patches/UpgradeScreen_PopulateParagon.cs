using HarmonyLib;
using Il2CppAssets.Scripts.Unity.UI_New.Upgrade;

namespace AbilityChoice.Patches;

[HarmonyPatch(typeof(UpgradeScreen), nameof(UpgradeScreen.PopulateParagon))]
internal static class UpgradeScreen_PopulateParagon
{
    [HarmonyPostfix]
    private static void Postfix(UpgradeScreen __instance)
    {
        var tower = __instance.currTowerId;
        if (__instance.paragonUpgradeDetails.isActiveAndEnabled)
        {
            TowerAbilityChoiceInfo.Setup(__instance.paragonUpgradeDetails);
        }
        else
        {
            TaskScheduler.ScheduleTask(() => TowerAbilityChoiceInfo.Setup(__instance.paragonUpgradeDetails),
                () => __instance.paragonUpgradeDetails.isActiveAndEnabled, () => __instance.currTowerId != tower);
        }
    }
}