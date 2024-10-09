using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2CppAssets.Scripts.Models.CorvusSpells.Instant;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Simulation.Corvus.Spells.Instant;
using Il2CppAssets.Scripts.Simulation.Corvus.TowerManager;
using Il2CppAssets.Scripts.Simulation.Objects;

namespace AbilityChoice.AbilityChoices.Hero.Corvus;

public class Trample : CorvusAbilityChoice
{
    public override string Description1 =>
        "The spirit is joined by a stampede of other spirits, crushing Bloons they pass.";

    private const int Factor = 5;

    public override void Apply1(TowerModel model)
    {
        var spell = InstantSpell(model);

        spell.initialManaCost = (int) (spell.initialManaCost / spell.cooldown);

        spell.duration = 1;
        spell.cooldown = 0;

        var trample = spell.Cast<TrampleModel>();
        trample.projectileModel.GetDescendant<DamageModel>().damage /= Factor;
        trample.projectileModel.pierce /= Factor;
    }


    [HarmonyPatch(typeof(Il2CppAssets.Scripts.Simulation.Corvus.Spells.Instant.Trample),
        nameof(Il2CppAssets.Scripts.Simulation.Corvus.Spells.Instant.Trample.Cast))]
    internal static class Trample_Cast
    {
        [HarmonyPrefix]
        internal static bool Prefix(Il2CppAssets.Scripts.Simulation.Corvus.Spells.Instant.Trample __instance)
        {
            if (!EnabledForSpell(CorvusSpellType.Trample)) return true;

            __instance.ClearProjectiles();

            var spell = __instance.spellModel;
            __instance.CreateProjectileAtSpirit(__instance.emission, spell.projectileModel);

            __instance.projectile = __instance.emittedProjectiles.Get(0);
            __instance.projectile.add_OnDestroyEvent(new System.Action<RootObject>(__instance.OnProjectileDestroy));


            var corvus = __instance.Sim.GetCorvusManager(__instance.tower.owner);

            if (corvus.duplicateSpiritSubTower != null)
            {
                corvus.CreateProjectileAtDuplicate(__instance.SpellType, __instance.emission, spell.projectileModel);

                __instance.projectileDuplicate =
                    corvus.GetDuplicateProjectilesEmittedBySpell(spell.spellType).FirstOrDefault();
                __instance.projectileDuplicate?.add_OnDestroyEvent(
                    new System.Action<RootObject>(__instance.OnProjectileDestroy));
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(Il2CppAssets.Scripts.Simulation.Corvus.Spells.Instant.Trample),
        nameof(Il2CppAssets.Scripts.Simulation.Corvus.Spells.Instant.Trample.Process))]
    internal static class Trample_Process
    {
        [HarmonyPrefix]
        internal static bool Prefix(Il2CppAssets.Scripts.Simulation.Corvus.Spells.Instant.Trample __instance)
        {
            if (!EnabledForSpell(CorvusSpellType.Trample)) return true;

            if (__instance.projectile != null)
            {
                __instance.projectile.Rotation = __instance.SpiritTower.entity.transformBehaviorCache.rotation.value;
            }
            
            if (__instance.projectileDuplicate != null)
            {
                __instance.projectileDuplicate.Rotation = __instance.Corvus.GetDuplicateTowerRotation();
            }

            return false;
        }
    }

    /// <summary>
    /// Clear horsies on deactivation
    /// </summary>
    [HarmonyPatch(typeof(CorvusManager), nameof(CorvusManager.ExpireInstantSpell))]
    internal static class CorvusManager_ExpireInstantSpell
    {
        [HarmonyPostfix]
        internal static void Postfix(CorvusInstantSpell spell)
        {
            if (spell.Is(out Il2CppAssets.Scripts.Simulation.Corvus.Spells.Instant.Trample trample))
            {
                trample.ClearProjectiles();
            }
        }
    }
}