using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppAssets.Scripts.Unity.UI_New.Popups;
using Il2CppAssets.Scripts.Unity.UI_New.Upgrade;
using BTD_Mod_Helper;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AbilityChoice.Patches;

[HarmonyPatch(typeof(Button), nameof(Button.OnPointerClick))]
internal static class Button_OnPointerClick
{
    [HarmonyPostfix]
    private static void Postfix(Button __instance, PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right &&
            __instance.gameObject.GetComponentInParent<UpgradeDetails>().IsType(out UpgradeDetails upgrade) 
            && upgrade.AbilityChoice() is AbilityChoice abilityChoice)
        {
            if (InGame.instance != null)
            {
                PopupScreen.instance.ShowOkPopup("In order for this to take effect, you'll need to exit to the main menu and come back to the game.");
            }
            abilityChoice.Toggle();
            upgrade.OnPointerExit(eventData);
            upgrade.OnPointerEnter(eventData);
        }
    }
}