using System;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.Helpers;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using Il2CppAssets.Scripts.Models.Effects;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Weapons;
using Il2CppAssets.Scripts.Models.Towers.Weapons.Behaviors;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem.IO;
using static Il2CppAssets.Scripts.Models.Towers.Behaviors.MagusPerfectusGraveyardStateManagerModel;

namespace AbilityChoice.AbilityChoices.Paragon;

public class ArcaneMetamorphosis : TowerAbilityChoice
{
    public override string UpgradeId => UpgradeType.WizardmonkeyParagon;

    public override string Description1 =>
        "Only the most perfect Wizard can channel the ancient powers of the beyond. Spends 1000 mana/s on flame thrower + wall of fire attacks, and 25% current mana on occasional Phoenix explosions that create zombie ZOMGs/BFBs.";

    public override string Description2 =>
        "Only the most perfect Wizard can channel the ancient powers of the beyond. Foregoes necromancy and mana to permanently use all attacks at once.";

    internal const int Factor = 4;

    public override void Apply1(TowerModel model)
    {
        var ability = AbilityModel(model);

        var arcaneMetamorphosis = ability.GetBehavior<ArcaneMetamorphosisModel>();

        arcaneMetamorphosis.manaPerSecond /= Factor;

        var manager = model.GetBehavior<MagusPerfectusGraveyardStateManagerModel>();
        var drainAttackNames = manager.attackManagerStates[StateDrainingOverTime];
        var drainAttacks = model.GetAttackModels().Where(a => drainAttackNames.Contains(a.name)).ToArray();
        var drainWeapons = drainAttacks.SelectMany(attackModel => attackModel.weapons).ToArray();

        foreach (var drainWeapon in drainWeapons)
        {
            drainWeapon.Rate *= Factor;
        }

        manager.attackManagerStates[StateConsuming] =
            manager.attackManagerStates[StateConsuming].Concat(drainAttackNames).ToArray();

        manager.attackManagerStates[StateDrainingOverTime] = new Il2CppStringArray(0);



        var ability2 = model.GetAbilities().First(a => a.displayName == "Phoenix Rebirth");
        var phoenixRebirth = ability2.GetBehavior<PhoenixRebirthModel>();

        ability2.Cooldown /= Factor;
        ability2.RemoveBehavior<CreateSoundOnAbilityModel>();

        phoenixRebirth.duration = 0;

        phoenixRebirth.projectileExplosionModel.GetDescendant<AddBehaviorToBloonModel>().lifespan /= Factor;
        phoenixRebirth.projectileExplosionModel.GetDescendant<DamageOverTimeModel>().damage /= Factor;

        phoenixRebirth.effectEndSubtowerModel.assetId.guidRef = "";

        phoenixRebirth.disabledSubTowers = Array.Empty<TowerModel>();

        TechBotify(model, ability2);
    }

    private const int Factor2 = 5;

    public override void Apply2(TowerModel model)
    {
        model.towerSelectionMenuThemeId = "Camo";

        model.GetAttackModel("Flamethrower").weapons[0]!.Rate *= Factor2;
        model.GetAttackModel("Walls of Fire").GetDescendants<DamageModel>().ForEach(d => d.damage /= Factor2);

        model.RemoveBehavior<MagusPerfectusGraveyardStateManagerModel>();
        model.GetDescendants<WeaponModel>().ToArray()
            .Concat(model.GetDescendants<TowerModel>().ToArray()
                .SelectMany(m => m.GetDescendants<WeaponModel>().ToArray()))
            .ToArray()
            .ForEach(w => w.RemoveBehavior<MagusPerfectusGraveyardModel>());
        model.GetDescendants<ProjectileModel>().ForEach(p => p.RemoveBehavior<SpawnZombieOnBloonDestroyedModel>());

        var ability2 = model.GetAbilities().First(a => a.displayName == "Phoenix Rebirth");
        var phoenixRebirth = ability2.GetBehavior<PhoenixRebirthModel>();
        phoenixRebirth.projectileExplosionModel.GetDescendant<AddBehaviorToBloonModel>().lifespan /= Factor2;
        phoenixRebirth.projectileExplosionModel.GetDescendant<DamageOverTimeModel>().damage /= Factor2;

        foreach (var towerModel in model.GetBehavior<TowerCreateParagonTowerModel>().towerModels)
        {
            towerModel.AddBehavior(new AttackHelper("Explosion")
            {
                Range = 2000,
                AddToSharedGrid = false,
                CanSeeCamo = true,
                FireWithoutTarget = true,
                AttackThroughWalls = true,
                Weapon = new WeaponHelper
                {
                    Rate = ability2.Cooldown / Factor,
                    FireWithoutTarget = true,
                    Projectile = phoenixRebirth.projectileExplosionModel.Duplicate(),
                    Behaviors =
                    [
                        new EjectEffectModel("", phoenixRebirth.effectSubtowerModel, 2.0f, Fullscreen.No, false, false,
                            false, false)
                    ]
                }
            });

            towerModel.UpdateTargetProviders();
        }
    }

    protected override void RemoveAbility(TowerModel model)
    {
        if (Mode2)
        {
            model.RemoveBehaviors<AbilityModel>();
        }
        else
        {
            AbilityModel(model).displayName += AbilityChoiceMod.DontShowAbilityKeyword;
        }
    }
}