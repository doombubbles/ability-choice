using AbilityChoice.AbilityChoices.Hero.Geraldo;
using HarmonyLib;
using Il2CppAssets.Scripts.Simulation.GeraldoItems;
using Il2CppAssets.Scripts.Simulation.Objects;

namespace AbilityChoice.Patches;

[HarmonyPatch(typeof(BottleHotSauceBehavior), nameof(BottleHotSauceBehavior.IsPositionValid))]
internal static class BottleHotSauceBehavior_IsPositionValid
{
    public static bool CurrentlyChecking { get; private set; }

    [HarmonyPrefix]
    private static void Prefix(BottleHotSauceBehavior __instance)
    {
        CurrentlyChecking = true;
    }

    [HarmonyPostfix]
    private static void Postfix()
    {
        CurrentlyChecking = false;
    }
}

[HarmonyPatch(typeof(Mutable), nameof(Mutable.IsMutatedBy), typeof(string))]
internal static class Mutable_IsMutatedBy
{
    [HarmonyPrefix]
    private static bool Prefix(string id, ref bool __result)
    {
        if (BottleHotSauceBehavior_IsPositionValid.CurrentlyChecking && id == nameof(BottleHotSauce))
        {
            __result = false;
            return false;
        }

        return true;
    }
}