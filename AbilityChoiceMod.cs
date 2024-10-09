using System;
using AbilityChoice.AbilityChoices.Hero.Corvus;
using AbilityChoice.Patches;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.ModOptions;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Profile;
using Il2CppAssets.Scripts.Simulation.Bloons;
using Il2CppAssets.Scripts.Simulation.Corvus.TowerManager;
using Il2CppAssets.Scripts.Simulation.Towers;
using Il2CppAssets.Scripts.Unity.Menu;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppAssets.Scripts.Unity.UI_New.Popups;
using UnityEngine;
using UnityEngine.EventSystems;

[assembly: MelonInfo(typeof(AbilityChoiceMod), ModHelperData.Name, ModHelperData.Version, ModHelperData.RepoOwner)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace AbilityChoice;

public class AbilityChoiceMod : BloonsTD6Mod
{
    private static readonly ModSettingButton SetAllToOff = new()
    {
        action = () => ModContent.GetContent<TowerAbilityChoice>().ForEach(choice => choice.Mode = 0),
        icon = TowerAbilityChoice.IconForMode(0),
        description = "Sets all abilities back to their default vanilla affects.",
        buttonSprite = VanillaSprites.YellowBtnLong,
        buttonText = "Set All"
    };

    private static readonly ModSettingButton SetAllToMode1 = new()
    {
        action = () => ModContent.GetContent<TowerAbilityChoice>().ForEach(choice => choice.Mode = 1),
        icon = TowerAbilityChoice.IconForMode(1),
        description = "For most towers, this is a permanent but weaker version of the ability.",
        buttonSprite = VanillaSprites.RedBtnLong,
        buttonText = "Set All"
    };

    private static readonly ModSettingButton SetAllToMode2 = new()
    {
        action = () => ModContent.GetContent<TowerAbilityChoice>().ForEach(choice => choice.Mode = 2),
        icon = TowerAbilityChoice.IconForMode(2),
        description = "For the towers that have it, a different alternate affect to the ability. Towers that don't" +
                      "have a second mode will be set to Mode 1.",
        buttonSprite = VanillaSprites.BlueBtnLong,
        buttonText = "Set All"
    };

    public static readonly ModSettingBool MoreBalanced = new(false)
    {
        description =
            "While none of the effects is meant to be completely imbalanced, this settings makes things err " +
            "more on the cautious side, at the risk of the effects not being as exciting to use."
    };

#if DEBUG
    public static readonly ModSettingButton CreateMds = new(GenerateReadme.Generate);
#endif

    public static MelonPreferences_Category AbilityChoiceSettings { get; private set; }

    public override void OnMainMenu()
    {
        AbilityChoiceSettings.SaveToFile(false);
    }

    public override void OnFixedUpdate()
    {
        if (InGame.instance != null && InGame.Bridge != null && AdoraSacrificeUI.Instance != null)
        {
            AdoraSacrificeUI.Instance.Process();
        }
    }

    public override void PreCleanProfile(ProfileModel profile)
    {
        base.PreCleanProfile(profile);
    }

    public override void OnUpdate()
    {
        if (Input.GetMouseButtonUp((int) PointerEventData.InputButton.Right))
        {
            var raycastResults = new Il2CppSystem.Collections.Generic.List<RaycastResult>();
            var data = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };
            EventSystem.current.RaycastAll(data, raycastResults);

            foreach (var raycastResult in raycastResults)
            {
                if (raycastResult.gameObject.HasComponent(out TowerAbilityChoiceInfo towerInfo))
                {
                    towerInfo.abilityChoice.Toggle();
                    towerInfo.UpdateIcon();
                    towerInfo.upgradeDetails.OnPointerExit(data);
                    towerInfo.upgradeDetails.OnPointerEnter(data);
                }
                else if (raycastResult.gameObject.HasComponent(out HeroAbilityChoiceInfo heroInfo))
                {
                    heroInfo.abilityChoice.Toggle();
                    heroInfo.UpdateIcon();
                    heroInfo.UpdateDescriptions();
                }
                else continue;

                if (InGame.instance != null)
                {
                    PopupScreen.instance.ShowOkPopup(
                        "In order for this to take effect, you'll need to exit to the main menu and come back to the game.");
                }
                else
                {
                    MenuManager.instance.buttonClickSound.Play("ClickSounds");
                }
            }
        }
    }

    public override void OnApplicationStart()
    {
        AbilityChoiceSettings = MelonPreferences.CreateCategory("AbilityChoiceSettings");
    }

    public override void OnNewGameModel(GameModel gameModel)
    {
        foreach (var abilityChoice in ModContent.GetContent<AbilityChoice>().Where(choice => choice.Enabled))
        {
            try
            {
                abilityChoice.Apply(gameModel);
            }
            catch (Exception e)
            {
                ModHelper.Error<AbilityChoiceMod>(e);
            }
        }
    }

    public override void OnRoundStart()
    {
        Syphon_OnBloonCreate.counter = 0;
    }

    public override void OnGameObjectsReset()
    {
        OverclockHandler.Dots.Clear();
        AdoraSacrificeUI.NextSacrificeTimes.Clear();
        CorvusHandler.SpellsToReactivate.Clear();
    }

    public override bool PreBloonLeaked(Bloon bloon)
    {
        if (bloon.Sim.GetCorvusManagerExists(InGame.Bridge.MyPlayerNumber))
        {
            var corvus = bloon.Sim.GetCorvusManager(InGame.Bridge.MyPlayerNumber);

            if (corvus.CanSpellBeCast(CorvusSpellType.SoulBarrier) && CorvusAbilityChoice.EnabledForSpell(CorvusSpellType.SoulBarrier))
            {
                corvus.CastSpell(CorvusSpellType.SoulBarrier);
            }
        }

        return true;
    }

    public override void OnTowerSaved(Tower tower, TowerSaveDataModel saveData)
    {
        if (AdoraSacrificeUI.NextSacrificeTimes.TryGetValue(tower.Id, out var time))
        {
            saveData.metaData["AbilityChoice-NextSacrificeTime"] = time.ToString();
        }
    }

    public override void OnTowerLoaded(Tower tower, TowerSaveDataModel saveData)
    {
        if (saveData.metaData.TryGetValue("AbilityChoice-NextSacrificeTime", out var time))
        {
            AdoraSacrificeUI.NextSacrificeTimes[tower.Id] = int.Parse(time);
        }
    }
}