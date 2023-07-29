using System.Collections.Generic;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;

namespace AbilityChoice.AbilityChoices.Hero;

public class Heartstopper : HeroAbilityChoice
{
    public override string HeroId => TowerType.Ezili;

    public override Dictionary<int, string> Descriptions1 => new()
    {
        { 3, "Blocks regrow for Bloons within range. All Purple Bloons are less resistant to damage." },
        { 12, "Deals increased damage to MOABs. Purple Bloons are fully susceptible to all damage." }
    };

    protected override void Apply1(TowerModel model)
    {
        var ability = AbilityModel(model);

        var growBlock = ability.GetDescendant<GrowBlockModel>();
        var immune = ability.GetDescendant<IgnoreDmgImmunityModel>();

        immune.chance = model.tier >= 12 ? 1 : .5f;

        model.AddBehavior(
            new AddBehaviorToBloonInZoneModel("GrowBlock", model.range, "Heartstopper", true, new[] { growBlock }, null,
                "GrowBlock"));

        model.AddBehavior(
            new AddBehaviorToBloonInZoneModel("Purple", 999999, "HeartstopperBreakPurple", true, new[] { immune },
                null, ""));
    }

    protected override void Apply2(TowerModel model)
    {
        // TODO heartstopper 2 ?
    }
}