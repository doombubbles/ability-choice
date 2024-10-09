using Il2CppAssets.Scripts.Models.Towers;

namespace AbilityChoice.AbilityChoices.Hero.Corvus;

public class SoulBarrier : CorvusAbilityChoice
{
    protected override int Order => 0;

    public override string Description1 =>
        "If a Bloon gets past your defenses, Temporarily use Mana in place of lives.";

    protected override bool HasMode2 => false;

    public override void Apply1(TowerModel model)
    {
    }

    /*[HarmonyPatch(typeof(Il2CppAssets.Scripts.Simulation.Corvus.Spells.Instant.SoulBarrier),
        nameof(Il2CppAssets.Scripts.Simulation.Corvus.Spells.Instant.SoulBarrier.Cast))]
    internal static class SoulBarrier_Cast
    {
        public static bool allowCast = false;

        [HarmonyPrefix]
        internal static bool Prefix(Il2CppAssets.Scripts.Simulation.Corvus.Spells.Instant.SoulBarrier __instance)
        {
            return !EnabledForSpell(CorvusSpellType.SoulBarrier) || allowCast;
        }
    }*/
}