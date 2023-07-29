using System;
using System.Collections.Generic;
using AbilityChoice.AbilityChoices.Support;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2CppAssets.Scripts;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Simulation.Objects;
using Il2CppAssets.Scripts.Simulation.Towers;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.Bridge;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppAssets.Scripts.Utils;
using UnityEngine;

namespace AbilityChoice;

internal static class OverclockHandler
{
    private static int ultraBoostTimer;

    public static readonly Dictionary<Tower, int> UltraBoostFixes = new();

    public static BehaviorMutator GetMutator(TowerModel engineer, int tier, bool ultra)
    {
        var model = engineer.GetAbilities()[0].GetBehavior<OverclockModel>().Duplicate();
        var cooldown = ultra ? .35f : .45f;
        model.rateModifier = cooldown / (cooldown + 2f / 3f * (1.05f - .15f * tier));
        return new OverclockModel.OverclockMutator(model);
    }

    public static void AddBoost(Tower from, Tower to)
    {
        if (AbilityChoiceMod.CurrentBoostIDs.TryGetValue(from.Id, out var id))
        {
            RemoveBoostOn(id);
        }

        to.RemoveMutatorsById("Overclock");
        var tier = to.towerModel.tier;
        if (to.towerModel.IsHero())
        {
            tier = (tier - 1) / 4;
        }

        to.AddMutator(GetMutator(from.towerModel, tier, from.towerModel.tier == 5));

        AbilityChoiceMod.CurrentBoostIDs[from.Id] = to.Id;
    }

    public static void UltraBoostStack(Tower to, int stack = 1)
    {
        var model = Game.instance.model.GetTower(TowerType.EngineerMonkey, 0, 5).GetAbilities()[0]
            .GetBehavior<OverclockPermanentModel>().Duplicate();
        var mutator = to.GetMutatorById("Ultraboost");
        if (mutator != null)
        {
            var stacks = mutator.mutator.Cast<OverclockPermanentModel.OverclockPermanentMutator>().stacks;
            if (stacks < model.maxStacks)
            {
                var newMutator = model.MutatorByStack(Math.Min(stacks + stack, model.maxStacks));
                to.RemoveMutatorsById("Ultraboost");
                to.AddMutator(newMutator);
            }
        }
        else
        {
            var newMutator = model.MutatorByStack(Math.Min(model.maxStacks, stack));
            to.AddMutator(newMutator);
        }
    }

    public static void RemoveBoostOn(ObjectId id)
    {
        var otherTower = InGame.instance.GetTowerManager().GetTowerById(id);
        otherTower?.RemoveMutatorsById("Overclock");
    }

    public static void OnUpdate()
    {
        if (!InGame.instance) return;

        foreach (var tower in UltraBoostFixes.Keys)
        {
            var stacks = UltraBoostFixes[tower];
            tower.RemoveMutatorsById("Ultraboost");
            UltraBoostStack(tower, stacks);
        }

        UltraBoostFixes.Clear();

        if (!TimeManager.inBetweenRounds)
        {
            if (TimeManager.fastForwardActive)
            {
                ultraBoostTimer += (int) TimeManager.networkScale;
            }
            else
            {
                ultraBoostTimer += 1;
            }
        }

        if (ultraBoostTimer >= 45 * 60)
        {
            foreach (var boostingKey in AbilityChoiceMod.CurrentBoostIDs.Keys)
            {
                var engi = InGame.instance.GetTowerManager().GetTowerById(boostingKey);
                if (engi.towerModel.tier != 5) continue;

                var tower = InGame.instance.GetTowerManager()
                    .GetTowerById(AbilityChoiceMod.CurrentBoostIDs[boostingKey]);
                if (tower != null)
                {
                    UltraBoostStack(tower);
                }
            }

            ultraBoostTimer = 0;
        }
    }


    [HarmonyPatch(typeof(InGame), nameof(InGame.SellTower))]
    internal static class InGame_SellTower
    {
        [HarmonyPrefix]
        internal static void Prefix(TowerToSimulation tower)
        {
            if (tower.tower != null && AbilityChoiceMod.CurrentBoostIDs.ContainsKey(tower.tower.Id))
            {
                RemoveBoostOn(AbilityChoiceMod.CurrentBoostIDs[tower.tower.Id]);
                AbilityChoiceMod.CurrentBoostIDs.Remove(tower.tower.Id);
            }
        }
    }

    [HarmonyPatch(typeof(InGame), nameof(InGame.TowerUpgraded))]
    internal static class InGame_TowerUpgraded
    {
        [HarmonyPostfix]
        internal static void Postfix(TowerToSimulation tower)
        {
            if (tower.tower == null) return;

            foreach (var boostingKey in AbilityChoiceMod.CurrentBoostIDs.Keys.Where(boostingKey =>
                         AbilityChoiceMod.CurrentBoostIDs[boostingKey] == tower.Id))
            {
                RemoveBoostOn(tower.Id);
                var engi = InGame.instance.GetTowerManager().GetTowerById(boostingKey);
                AddBoost(engi, tower.tower);
                return;
            }
        }
    }


    [HarmonyPatch(typeof(OverclockInput), nameof(OverclockInput.CursorUp))]
    internal static class OverclockInput_CursorUp
    {
        [HarmonyPostfix]
        internal static void Postfix(OverclockInput __instance, Vector2 cursorPosWorld, bool isCursorInWorld)
        {
            var abilityName = __instance.ability.model.displayName;
            var enabled = abilityName == UpgradeType.Ultraboost && ModContent.GetInstance<Ultraboost>().Enabled ||
                          abilityName == UpgradeType.Overclock && ModContent.GetInstance<Overclock>().Enabled;

            if (!enabled || !isCursorInWorld) return;

            var selected = InGame.instance.bridge.GetSelection(cursorPosWorld, 20);

            if (!selected.Is(out TowerToSimulation tower)) return;

            var engi = __instance.tower.tower;
            AddBoost(engi, tower.tower);

            if (engi.towerModel.tier == 5)
            {
                ultraBoostTimer = 0;
            }
        }
    }

    [HarmonyPatch(typeof(UnityToSimulation), nameof(UnityToSimulation.GetAllAbilities))]
    internal static class UnityToSimulation_GetAllAbilities
    {
        [HarmonyPostfix]
        internal static void Postfix(ref Il2CppSystem.Collections.Generic.List<AbilityToSimulation> __result)
        {
            __result = __result?.Where(a2s => a2s != null &&
                                              !(a2s.model.displayName == "Overclock" &&
                                                ModContent.GetInstance<Overclock>().Enabled ||
                                                a2s.model.displayName == "Ultraboost" &&
                                                ModContent.GetInstance<Ultraboost>().Enabled));
        }
    }
}