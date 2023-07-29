using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2CppAssets.Scripts.Unity.Menu;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppAssets.Scripts.Unity.UI_New.Popups;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AbilityChoice.Patches;

[HarmonyPatch(typeof(Button), nameof(Button.OnPointerClick))]
internal static class Button_OnPointerClick
{
    [HarmonyPostfix]
    private static void Postfix(Button __instance, PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right) return;

        if (__instance.gameObject.HasComponent(out TowerAbilityChoiceInfo towerInfo))
        {
            towerInfo.abilityChoice.Toggle();
            towerInfo.UpdateIcon();
            towerInfo.upgradeDetails.OnPointerExit(eventData);
            towerInfo.upgradeDetails.OnPointerEnter(eventData);
        }
        else if (__instance.gameObject.HasComponent(out HeroAbilityChoiceInfo heroInfo))
        {
            heroInfo.abilityChoice.Toggle();
            heroInfo.UpdateIcon();
            heroInfo.UpdateDescriptions();
        }
        else return;


        if (InGame.instance != null)
        {
            PopupScreen.instance.ShowOkPopup(
                "In order for this to take effect, you'll need to exit to the main menu and come back to the game.");
        }

        MenuManager.instance.buttonClickSound.Play("ClickSounds");
    }
}