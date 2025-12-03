using AbilityChoice.AbilityChoices.Hero.AdmiralBrickell;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppAssets.Scripts.Unity.UI_New.InGame.TowerSelectionMenu;
using Il2CppAssets.Scripts.Unity.UI_New.InGame.TowerSelectionMenu.TowerSelectionMenuThemes;

namespace AbilityChoice.Patches;

[HarmonyPatch(typeof(TowerSelectionMenu), nameof(TowerSelectionMenu.UpdateTower))]
internal static class TowerSelectionMenu_UpdateTower
{
    [HarmonyPostfix]
    private static void Postfix(TowerSelectionMenu __instance)
    {
        if (ModContent.GetInstance<BloodSacrifice>().Mode == 0) return;

        var themeManager = __instance.themeManager;
        var currentTheme = themeManager.CurrentTheme;

        if (currentTheme == null) return;

        var ui = currentTheme.GetComponent<AdoraSacrificeUI>();
        if (ui != null)
        {
            ui.UpdateTower();
        }
    }
}

[HarmonyPatch(typeof(MenuThemeManager), nameof(MenuThemeManager.SetTheme))]
internal static class MenuThemeManager_SetTheme
{
    [HarmonyPostfix]
    private static void Postfix(MenuThemeManager __instance, BaseTSMTheme newTheme)
    {
        if (ModContent.GetInstance<BloodSacrifice>().Mode == 0 ||
            !__instance.PlayerContext.towerSelectionMenu.Is(out var menu) ||
            !newTheme.Is(out TSMThemeDefault theme)) return;

        var ui = theme.GetComponent<AdoraSacrificeUI>();
        if (ui == null)
        {
            ui = AdoraSacrificeUI.Create(menu, theme);
            ui.UpdateTower();
        }
    }
}