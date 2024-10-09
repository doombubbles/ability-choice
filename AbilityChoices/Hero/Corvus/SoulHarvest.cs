using System.Collections.Generic;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;

namespace AbilityChoice.AbilityChoices.Hero.Corvus;

public class SoulHarvest : HeroAbilityChoice
{
    public override string HeroId => TowerType.Corvus;

    public override Dictionary<int, string> Descriptions1 => new()
    {
        {3, "Corvus periodically harvests nearby Bloons in an instant. Learns: Echo, Haste."}
    };

    private const int Factor = 5;

    public override void Apply1(TowerModel model)
    {
        var ability = AbilityModel(model);

        var attack = ability.GetDescendant<AttackModel>().Duplicate();

        attack.weapons[0].Rate = ability.Cooldown / Factor;
        
        attack.GetDescendant<DamageModel>().damage /= Factor;
        attack.GetDescendants<DamageModifierForTagModel>().ForEach(tagModel => tagModel.damageAddative /= Factor);

        attack.GetDescendant<AddCorvusManaModel>().amountOnEmit /= Factor;
        
        model.AddBehavior(attack);
    }
}