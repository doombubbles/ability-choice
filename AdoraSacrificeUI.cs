using System;
using BTD_Mod_Helper.Api.Components;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using Il2Cpp;
using Il2CppAssets.Scripts;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Simulation;
using Il2CppAssets.Scripts.Simulation.Towers.Behaviors;
using Il2CppAssets.Scripts.Simulation.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppAssets.Scripts.Unity.UI_New.InGame.TowerSelectionMenu;
using Il2CppAssets.Scripts.Unity.UI_New.InGame.TowerSelectionMenu.TowerSelectionMenuThemes;
using Il2CppSystem.Collections.Generic;
using Il2CppTMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AbilityChoice;

[RegisterTypeInIl2Cpp(false)]
public class AdoraSacrificeUI : MonoBehaviour
{
    private TowerSelectionMenu menu;
    private TSMThemeDefault theme;

    private ModHelperPanel panel;

    public static AdoraSacrificeUI Instance { get; private set; }

    public static int SacrificeAmount { get; private set; }

    private ModHelperSlider slider;

    private ModHelperSlider slider20;

    public void UpdateTower()
    {
        var tower = menu.selectedTower;

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

    public static AdoraSacrificeUI Create(TowerSelectionMenu menu, TSMThemeDefault theme)
    {
        var ui = theme.gameObject.AddComponent<AdoraSacrificeUI>();
        
        var panel = theme.gameObject.AddModHelperPanel(new Info("AdoraSacrificePanel", InfoPreset.FillParent));

        ui.panel = panel;

        ui.panel.AddImage(new Info("Icon", 372, -75, 120), VanillaSprites.BloodSacrificeAA);
        ui.slider = panel.AddSlider(new Info("SacrificeSlider", 0, -75, 550, 75), 0, 0, 150, 5,
            new Vector2(100, 100), new Action<float>(amount => SacrificeAmount = (int) amount), 52f, "$/s");
        ui.slider20 = panel.AddSlider(new Info("SacrificeSliderLvl20", 0, -75, 550, 75), 0, 0, 33, 1,
            new Vector2(100, 100), new Action<float>(amount => SacrificeAmount = (int) amount), 52f, "$/s");

        ui.slider.gameObject.GetComponentInChildrenByName<Image>("Fill").color = new Color(1, 1, 0);
        ui.slider20.gameObject.GetComponentInChildrenByName<Image>("Fill").color = new Color(1, 1, .5f);

        ui.slider.gameObject.GetComponentInChildrenByName<RectTransform>("Label").localPosition =
            new Vector3(0, -80, 0);
        ui.slider20.gameObject.GetComponentInChildrenByName<RectTransform>("Label").localPosition =
            new Vector3(0, -80, 0);

        ui.slider.gameObject.GetComponentInChildrenByName<NK_TextMeshProUGUI>("Label").fontStyle = FontStyles.SmallCaps;
        ui.slider20.gameObject.GetComponentInChildrenByName<NK_TextMeshProUGUI>("Label").fontStyle =
            FontStyles.SmallCaps;

        ui.slider.gameObject.GetComponentInChildrenByName<RectTransform>("DefaultNotch").gameObject.SetActive(false);
        ui.slider20.gameObject.GetComponentInChildrenByName<RectTransform>("DefaultNotch").gameObject.SetActive(false);

        ui.menu = menu;
        ui.theme = theme;

        Instance = ui;

        return ui;
    }

    internal static Dictionary<ObjectId, int> NextSacrificeTimes = new();

    public void Process()
    {
        var bridge = InGame.Bridge;
        var sim = bridge.Simulation;

        var adoras = sim.towerManager.GetTowersByBaseId(TowerType.Adora).ToList()
            .Where(tower => tower.owner == bridge.MyPlayerNumber);

        foreach (var adora in adoras)
        {
            if (!NextSacrificeTimes.TryGetValue(adora.Id, out var nextSacrifice))
            {
                nextSacrifice = 0;
            }
            
            if (sim.roundTime.elapsed < nextSacrifice) continue;
            
            NextSacrificeTimes[adora.Id] = sim.roundTime.elapsed + 60;
            
            var bloodSacrifice = adora.entity.GetBehavior<BloodSacrifice>();

            if (bloodSacrifice == null) continue;

            var model = bloodSacrifice.bloodSacrificeModel;

            adora.entity.GetBehavior<Hero>().AddXp(SacrificeAmount * model.xpMultiplier);

            var factor = model.bonusSacrificeAmount / model.buffDuration;

            var mutator = model.GetMutator((int) Math.Round(SacrificeAmount / factor), "");

            adora.AddMutator(mutator, 66);

            var towersInRange = sim.towerManager.GetTowersInRange(adora.Position, adora.towerModel.range).ToList();

            foreach (var sunGuy in towersInRange.Where(tower =>
                         tower.towerModel.appliedUpgrades.Contains(UpgradeType.SunAvatar)))
            {
                sunGuy.AddMutatorIncludeSubTowers(mutator, 66);
            }

            var amount = Math.Min(SacrificeAmount, bridge.GetCash());
            
            sim.RemoveCash(amount, Simulation.CashType.Normal, adora.owner, Simulation.CashSource.TowerSold);

            // sim.AddBehavior<ImfLoanCollection>(new ImfLoanCollectionModel("BloodSacrifice", .5f, SacrificeAmount));
        }
    }
}