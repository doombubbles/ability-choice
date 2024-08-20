using System;
using AbilityChoice.Patches;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.ModOptions;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;

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
        if (InGame.instance != null && AdoraSacrificeUI.Instance != null)
        {
            AdoraSacrificeUI.Instance.Process();
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
}