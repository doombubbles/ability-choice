using System;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.UI_New.Main.HeroSelect;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppNinjaKiwi.Common;
using UnityEngine;
using UnityEngine.UI;

namespace AbilityChoice;

[RegisterTypeInIl2Cpp(false)]
public class HeroAbilityChoiceInfo : MonoBehaviour
{
    public HeroUpgradeButton button;

    public HeroAbilityChoice abilityChoice;
    public Il2CppReferenceArray<HeroUpgradeButton> buttons;

    /// <inheritdoc />
    public HeroAbilityChoiceInfo(IntPtr ptr) : base(ptr)
    {
    }

    public void UpdateIcon()
    {
        var background = button.abilityIconContainer.GetComponentInChildrenByName<Image>("Bg");
        background.SetSprite(HeroAbilityChoice.IconForMode(abilityChoice?.Mode ?? 0));

        if (abilityChoice != null)
        {
            button.abilityIconContainer.SetActive(true);
            if (abilityChoice is GeraldoAbilityChioce geraldoAbilityChioce)
            {
                button.abillityIcon.SetSprite(geraldoAbilityChioce.GeraldoItem().defaultIcon);
            }
        }
    }

    public void UpdateDescriptions()
    {
        foreach (var upgradeButton in buttons)
        {
            UpdateDescription(abilityChoice.HeroId, upgradeButton);
        }
    }

    public static void UpdateDescription(string heroId, HeroUpgradeButton button)
    {
        var key = $"{heroId} Level {button.HeroIndex} Description";
        button.descriptions.SetText(LocalizationManager.Instance.GetText(key));
    }

    public static void Setup(string selectedHero, Il2CppReferenceArray<HeroUpgradeButton> buttons)
    {
        var models = Game.instance.model.GetTowersWithBaseId(selectedHero);

        var choices = ModContent.GetContent<HeroAbilityChoice>()
            .Where(choice => choice.HeroId == selectedHero)
            .ToList();

        foreach (var button in buttons)
        {
            if (button.gameObject.HasComponent(out HeroAbilityChoiceInfo info))
            {
                info.abilityChoice = null;
            }
        }

        foreach (var abilityChoice in choices)
        {
            var towerModel = models.First(towerModel => abilityChoice.AppliesTo(towerModel));

            var button = buttons.First(upgradeButton => upgradeButton.HeroIndex == towerModel.tier);

            var info = button.GetComponent<HeroAbilityChoiceInfo>() ??
                       button.gameObject.AddComponent<HeroAbilityChoiceInfo>();

            info.abilityChoice = abilityChoice;
            info.button = button;
            info.buttons = buttons;
        }


        foreach (var button in buttons)
        {
            if (button.gameObject.HasComponent(out HeroAbilityChoiceInfo info))
            {
                info.UpdateIcon();
            }
        }
    }
}