using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;

namespace AbilityChoice.Patches;

[HarmonyPatch(typeof(AdoraSunGodTransformationModel.DarkMutator),
    nameof(AdoraSunGodTransformationModel.DarkMutator.Mutate))]
internal static class AdoraSunGodTransformationModel_DarkMutator_Mutate
{
    [HarmonyPostfix]
    private static void Postfix(Model model)
    {
        var newBall = model.GetDescendant<AbilityCreateTowerModel>().towerModel;
        model.GetDescendants<TowerCreateTowerModel>().ForEach(createTower =>
        {
            createTower.towerModel = newBall;
            createTower.name += "Dark";
        });
    }
}