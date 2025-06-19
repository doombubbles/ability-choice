using System;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2CppAssets.Scripts;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Simulation.Objects;
using Il2CppAssets.Scripts.Simulation.Towers;
using Il2CppAssets.Scripts.Simulation.Towers.Behaviors.Abilities;
using Il2CppAssets.Scripts.Simulation.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.Bridge;
using Il2CppAssets.Scripts.Unity.UI_New.InGame.TowerSelectionMenu;
using Il2CppSystem.Collections.Generic;
using Il2CppSystem.IO;

namespace AbilityChoice;

internal static class OverclockHandler
{
    public const string Enabled = "AbilityChoiceEnabled";

    private const string OverclockId = "Overclock";

    public static BehaviorMutator GetMutator(TowerModel engineer, int tier)
    {
        var ability = engineer.GetAbilities()[0];
        var overclock = ability.GetBehavior<OverclockModel>().Duplicate();
        var uptime = overclock.tierBasedDurationMultiplier[tier] * overclock.lifespanFrames / ability.cooldownFrames;

        overclock.rateModifier = 1 / AbilityChoice.CalcAvgBonus(uptime, 1 / overclock.rateModifier);
        overclock.villageRangeModifier = AbilityChoice.CalcAvgBonus(uptime, overclock.villageRangeModifier);

        return new OverclockModel.OverclockMutator(overclock);
    }

    public static void ApplyOverclock(Tower from, Tower to)
    {
        if (from.Sim.factory.GetUncast<Overclock>().ToArray().Count(o => o.selectedTowerId == to.Id) <= 1)
        {
            to.RemoveMutatorsById(OverclockId);
        }

        var tier = to.towerModel.tier;
        if (to.towerModel.IsHero())
        {
            tier = (tier - 1) / 4;
        }

        to.AddMutator(GetMutator(from.towerModel, tier));
    }

    private static bool OverclockAbilityChoice(this Overclock overclock) =>
        overclock.overclockModel.OverclockAbilityChoice();

    private static bool OverclockAbilityChoice(this AbilityModel abilityModel) =>
        abilityModel.HasDescendant(out OverclockModel overclockModel) && overclockModel.OverclockAbilityChoice();

    private static bool OverclockAbilityChoice(this OverclockModel overclockModel) =>
        overclockModel.name.Contains(Enabled);


    internal static readonly Dictionary<ObjectId, List<Entity>> Dots = new();

    private static TechBotLink GetFakeTechBotLink(Ability __instance)
    {
        var oc = __instance.entity.GetBehaviorInDependants<Overclock>();

        if (!Dots.TryGetValue(__instance.tower.Id, out var dots))
        {
            dots = Dots[__instance.tower.Id] = new List<Entity>();
        }

        var model = Game.instance.model.GetPowerWithName(TowerType.TechBot).tower
            .GetDescendant<TechBotLinkModel>();

        return new TechBotLink
        {
            ability = __instance,
            drawDots = true,
            entity = __instance.entity,
            Sim = __instance.Sim,
            selectedTowerId = oc.selectedTowerId,
            linkedTower = oc.selectedTower,
            model = model,
            techBotLinkModel = model,
            lineDotDisplays = dots
        };
    }

    /// <summary>
    /// Override Overclock application
    /// </summary>
    [HarmonyPatch(typeof(Overclock), nameof(Overclock.Activate))]
    internal static class Overclock_Activate
    {
        [HarmonyPostfix]
        internal static void Postfix(Overclock __instance)
        {
            if (!__instance.OverclockAbilityChoice() ||
                __instance.selectedTower == null ||
                __instance.IsBanned(__instance.selectedTower)) return;

            ApplyOverclock(__instance.ability.tower, __instance.selectedTower);
        }
    }

    /// <summary>
    /// Ultraboost auto stacking, link display
    /// </summary>
    [HarmonyPatch(typeof(Ability), nameof(Ability.Process))]
    internal static class Ability_Process
    {
        [HarmonyPostfix]
        internal static void Postfix(Ability __instance)
        {
            if (__instance.CooldownRemaining == 0 &&
                __instance.abilityModel.OverclockAbilityChoice() &&
                __instance.abilityModel.GetBehavior<OverclockPermanentModel>().Is(out var overclockPermanentModel) &&
                __instance.entity.GetBehaviorInDependants<Overclock>().Is(out var overclock) &&
                overclock.selectedTower != null &&
                !(overclock.selectedTower.GetMutatorById(OverclockPermanentModel.MutatorId).Is(out var timedMutator) &&
                  timedMutator.mutator.Is(out OverclockPermanentModel.OverclockPermanentMutator ultraBoost) &&
                  ultraBoost.stacks >= overclockPermanentModel.maxStacks))
            {
                __instance.Activate();
            }

            if (__instance.abilityModel.OverclockAbilityChoice() &&
                __instance.entity.GetBehaviorInDependants<Overclock>().Is(out var oc))
            {
                var fakeTechBotLink = GetFakeTechBotLink(__instance);

                if (oc.selectedTower != null && TowerSelectionMenu.instance.selectedTower?.Id == __instance.tower.Id)
                {
                    fakeTechBotLink.PlotPointsToLinkedTower();
                }
                else
                {
                    fakeTechBotLink.RemoveDots();
                }
            }
        }
    }

