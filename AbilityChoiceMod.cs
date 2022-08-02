using System.Collections.Generic;
using AbilityChoice;
using Assets.Scripts.Unity.UI_New.InGame;
using BTD_Mod_Helper;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.ModOptions;
using MelonLoader;

[assembly: MelonInfo(typeof(AbilityChoiceMod), ModHelperData.Name, ModHelperData.Version, ModHelperData.RepoOwner)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace AbilityChoice;

public class AbilityChoiceMod : BloonsTD6Mod
{
    private static readonly ModSettingButton SetAllToOff = new()
    {
        action = () => ModContent.GetContent<AbilityChoice>().ForEach(choice => choice.SetMode(0)),
        icon = AbilityChoice.IconForMode(0),
        description = "Sets all abilities back to their default vanilla affects.",
        buttonSprite = VanillaSprites.YellowBtnLong,
        buttonText = "Set All"
    };
    
    private static readonly ModSettingButton SetAllToMode1 = new()
    {
        action = () => ModContent.GetContent<AbilityChoice>().ForEach(choice => choice.SetMode(1)),
        icon = AbilityChoice.IconForMode(1),
        description = "For most towers, this is a permanent but weaker version of the ability.",
        buttonSprite = VanillaSprites.RedBtnLong,
        buttonText = "Set All"
    };
    
    private static readonly ModSettingButton SetAllToMode2 = new()
    {
        action = () => ModContent.GetContent<AbilityChoice>().ForEach(choice => choice.SetMode(2)),
        icon = AbilityChoice.IconForMode(2),
        description = "For the towers that have it, a different alternate affect to the ability. Towers that don't" +
                      "have a second mode will be set to Mode 1.",
        buttonSprite = VanillaSprites.BlueBtnLong,
        buttonText = "Set All"
    };
    
#if DEBUG
    public static readonly ModSettingButton CreateMds = new(GenerateReadme.Generate);
#endif

    public static Dictionary<int, int> CurrentBoostIDs = new();

    public override void OnMainMenu()
    {
        ResetCaches();
    }

    public override void OnRestart()
    {
        ResetCaches();
    }

    public override void OnUpdate()
    {
        OverclockHandler.OnUpdate();
    }

    public void ResetCaches()
    {
        if (InGame.instance == null || !InGame.instance.quitting)
        {
            CurrentBoostIDs = new Dictionary<int, int>();
        }
    }

    public static MelonPreferences_Category AbilityChoiceSettings { get; private set; }

    public override void OnApplicationStart()
    {
        AbilityChoiceSettings = MelonPreferences.CreateCategory("AbilityChoiceSettings");
    }
}