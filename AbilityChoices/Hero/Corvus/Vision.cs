using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;

namespace AbilityChoice.AbilityChoices.Hero.Corvus;

public class Vision : CorvusAbilityChoice
{
    public override string Description1 => "Lets the Spirit see Camo Bloons, and remove Camo property from anything it hits.";
    public override string Description2 => "Lets the Spirit see Camo Bloons.";

    protected override int Order => 2;

    public override void Apply1(TowerModel model)
    {
        var spell = InstantSpell(model);

        spell.initialManaCost = (int) (spell.initialManaCost / spell.duration);

        spell.duration = 1;
        spell.cooldown = 0;

        spell.soundModel = null;
    }

    public override void Apply2(TowerModel model)
    {
        var spell = InstantSpell(model);

        var factor = spell.duration / (spell.duration + spell.cooldown);

        spell.initialManaCost = (int) (factor * (spell.initialManaCost / spell.duration));

        spell.duration = 1;
        spell.cooldown = 0;
            
        spell.soundModel = null;
    }

    [HarmonyPatch(typeof(Il2CppAssets.Scripts.Simulation.Corvus.Spells.Instant.Vision.VisionMutator),
        nameof(Il2CppAssets.Scripts.Simulation.Corvus.Spells.Instant.Vision.VisionMutator.ChangeTowerModel))]
    internal static class VisionMutator_ChangeTowerModel
    {
        [HarmonyPostfix]
        internal static void Postfix(TowerModel towerModel)
        {
            if (GetInstance<Vision>().Mode2)
            {
                towerModel.GetDescendants<RemoveBloonModifiersModel>()
                    .ForEach(modifiersModel => modifiersModel.cleanseCamo = false);
            }
        }
    }
}