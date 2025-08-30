using System.Collections.Generic;
using BTD_Mod_Helper.Extensions;
using Il2Cpp;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;

namespace AbilityChoice.AbilityChoices.Hero.Silas;

public class FrozenCascade : HeroAbilityChoice
{
    public override string HeroId => TowerType.Silas;

    public override Dictionary<int, string> Descriptions1 => new()
    {
        {
            7,
            "Occasionally creates shattering blasts of ice around Silas then other Ice Monkeys spreading out from Silas. Frozen Bloons take extra damage."
        }
    };

    public override Dictionary<int, string> Descriptions2 => new()
    {
        {
            7,
            "The Ice Fragments of nearby Ice Monkey can hit all Bloon types and do more damage, further increased against Frozen Bloons."
        }
    };

    private const int Factor = 5;

    public override void Apply1(TowerModel model)
    {
        var ability = AbilityModel(model);
        ability.Cooldown /= Factor;

        ability.RemoveBehavior<CreateSoundOnAbilityModel>();

        ability.GetDescendants<DamageModel>().ForEach(damage => damage.damage /= Factor);
        ability.GetDescendants<DamageModifierForTagModel>().ForEach(modifier => modifier.damageAddative /= Factor);
        ability.GetDescendants<FreezeModel>().ForEach(freeze => freeze.Lifespan /= Factor);
        ability.GetDescendants<AddHeatToBloonModel>().ForEach(heat => heat.heatAmount /= Factor);
    }

    public override void Apply2(TowerModel model)
    {
        var ability = AbilityModel(model);

        var iceFragment = model.GetBehavior<SilasIceFragmentsModel>().iceFragmentProjectileModel;

        iceFragment.GetDescendants<DamageModel>().ForEach(damage =>
        {
            damage.damage *= 120 / ability.Cooldown;
            damage.immuneBloonProperties = BloonProperties.None;
            damage.immuneBloonPropertiesOriginal = BloonProperties.None;
        });
        iceFragment.GetDescendants<DamageModifierForBloonStateModel>().ForEach(state =>
        {
            state.damageAdditive *= 120 / ability.Cooldown;
        });
    }

    protected override void RemoveAbility(TowerModel model)
    {
        if (Mode2)
        {
            base.RemoveAbility(model);
        }
        else
        {
            TechBotify(model);
        }
    }
}