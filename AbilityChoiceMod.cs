using System.Collections.Generic;
using AbilityChoice;
using Assets.Scripts.Unity.UI_New.InGame;
using BTD_Mod_Helper;
using MelonLoader;

[assembly: MelonInfo(typeof(AbilityChoiceMod), ModHelperData.Name, ModHelperData.Version, ModHelperData.RepoOwner)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace AbilityChoice;

public class AbilityChoiceMod : BloonsTD6Mod
{
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