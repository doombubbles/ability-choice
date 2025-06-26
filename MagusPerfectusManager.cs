using System.Collections.Generic;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2CppAssets.Scripts.Simulation.Towers.Behaviors;
using Il2CppAssets.Scripts.Simulation.Towers.Behaviors.Abilities.Behaviors;
using static Il2CppAssets.Scripts.Simulation.Towers.Behaviors.MagusPerfectusGraveyardStateManager;
namespace AbilityChoice;

internal static class MagusPerfectusManager
{
    private static bool activatingPhoenixRebirth;

    /// <summary>
    /// Consume only a portion of mana when exploding Phoneix
    /// </summary>
    [HarmonyPatch(typeof(MagusPerfectusGraveyardStateManager),
        nameof(MagusPerfectusGraveyardStateManager.TriggerManaExplosion))]
    internal static class MagusPerfectusGraveyardStateManager_TriggerManaExplosion
    {
        [HarmonyPrefix]
        internal static bool Prefix(MagusPerfectusGraveyardStateManager __instance, ref int __result)
        {
            if (activatingPhoenixRebirth)
            {
                __instance.explosionRemainingFrames = (int) (7.5 * 60);

                if (__instance.ParagonNecroData is { } necroData)
                {
                    __result = necroData.RbePool() / AbilityChoices.Magic.WizardMonkey.ArcaneMetamorphosis.Factor;

                    if (necroData.RbePool() - __result < 1000)
                    {
                        __result = necroData.RbePool();
                    }

                    __instance.ConsumeMana(__result, false);
                }

                return false;
            }

            return true;
        }
    }

    /// <summary>
    /// Track when doing an Ability Choice phoenix explosion
    /// </summary>
    [HarmonyPatch(typeof(PhoenixRebirth), nameof(PhoenixRebirth.Activate))]
    internal static class PhoenixRebirth_Activate
    {
        [HarmonyPrefix]
        internal static void Prefix(PhoenixRebirth __instance, ref int __state)
        {
            activatingPhoenixRebirth =
                __instance.ability.abilityModel.displayName.Contains(AbilityChoiceMod.DontShowAbilityKeyword);
        }

        [HarmonyPostfix]
        internal static void Postfix(PhoenixRebirth __instance)
        {
            activatingPhoenixRebirth = false;
        }
    }

    private static bool spawningBfbs;

    /// <summary>
    /// Use excess mana to summon BFBs
    /// </summary>
    [HarmonyPatch(typeof(PhoenixRebirth), nameof(PhoenixRebirth.SpawnZombieZOMGs))]
    internal static class PhoenixRebirth_SpawnZombieZOMGs
    {
        [HarmonyPostfix]
        internal static void Postfix(PhoenixRebirth __instance, int manaConsumed)
        {
            if (spawningBfbs) return;
            spawningBfbs = true;

            var leftover = manaConsumed % __instance.behaviourModel.manaPerZombieZOMG;

            var zomg = __instance.behaviourModel.projectileZOMG;

            __instance.behaviourModel.projectileZOMG = __instance.behaviourModel.projectileBFB;
            __instance.behaviourModel.manaPerZombieZOMG /= AbilityChoices.Magic.WizardMonkey.ArcaneMetamorphosis.Factor;

            __instance.SpawnZombieZOMGs(leftover);

            __instance.behaviourModel.projectileZOMG = zomg;
            __instance.behaviourModel.manaPerZombieZOMG *= AbilityChoices.Magic.WizardMonkey.ArcaneMetamorphosis.Factor;

            spawningBfbs = false;
        }
    }

    /// <summary>
    /// Dont use while generating mana
    /// </summary>
    [HarmonyPatch(typeof(PhoenixRebirth), nameof(PhoenixRebirth.CanUseAbility))]
    internal static class PhoenixRebirth_CanUseAbility
    {
        [HarmonyPrefix]
        internal static bool Prefix(PhoenixRebirth __instance, ref bool __result)
        {
            if (__instance.ability.abilityModel.displayName.Contains(AbilityChoiceMod.DontShowAbilityKeyword) &&
                __instance.GraveyardManager.graveyardManaState == GraveyardManaState.Generating)
            {
                __result = false;
                return false;
            }

            return true;
        }
    }


    private static readonly Dictionary<int, int> PreviousMana = new();

    /// <summary>
    /// Implement custom mana threshold
    /// </summary>
    [HarmonyPatch(typeof(MagusPerfectusGraveyardStateManager), nameof(MagusPerfectusGraveyardStateManager.Process))]
    internal static class MagusPerfectusGraveyardStateManager_Process
    {
        [HarmonyPostfix]
        internal static void Postfix(MagusPerfectusGraveyardStateManager __instance)
        {
            if (__instance.ParagonNecroData is not { } necroData) return;

            var mana = necroData.RbePool();

            if (PreviousMana.TryGetValue(__instance.registeredNecroIdx, out var prevMana) &&
                prevMana > AbilityChoiceMod.MagusPerfectusSwitchThreshold &&
                mana <= AbilityChoiceMod.MagusPerfectusSwitchThreshold)
            {
                __instance.SwitchGraveyardManaState(GraveyardManaState.Generating);
            }

            PreviousMana[__instance.registeredNecroIdx] = mana;
        }
    }

    /// <summary>
    /// Drain mana for Arcane Metamorphosis
    /// </summary>
    [HarmonyPatch(typeof(ArcaneMetamorphosis), nameof(ArcaneMetamorphosis.Process))]
    internal static class ArcaneMeta_Process
    {
        [HarmonyPrefix]
        internal static void Prefix(ArcaneMetamorphosis __instance)
        {
            if (ModContent.GetInstance<AbilityChoices.Magic.WizardMonkey.ArcaneMetamorphosis>().Mode == 1)
            {
                __instance.isCurrentlyMorphed =
                    __instance.GraveyardManager.graveyardManaState == GraveyardManaState.Consuming &&
                    __instance.GraveyardManager.graveyardStateAttacks.GetValues().ToArray()
                        .SelectMany(list => list.ToArray()).Any(attack => attack.HasValidTarget());
            }
        }
    }

}