    /// <summary>
    /// Ensure link display removed
    /// </summary>
    [HarmonyPatch(typeof(Ability), nameof(Ability.OnDestroy))]
    internal static class Ability_OnDestroy
    {
        [HarmonyPrefix]
        internal static void Prefix(Ability __instance)
        {
            if (!__instance.abilityModel.OverclockAbilityChoice()) return;

            var fakeTechBotLink = GetFakeTechBotLink(__instance);
            fakeTechBotLink.RemoveDots();
        }
    }

    /// <summary>
    /// Ensure don't Ultraboost too fast
    /// </summary>
    [HarmonyPatch(typeof(OverclockPermanent), nameof(OverclockPermanent.ApplyToTower))]
    internal static class OverclockPermanent_ApplyToTower
    {
        [HarmonyPrefix]
        internal static bool Prefix(OverclockPermanent __instance)
        {
            if (!__instance.ability.abilityModel.OverclockAbilityChoice()) return true;

            return __instance.ability.CooldownRemaining == 0;
        }
    }

    /// <summary>
    /// For clarity, don't allow re-overclocking the same tower
    /// </summary>
    [HarmonyPatch(typeof(TapTowerAbilityBehavior), nameof(TapTowerAbilityBehavior.GetCustomInputData))]
    internal static class TapTowerAbilityBehavior_GetCustomInputData
    {
        [HarmonyPostfix]
        internal static void Postfix(TapTowerAbilityBehavior __instance, ref Il2CppSystem.Object __result)
        {
            if (!__instance.Is(out Overclock overclock) || !overclock.OverclockAbilityChoice() || !__result.Is(out OverclockCIData data)) return;

            data.validTowerIds.RemoveAll(new Func<ObjectId, bool>(id => id == overclock.selectedTowerId));
        }
    }

    /// <summary>
    /// Reapply when a tower gets upgraded because the tier may change the mutator
    /// </summary>
    [HarmonyPatch(typeof(TapTowerAbilityBehavior), nameof(TapTowerAbilityBehavior.OnTowerUpgraded))]
    internal static class TapTowerAbilityBehavior_OnTowerUpgraded
    {
        [HarmonyPostfix]
        internal static void Postfix(TapTowerAbilityBehavior __instance, Tower tower)
        {
            if (!__instance.Is(out Overclock overclock) || !overclock.OverclockAbilityChoice()) return;

            if (tower.Id == overclock.selectedTowerId && !__instance.IsBanned(tower))
            {
                ApplyOverclock(__instance.ability.tower, tower);
            }

            if (tower.Id == __instance.ability.tower.Id &&
                __instance.selectedTower != null &&
                __instance.ability.entity.GetBehavior<OverclockPermanent>() != null &&
                __instance.selectedTower.GetMutatorById(OverclockPermanentModel.MutatorId) == null)
            {
                __instance.ability.ClearCooldown();
                __instance.ability.Activate();
            }
        }
    }

    /// <summary>
    /// Remove mutators from selected tower when destroyed
    /// </summary>
    [HarmonyPatch(typeof(Overclock), nameof(Overclock.OnDestroy))]
    internal static class Overclock_OnDestroy
    {
        [HarmonyPrefix]
        internal static void Prefix(Overclock __instance)
        {
            if (!__instance.OverclockAbilityChoice() || __instance.selectedTower == null) return;

            __instance.selectedTower.RemoveMutatorsById(OverclockId);
        }
    }

    /// <summary>
    /// Remove from original tower when a new tower is boosted
    /// </summary>
    [HarmonyPatch(typeof(Overclock), nameof(Overclock.ApplyCustomInputData))]
    internal static class Overclock_ApplyCustomInputData
    {
        [HarmonyPrefix]
        internal static void Prefix(Overclock __instance,
            Il2CppAssets.Scripts.Simulation.Towers.Behaviors.Abilities.CustomInputData data)
        {
            if (__instance.selectedTower != null && __instance.selectedTowerId != data.objectIdValue)
            {
                __instance.selectedTower.RemoveMutatorsById(OverclockId);
            }
        }
    }

    /// <summary>
    /// Can switch Overclocks without cooldown
    /// </summary>
    [HarmonyPatch(typeof(Ability), nameof(Ability.IsReady))]
    internal static class Ability_IsReady
    {
        [HarmonyPrefix]
        internal static void Prefix(Ability __instance, ref bool ignoreCooldown)
        {
            if (!__instance.abilityModel.OverclockAbilityChoice()) return;

            ignoreCooldown = true;
        }
    }

    /// <summary>
    /// Can switch Overclocks without cooldown
    /// </summary>
    [HarmonyPatch(typeof(Ability), nameof(Ability.Activate))]
    internal static class Ability_Activate
    {
        [HarmonyPrefix]
        internal static void Prefix(Ability __instance, ref bool ignoreCooldown)
        {
            if (!__instance.abilityModel.OverclockAbilityChoice()) return;

            ignoreCooldown = true;
        }
    }

    /// <summary>
    /// Hide abilities from bar
    /// </summary>
    [HarmonyPatch(typeof(UnityToSimulation), nameof(UnityToSimulation.GetAllAbilities))]
    internal static class UnityToSimulation_GetAllAbilities
    {
        [HarmonyPostfix]
        internal static void Postfix(ref Il2CppSystem.Collections.Generic.List<AbilityToSimulation> __result)
        {
            __result = __result?.Where(a => a?.model.OverclockAbilityChoice() != true);
        }
    }
}