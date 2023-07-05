using System.Collections.Generic;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Effects;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;

namespace AbilityChoice.AbilityChoices.Hero;

public class RallyingRoar : HeroAbilityChoice
{
    public override string HeroId => TowerType.PatFusty;

    public override Dictionary<int, string> Descriptions1 => new()
    {
        { 3, "Occasionally lets out a roar, rallying nearby Monkeys to pop +1 layer for a short time." }
    };

    private const int Factor = 3;

    protected override void Apply1(TowerModel model)
    {
        var ability = AbilityModel(model);

        ability.RemoveBehavior<CreateSoundOnAbilityModel>();
        ability.GetBehavior<CreateEffectOnAbilityModel>().effectModel.scale = .5f;
        ability.Cooldown /= Factor;

        var buff = ability.GetBehavior<ActivateTowerDamageSupportZoneModel>();

        buff.lifespan /= Factor;
        buff.lifespanFrames /= Factor;
    }

    protected override void Apply2(TowerModel model)
    {
        // TODO rallying roar 2
    }

    protected override void RemoveAbility(TowerModel model)
    {
        TechBotify(model);
    }
}