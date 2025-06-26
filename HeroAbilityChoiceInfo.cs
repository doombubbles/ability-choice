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
    public HeroAbilityChoice abilityChoice;
    public Il2CppReferenceArray<HeroUpgradeButton> buttons;

    public GameObject container;

    public Image abilityIcon;
    public Image background;

    public int index;

    /// <inheritdoc />
    public HeroAbilityChoiceInfo(IntPtr ptr) : base(ptr)
    {
    }

    public void UpdateIcon()
    {
        background.SetSprite(HeroAbilityChoice.IconForMode(abilityChoice?.Mode ?? 0));

        if (abilityChoice != null)
        {
            container.SetActive(true);
            if (abilityChoice.Icon is { } icon)
            {
                abilityIcon.SetSprite(icon);
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
            foreach (var info in button.transform.GetComponentsInChildren<HeroAbilityChoiceInfo>(true))
            {
                info.abilityChoice = null;
                if (info.index > 0)
                {
                    info.container.SetActive(false);
                }
            }
        }

        foreach (var abilityChoice in choices)
        {
            var towerModel = models.First(towerModel => abilityChoice.AppliesTo(towerModel));

            var button = buttons.First(upgradeButton => upgradeButton.HeroIndex == towerModel.tier);

            var info = button.abilityIconContainer.GetComponentOrAdd<HeroAbilityChoiceInfo>();

            if (info.abilityChoice != null)
            {
                var infos = button.transform.GetComponentsInChildren<HeroAbilityChoiceInfo>(true);
                if (!infos.FirstOrDefault(i => i.abilityChoice == null).Is(out info))
                {
                    var index = infos.Length;
                    var container = Instantiate(button.abilityIconContainer,
                        button.abilityIconContainer.transform.parent);
                    container.name = container.name.Replace("(Clone)", index.ToString());
                    info = container.GetComponent<HeroAbilityChoiceInfo>();
                    info.container = container;
                    info.index = index;
                }
            }
            else
            {
                info.container = button.abilityIconContainer;
            }

            info.container.SetActive(true);
            info.abilityChoice = abilityChoice;
            info.buttons = buttons;
            info.abilityIcon = info.gameObject.GetComponentInChildrenByName<Image>("Icon");
            info.background = info.gameObject.GetComponentInChildrenByName<Image>("Bg");
            info.container.GetComponentOrAdd<Text>();
        }


        foreach (var button in buttons)
        {
            foreach (var info in button.transform.GetComponentsInChildren<HeroAbilityChoiceInfo>())
            {
                info.UpdateIcon();
            }
        }
    }
}