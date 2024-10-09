using System;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;

namespace AbilityChoice.AbilityChoices.Primary;

public class SuperMonkeyFanClub : TowerAbilityChoice
{
    public override string UpgradeId => UpgradeType.SuperMonkeyFanClub;

    public override string Description1 =>
        "Up to 3 nearby Dart Monkeys including itself are permanently Super Monkey fans.";

    public override string Description2 => "Gains permanent Super attack speed and range itself.";

    protected virtual float SuperMonkeyAttackSpeed => .06f;

    public override void Apply1(TowerModel model)
    {
        var abilityModel = AbilityModel(model);
        // abilityModel.enabled = false;
        abilityModel.CooldownSpeedScale = -1;

        var monkeyFanClubModel = abilityModel.GetBehavior<MonkeyFanClubModel>();

        monkeyFanClubModel.towerCount =
            (int) Math.Round(monkeyFanClubModel.towerCount * monkeyFanClubModel.lifespan / abilityModel.Cooldown);

        const int interval = 5;

        monkeyFanClubModel.lifespan = interval + 1;
        monkeyFanClubModel.lifespanFrames = (interval + 1) * 60;

        monkeyFanClubModel.effectOnOtherId = CreatePrefabReference("");
        monkeyFanClubModel.effectModel.assetId = CreatePrefabReference("");
        monkeyFanClubModel.otherDisplayModel.display = CreatePrefabReference("");
        monkeyFanClubModel.endDisplayModel.effectModel.assetId = CreatePrefabReference("");

        abilityModel.RemoveBehavior<CreateEffectOnAbilityModel>();
        abilityModel.RemoveBehavior<CreateSoundOnAbilityModel>();

        model.AddBehavior(new ActivateAbilityAfterIntervalModel("ActivateAbilityAfterIntervalModel_", abilityModel,
            interval));
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
        if (setting.Value == 1)
        {
            return;
        }

        base.RemoveAbility(model);
    }
}