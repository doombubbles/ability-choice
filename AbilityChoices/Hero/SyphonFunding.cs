using System.Collections.Generic;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;

namespace AbilityChoice.AbilityChoices.Hero;

public class SyphonFunding : HeroAbilityChoice
{
    public override string HeroId => TowerType.Benjamin;

    public override Dictionary<int, string> Descriptions1 => new()
    {
        { 10, "Every few bloons each round is downgraded by 1 rank and give double cash per pop." },
        { 20, "More Bloons get downgraded, and they now give triple cash per pop." }
    };

    protected override void Apply1(TowerModel model)
    {
        var ability = AbilityModel(model);
        var syphonFunding = ability.GetBehavior<SyphonFundingModel>();

        syphonFunding.lifespan /= ability.Cooldown;

        model.AddBehavior(syphonFunding);

        // See Syphon_OnBloonCreate
    }

    protected override void Apply2(TowerModel model)
    {
        // TODO syphon funding 2
    }
}