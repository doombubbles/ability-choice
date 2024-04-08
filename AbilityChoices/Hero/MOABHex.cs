using System.Collections.Generic;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;

namespace AbilityChoice.AbilityChoices.Hero;

public class MOABHex : HeroAbilityChoice
{
    public override string HeroId => TowerType.Ezili;

    public override Dictionary<int, string> Descriptions1 => new()
    {
        { 10, "Periodically places a hex on a MOAB-CLass Bloon, dealing massive % damage to it." },
    };

    public override Dictionary<int, string> Descriptions2 => new()
    {
        { 10, "Main attack now does bonus % health damage to MOABs. Range increased." },
        { 20, "Main attack % damage increased, and can now affect BAD Bloons. Range increased." },
    };


    private const int Factor = 5;

    public override void Apply1(TowerModel model)
    {
        var ability = AbilityModel(model);
        ability.RemoveBehaviors<CreateEffectOnAbilityModel>();
        ability.RemoveBehaviors<CreateSoundOnAbilityModel>();

        var hexManager = model.GetBehavior<HexManagerModel>();
        hexManager.hex.totalDuration /= Factor;
        hexManager.hex.totalIntervalFrames /= Factor;

        var addBehavior = ability.GetDescendant<AddBehaviorToBloonModel>();
        addBehavior.lifespan /= Factor;
        addBehavior.lifespanFrames /= Factor;

        var hex = ability.GetDescendant<HexModel>();

        hex.totalDuration /= Factor;
        hex.totalIntervalFrames /= Factor;

        ability.Cooldown /= Factor;
    }

    private static readonly List<string> NonBADs = new() { BloonTag.Moab, BloonTag.Bfb, BloonTag.Zomg, BloonTag.Ddt, };

    public override void Apply2(TowerModel model)
    {
        var ability = AbilityModel(model);

        var hex = model.GetDescendant<HexModel>();
        var uptime = hex.totalDuration / ability.Cooldown;

        // Also add some to the main single target attack
        var proj = model.GetDescendant<CreateProjectileOnContactModel>().projectile;

        var percent = hex.damagePercentOfMax * uptime;

        if (model.tier < 20)
        {
            foreach (var tag in NonBADs)
            {
                proj.AddBehavior(new DamagePercentOfMaxModel(tag, percent, new[]
                {
                    tag
                }, false));
            }

            model.range += 20;
        }
        else
        {
            proj.AddBehavior(new DamagePercentOfMaxModel("", percent, new[]
            {
                BloonTag.Moabs
            }, false));

            model.range += 10;
        }

        proj.hasDamageModifiers = true;
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