using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Simulation.SMath;
using Il2CppAssets.Scripts.Simulation.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Unity;
using Il2CppSystem.IO;

namespace AbilityChoice.AbilityChoices.Primary.DartMonkey;

public class SuperMonkeyFanClub : TowerAbilityChoice
{
    public override string UpgradeId => UpgradeType.SuperMonkeyFanClub;

    public override string Description1 => "3+ nearby Dart Monkeys including itself are permanently Super Monkey fans.";

    public override string Description2 => "Gains permanent Super attack speed and range itself.";

    protected virtual float SuperMonkeyAttackSpeed => .06f;

    private const int Factor = 10;

    private MonkeyFanClubModel baseModel;
    public MonkeyFanClubModel BaseModel =>
        baseModel ??= AbilityModel(GetAffected(Game.instance.model).MinBy(model => model.tiers.Sum()))
            .GetBehavior<MonkeyFanClubModel>();

    public override void Apply1(TowerModel model)
    {
        var abilityModel = AbilityModel(model);

        var monkeyFanClubModel = abilityModel.GetBehavior<MonkeyFanClubModel>();

        abilityModel.Cooldown /= Factor;

        ModifyModel(monkeyFanClubModel, abilityModel);

        monkeyFanClubModel.effectOnOtherId = CreatePrefabReference("");
        monkeyFanClubModel.effectModel.assetId = CreatePrefabReference("");
        monkeyFanClubModel.otherDisplayModel.display = CreatePrefabReference("");
        monkeyFanClubModel.endDisplayModel.effectModel.assetId = CreatePrefabReference("");

        abilityModel.RemoveBehavior<CreateEffectOnAbilityModel>();
        abilityModel.RemoveBehavior<CreateSoundOnAbilityModel>();

    }

    public override void Apply2(TowerModel model)
    {
        var baseRate = BaseTowerModel.GetWeapon().Rate;
        model.GetWeapon().rate *= SuperMonkeyAttackSpeed / baseRate;
        model.range += 8;
        model.GetAttackModels()[0].range += 8;
        foreach (var projectileModel in model.GetDescendants<ProjectileModel>().ToList()
                     .Where(projectileModel => !string.IsNullOrEmpty(projectileModel.display.AssetGUID)))
        {
            projectileModel.GetBehavior<TravelStraitModel>().lifespan *= 2f;
            projectileModel.GetBehavior<TravelStraitModel>().lifespanFrames *= 2;
        }
    }

    protected override void RemoveAbility(TowerModel model)
    {
        if (Mode2)
        {
            base.RemoveAbility(model);
        }
        else
        {
            TechBotify(model);
        }
    }

    public void ModifyModel(MonkeyFanClubModel model, AbilityModel ability)
    {
        model.lifespanFrames = (int) Math.Round(ability.EffectiveCooldownFrames()) + 2;
        model.lifespan = model.lifespanFrames / 60f;
        model.towerCount = Math.RoundToNearestInt(BaseModel.towerCount * BaseModel.lifespan / (ability.EffectiveCooldown() * Factor), 1);
    }

    [HarmonyPatch(typeof(MonkeyFanClub), nameof(MonkeyFanClub.UpdatedModel))]
    internal static class MonkeyFanClub_UpdatedModel
    {
        [HarmonyPrefix]
        internal static void Prefix(MonkeyFanClub __instance, Model modelToUse)
        {
            var superMonkeyFanClub = __instance.isPlasma
                                         ? GetInstance<PlasmaMonkeyFanClub>()
                                         : GetInstance<SuperMonkeyFanClub>();

            if (superMonkeyFanClub.Mode != 1) return;

            var fanClubModel = modelToUse.Cast<MonkeyFanClubModel>();
            var realAbility =
                __instance.ability.tower.towerModel.GetDescendant<ActivateAbilityAfterIntervalModel>().abilityModel;

            superMonkeyFanClub.ModifyModel(fanClubModel, realAbility);
        }
    }

    [HarmonyPatch(typeof(MonkeyFanClub), nameof(MonkeyFanClub.Activate))]
    internal static class MonkeyFanClub_Activate
    {
        [HarmonyPrefix]
        internal static bool Prefix(MonkeyFanClub __instance)
        {
            var superMonkeyFanClub = __instance.isPlasma
                                         ? GetInstance<PlasmaMonkeyFanClub>()
                                         : GetInstance<SuperMonkeyFanClub>();

            if (superMonkeyFanClub.Mode != 1) return true;

            var fanClubModel = __instance.monkeyFanClubModel;
            var realAbility =
                __instance.ability.tower.towerModel.GetDescendant<ActivateAbilityAfterIntervalModel>().abilityModel;

            superMonkeyFanClub.ModifyModel(fanClubModel, realAbility);

            var towers = __instance.Sim.towerManager
                .GetClosestTowers(__instance.ability.tower.Position)
                .ToArray()
                .Where(tower => tower != __instance.ability.tower && !tower.IsSuspended &&
                                tower.towerModel.baseId == __instance.ability.tower.towerModel.baseId &&
                                tower.towerModel.tiers[0] <= fanClubModel.maxTier &&
                                tower.towerModel.tiers[1] <= 4 &&
                                tower.towerModel.tiers[2] <= fanClubModel.maxTier &&
                                !fanClubModel.ignoreWithMutatorsList.Any(tower.IsMutatedBy) &&
                                (__instance.isPlasma || tower.GetMutatorById("MonkeyFanClubPlasma") == null))
                .Prepend(__instance.ability.tower)
                .ToArray();

            var closestMutators = towers
                .Take(fanClubModel.towerCount)
                .Select(tower => tower.GetMutatorById(__instance.mutatorId))
                .Where(mutator => mutator != null)
                .ToArray();


            // Extend duration instead of replacing
            if ((closestMutators.Length == fanClubModel.towerCount || closestMutators.Length == towers.Length) &&
                closestMutators.Skip(1).All(mutator => mutator.mutator == __instance.mutator))
            {
                foreach (var mutator in closestMutators)
                {
                    mutator.totalDuration = fanClubModel.lifespanFrames;
                    mutator.removeAt = fanClubModel.lifespanFrames;
                }
                return false;
            }

            // Remove so that new mutators can be applied
            foreach (var tower in towers)
            {
                if (tower.GetMutatorById(__instance.mutatorId) is { } mutator &&
                    (mutator.mutator == __instance.mutator || tower == __instance.ability.tower))
                {
                    tower.RemoveMutator(mutator.mutator);
                }
            }

            return true;
        }
    }
}