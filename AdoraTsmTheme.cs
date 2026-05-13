using System;
using BTD_Mod_Helper.Api.Components;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.Towers;
using BTD_Mod_Helper.Extensions;
using Il2Cpp;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Unity.Bridge;
using Il2CppAssets.Scripts.Unity.UI_New.InGame.TowerSelectionMenu.TowerSelectionMenuThemes;
using Il2CppTMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AbilityChoice;

public class AdoraTsmTheme : ModTsmTheme
{
    public override string BaseTheme => "Default";

    public static int SacrificeAmount { get; private set; }

    private ModHelperPanel panel;
    private ModHelperSlider slider;
    private ModHelperSlider slider20;

    public override void SetupTheme(BaseTSMTheme theme)
    {
        panel = theme.gameObject.AddModHelperPanel(new Info("AdoraSacrificePanel", InfoPreset.FillParent));

        panel.AddImage(new Info("Icon", 372, -75, 120), VanillaSprites.BloodSacrificeAA);
        slider = panel.AddSlider(new Info("SacrificeSlider", 0, -75, 550, 75), 0, 0, 150, 5,
            new Vector2(100, 100), new Action<float>(amount => SacrificeAmount = (int) amount), 52f, "$/s");
        slider20 = panel.AddSlider(new Info("SacrificeSliderLvl20", 0, -75, 550, 75), 0, 0, 33, 1,
            new Vector2(100, 100), new Action<float>(amount => SacrificeAmount = (int) amount), 52f, "$/s");

        slider.gameObject.GetComponentInChildrenByName<Image>("Fill").color = new Color(1, 1, 0);
        slider20.gameObject.GetComponentInChildrenByName<Image>("Fill").color = new Color(1, 1, .5f);

        slider.gameObject.GetComponentInChildrenByName<RectTransform>("Label").localPosition =
            new Vector3(0, -80, 0);
        slider20.gameObject.GetComponentInChildrenByName<RectTransform>("Label").localPosition =
            new Vector3(0, -80, 0);

        slider.gameObject.GetComponentInChildrenByName<NK_TextMeshProUGUI>("Label").fontStyle = FontStyles.SmallCaps;
        slider20.gameObject.GetComponentInChildrenByName<NK_TextMeshProUGUI>("Label").fontStyle =
            FontStyles.SmallCaps;

        slider.gameObject.GetComponentInChildrenByName<RectTransform>("DefaultNotch").gameObject.SetActive(false);
        slider20.gameObject.GetComponentInChildrenByName<RectTransform>("DefaultNotch").gameObject.SetActive(false);
    }

    public override void TowerChanged(BaseTSMTheme theme, TowerToSimulation tower)
    {
        if (tower == null || tower.Def.baseId != TowerType.Adora || tower.Def.tier < 7)
        {
            panel.SetActive(false);
            return;
        }

        panel.SetActive(true);

        if (tower.Def.tier < 20)
        {
            slider.SetActive(true);
            slider20.SetActive(false);

            slider.SetCurrentValue(Math.Clamp(SacrificeAmount, 0, 150));
        }
        else
        {
            slider20.SetActive(true);
            slider.SetActive(false);

            slider20.SetCurrentValue(Math.Clamp(SacrificeAmount, 0, 33));

            slider20.gameObject.GetComponentInChildrenByName<Image>("Fill").color =
                tower.tower.GetMutatorById("AdoraSunGodTransformationVengeance") != null
                    ? new Color(.75f, 0, 0)
                    : new Color(1, 1, .5f);
        }
    }
}