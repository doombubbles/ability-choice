using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.GenericBehaviors;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Simulation.SMath;
using Il2CppAssets.Scripts.Simulation.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Unity;
namespace AbilityChoice.AbilityChoices.Magic.Alchemist;

public class TotalTransformingTonic : TransformingTonic
{
    public override string UpgradeId => UpgradeType.TotalTransformation;

    public override string Description1 =>
        "Has a monstrous laser attack, and 3+ nearby Monkeys are constantly transformed into monsters.";

    public override string Description2 =>
        "Transforms just itself permanently into an even more powerful monster!";

    private MorphTowerModel baseModel;
    public MorphTowerModel BaseModel =>
        baseModel ??= AbilityModel(GetAffected(Game.instance.model).MinBy(model => model.tiers.Sum()))
            .GetBehavior<MorphTowerModel>();

    private const int Factor = 8;

    public override void Apply1(TowerModel model)
    {
        base.Apply1(model);

        var abilityModel = AbilityModel(model);
        // abilityModel.enabled = false;
        var morphTowerModel = abilityModel.GetBehavior<MorphTowerModel>();

        abilityModel.Cooldown /= Factor;

        ModifyModel(morphTowerModel, abilityModel);

        morphTowerModel.effectModel.assetId = CreatePrefabReference("");
        morphTowerModel.effectOnTransitionBackModel.assetId = CreatePrefabReference("");

        abilityModel.RemoveBehavior<CreateEffectOnAbilityModel>();
        abilityModel.RemoveBehavior<CreateSoundOnAbilityModel>();
        abilityModel.RemoveBehavior<IncreaseRangeModel>();
        abilityModel.RemoveBehavior<ActivateAttackModel>();
        abilityModel.RemoveBehavior<SwitchDisplayModel>();
    }

    public override void Apply2(TowerModel model)
    {
        var ability = AbilityModel(model);
        var abilityAttack = ability.GetBehavior<ActivateAttackModel>().attacks[0].Duplicate();
        var abilityWeapon = abilityAttack.weapons[0];
        abilityWeapon.Rate /= 2;
        abilityWeapon.projectile.pierce *= 3;
        abilityWeapon.projectile.GetDamageModel().damage *= 2;

        abilityAttack.range = model.range;

        model.AddBehavior(abilityAttack);

        model.IncreaseRange(ability.GetBehavior<IncreaseRangeModel>().addative);

        var display = ability.GetBehavior<SwitchDisplayModel>().display;
        model.display = model.GetBehavior<DisplayModel>().display = display;
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

    public void ModifyModel(MorphTowerModel model, AbilityModel ability)
    {
        model.lifespanFrames = (int) Math.Round(ability.EffectiveCooldownFrames()) + 2;
        model.lifespan = model.lifespanFrames / 60f;
        model.maxTowers = Math.CeilToInt(BaseModel.maxTowers * BaseModel.lifespan / (ability.EffectiveCooldown() * Factor));
    }

    [HarmonyPatch(typeof(MorphTower), nameof(MorphTower.UpdatedModel))]
    internal static class MorphTower_UpdatedModel
    {
        [HarmonyPrefix]
        internal static void Prefix(MorphTower __instance, Model modelToUse)
        {
            if (GetInstance<TotalTransformingTonic>() is not {Mode: 1} totalTransform) return;

            var morphTowerModel = modelToUse.Cast<MorphTowerModel>();
            var abilityModel = __instance.ability.tower.towerModel.GetDescendant<ActivateAbilityAfterIntervalModel>()
                .abilityModel;

            totalTransform.ModifyModel(morphTowerModel, abilityModel);
        }
    }

    [HarmonyPatch(typeof(MorphTower), nameof(MorphTower.Activate))]
    internal static class MorphTower_Activate
    {
        [HarmonyPrefix]
        internal static void Prefix(MorphTower __instance)
        {
            if (GetInstance<TotalTransformingTonic>() is not {Mode: 1} totalTransform) return;

            var morphTowerModel = __instance.morphTowerModel;
            var abilityModel = __instance.ability.tower.towerModel.GetDescendant<ActivateAbilityAfterIntervalModel>()
                .abilityModel;

            totalTransform.ModifyModel(morphTowerModel, abilityModel);
        }
    }
}