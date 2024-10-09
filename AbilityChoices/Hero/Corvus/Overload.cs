using HarmonyLib;
using Il2CppAssets.Scripts.Models.CorvusSpells;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Simulation.Corvus.TowerManager;

namespace AbilityChoice.AbilityChoices.Hero.Corvus;

public class Overload : CorvusAbilityChoice
{
    private const int Period = 10;

    public override string Description1 =>
        $"Every {Period} seconds, the Spirit spends mana to emit a detonation of overwhelming energy.";

    public override string Description2 => "The Spirit constantly emits detonations of energy.";

    public override void Apply1(TowerModel model)
    {
        var spell = InstantSpell(model);

        spell.initialManaCost = (int) (Period * spell.initialManaCost / (spell.cooldown + spell.duration));

        var factor = (spell.cooldown + spell.duration) / Period;

        spell.duration = Period;
        spell.cooldown = 0;

        var overload = spell.Cast<OverloadModel>();
        overload.projectile.GetDescendant<DamageModel>().damage /= factor;
    }

    public override void Apply2(TowerModel model)
    {
        var spell = InstantSpell(model);

        spell.initialManaCost = (int) (spell.initialManaCost / (spell.cooldown + spell.duration));

        spell.duration = 1;
        spell.cooldown = 0;

        var overload = spell.Cast<OverloadModel>();
        overload.projectile.GetDescendant<DamageModel>().damage /= spell.cooldown + spell.duration;
    }

    [HarmonyPatch(typeof(Il2CppAssets.Scripts.Simulation.Corvus.Spells.Instant.Overload),
        nameof(Il2CppAssets.Scripts.Simulation.Corvus.Spells.Instant.Overload.Cast))]
    internal static class Overload_Cast
    {
        [HarmonyPrefix]
        internal static bool Prefix(Il2CppAssets.Scripts.Simulation.Corvus.Spells.Instant.Overload __instance)
        {
            if (!EnabledForSpell(CorvusSpellType.Overload)) return true;

            var spell = __instance.spellModel;
            __instance.CreateProjectileAtSpirit(__instance.emission, spell.projectile);

            var corvus = __instance.Sim.GetCorvusManager(__instance.tower.owner);

            if (corvus.duplicateSpiritSubTower != null)
            {
                corvus.CreateProjectileAtDuplicate(__instance.SpellType, __instance.emission, spell.projectile);
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(CorvusManager), nameof(CorvusManager.IsSpellActive), typeof(CorvusSpellType))]
    internal static class CorvusManager_IsSpellActive
    {
        [HarmonyPrefix]
        internal static bool Prefix(CorvusSpellType spellType, ref bool __result)
        {
            if (!CorvusHandler.CorvusManager_TriggerCorvusToReleaseSpirit.releasing ||
                spellType != CorvusSpellType.Overload ||
                !EnabledForSpell(CorvusSpellType.Overload)) return true;

            __result = false;
            return false;
        }
    }
}