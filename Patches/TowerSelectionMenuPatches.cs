using System.Collections.Generic;
using System.Reflection;
using AbilityChoice.AbilityChoices.Hero;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2CppAssets.Scripts.Unity.UI_New.InGame.TowerSelectionMenu;
using Il2CppAssets.Scripts.Unity.UI_New.InGame.TowerSelectionMenu.TowerSelectionMenuThemes;

namespace AbilityChoice.Patches;

[HarmonyPatch(typeof(TowerSelectionMenu), nameof(TowerSelectionMenu.UpdateTower))]
internal static class TowerSelectionMenu_UpdateTower
{
    [HarmonyPostfix]
    private static void Postfix(TowerSelectionMenu __instance)
    {
        var themeManager = __instance.themeManager;
        var currentTheme = themeManager.CurrentTheme;
        if (ModContent.GetInstance<BloodSacrifice>().Mode == 0) return;

        if (currentTheme == null)
        {
            TaskScheduler.ScheduleTask(() => Postfix(__instance), () => themeManager.CurrentTheme != null);
            return;
        }

        if (!currentTheme.Is(out TSMThemeDefault themeDefault)) return;

        var ui = currentTheme.GetComponentInChildren<AdoraSacrificeUI>();
        if (ui == null)
        {
            ui = AdoraSacrificeUI.Create(__instance, themeDefault);
        }

        ui.UpdateTower();
    }
}