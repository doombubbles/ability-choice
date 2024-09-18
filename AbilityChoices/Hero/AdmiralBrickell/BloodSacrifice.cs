using System.Collections.Generic;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;

namespace AbilityChoice.AbilityChoices.Hero.AdmiralBrickell;

public class BloodSacrifice : HeroAbilityChoice
{
    public override string HeroId => TowerType.Adora;

    public override Dictionary<int, string> Descriptions1 => new()
    {
        {
            7,
            "You may pay tribute to Adora, passively spending money in order to give bonus XP to Adora and boost her attack range and rate of fire."
        },
        {
            20,
            "Ball of Light is greatly improved, plus increased rate of fire, range, and cost efficiency for paying tribute."
        }
    };

    public override void Apply1(TowerModel model)
    {
        var ability = AbilityModel(model);

        var bloodSacrifice = ability.GetBehavior<BloodSacrificeModel>();

        model.AddBehavior(bloodSacrifice);
    }
}