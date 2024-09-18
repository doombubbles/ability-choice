using AbilityChoice.AbilityChoices.Hero;
using AbilityChoice.AbilityChoices.Hero.Geraldo;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Simulation.GeraldoItems;
using Il2CppAssets.Scripts.Simulation.SMath;
using Il2CppAssets.Scripts.Simulation.Towers;

namespace AbilityChoice.Patches;

[HarmonyPatch(typeof(BottleHotSauceBehavior), nameof(BottleHotSauceBehavior.Activate))]
internal static class BottleHotSauceBehavior_Activate
{
    [HarmonyPrefix]
    private static bool Prefix(BottleHotSauceBehavior __instance, Vector2 location, int inputId)
    {
        var selectableObject = __instance.sim.Map.GetSelection(location, 20);

        if (selectableObject.Is(out Tower tower) && tower.IsMutatedBy(nameof(BottleHotSauce)))
        {
            tower.RemoveMutatorsById(nameof(BottleHotSauce));

            __instance.sim.towerManager.GetChildTowers(tower).ForEach(t =>
            {
                if (t.towerModel.baseId == TowerType.HotSauceCreature)
                {
                    __instance.sim.towerManager.DestroyTower(t, inputId);
                }
            });
        }

        return true;
    }
}