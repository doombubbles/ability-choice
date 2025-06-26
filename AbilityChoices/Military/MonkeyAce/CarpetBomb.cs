using AbilityChoice.Displays;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.Helpers;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Weapons.Behaviors;
using Il2CppAssets.Scripts.Simulation.Towers.Behaviors;
using Il2CppAssets.Scripts.Unity;
using Il2CppNinjaKiwi.Common.ResourceUtils;
namespace AbilityChoice.AbilityChoices.Military.MonkeyAce;

public class CarpetBomb : TowerAbilityChoice
{
    public override string UpgradeId => UpgradeType.MonkeyaceParagon;

    public override string Description1 =>
        "A flying shadow of darkness spells Bloon obliteration. Occasionally drops carpet bombs along a selected path.";

    public override string Description2 =>
        "A flying shadow of darkness spells Bloon obliteration. Joined by 3 bomber planes that launch homing bombs at the strongest Bloon.";

    private const int Factor = 4;

    public override void Apply1(TowerModel model)
    {
        var ability = AbilityModel(model);
        var carpetBomb = ability.GetBehavior<CarpetBombAbilityModel>();
        var proj = carpetBomb.projectileModel;
        ability.Cooldown /= Factor;

        proj.GetDescendants<DamageModel>().ForEach(damageModel => damageModel.damage /= Factor);
        proj.GetDescendants<DamageModifierForTagModel>().ForEach(tagModel => tagModel.damageAddative /= Factor);
        proj.GetDescendants<SlowModel>().ForEach(slowModel => slowModel.Lifespan /= Factor);

        TechBotify(model);
    }

    public override void Apply2(TowerModel model)
    {
        var ability = AbilityModel(model);
        var carpetBomb = ability.GetBehavior<CarpetBombAbilityModel>();

        var proj = carpetBomb.projectileModel.Duplicate();

        proj.GetDescendants<SlowModel>().ForEach(slowModel => slowModel.Lifespan /= 3);

        var createEffect = proj.GetBehavior<CreateEffectOnExhaustFractionModel>();
        proj.RemoveBehavior(createEffect);
        proj.SetDisplay(createEffect.effectModel.assetId);
        proj.AddBehavior(new ArriveAtTargetModel("", 0, new[] {0, 0.9f}, true,
            false, 500, false, true, false, 360f, true));

        var overallRate = ability.Cooldown / carpetBomb.numProjectiles;

        var basePlane = Game.instance.model.GetTower(TowerType.MonkeyBuccaneer, 4, 0, 0).GetDescendant<TowerModel>();

        for (var i = 0; i < 3; i++)
        {
            var tower = basePlane.Duplicate("ParagonBomber");

            tower.baseId = "ParagonBomber";

            tower.RemoveBehaviors<AttackModel>();
            tower.RemoveBehavior<TowerExpireOnParentUpgradedModel>();
            tower.GetBehavior<AirUnitModel>().display = new PrefabReference(GetDisplayGUID<ParagonBomber>());

            tower.AddBehavior(new AttackHelper("", true)
            {
                Range = 2000,
                AttackThroughWalls = true,
                CanSeeCamo = true,
                TargetProvider = new FighterPilotPatternStrongModel("", false, 40, true),
                Weapon = new WeaponHelper
                {
                    Emission = carpetBomb.singleEmissionModel.Duplicate(),
                    Rate = overallRate * 3,
                    CustomStartCooldown = overallRate * i,
                    StartInCooldown = true,
                    Projectile = proj.Duplicate(),
                    Behaviors =
                    [
                        new FireFromAirUnitModel("")
                    ]
                }
            });

            tower.GetDescendant<FighterMovementModel>().maxSpeed += i;


            tower.UpdateTargetProviders();

            model.AddBehavior(new TowerCreateTowerModel("Plane" + i, tower, true));
        }


    }



    [HarmonyPatch(typeof(FighterMovement), nameof(FighterMovement.Process))]
    internal static class FighterMovement_Process
    {
        [HarmonyPrefix]
        private static void Prefix(FighterMovement __instance)
        {
            if (__instance.currentPathSupplier == null)
            {
                __instance.Attatched();
            }
        }
    }
}