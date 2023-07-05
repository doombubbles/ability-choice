using BTD_Mod_Helper.Api.Display;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Utils;

namespace AbilityChoice.Displays;

/// <summary>
/// Just used so that unique data can be stored within <see cref="SelectTargetCIData"/>}
/// </summary>
public class MegaMineInvalid : ModDisplay
{
    public override PrefabReference BaseDisplayReference =>
        Game.instance.model.GetTower(TowerType.WizardMonkey, 1, 2, 0)
            .GetDescendant<TargetSelectedPointModel>().displayInvalid;
}