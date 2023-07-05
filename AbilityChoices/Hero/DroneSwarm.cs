using System.Collections.Generic;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using Il2CppSystem;

namespace AbilityChoice.AbilityChoices.Hero;

public class DroneSwarm : HeroAbilityChoice
{
    public override string HeroId => TowerType.Etienne;

    public override Dictionary<int, string> Descriptions1 => new()
    {
        { 3, "Etienne launches an additional permanent drone." },
        { 6, "Etienne now controls 3 drones at once." },
        { 7, "Drone pierce increased." },
        { 9, "Drones pop an extra Bloon layer. Adds a fourth drone." },
        { 11, "Etienne gains a fifth drone to control." },
        { 16, "Etienne's range and drone popping power both improved." },
        {
            19, "Etienne now controls six drones all the time!"
        }
    };

    protected override void Apply1(TowerModel model)
    {
        var ability = AbilityModel(model);
        var droneSupport = model.GetBehavior<DroneSupportModel>();
        var current = droneSupport.count;
        var bonus = ability.GetDescendant<DroneSupportModel>().count;
        var duration = ability.GetDescendant<DroneSwarmModel>().duration;
        var cooldown = ability.Cooldown;

        var avg = current * CalcAvgBonus(duration / cooldown, (current + bonus) / (float) current);

        /*
        ModHelper.Msg<AbilityChoiceMod>(
            $"Level {model.tier}: {current} -> {avg * current:N1} (x{avg:N1}, +{(avg * current) - current})");
            */

        droneSupport.count++;
        if (model.tier is 6 or >= 9)
        {
            droneSupport.count++;
        }

        var now = droneSupport.count;

        var shouldBeMore = avg / now;
        var proj = droneSupport.droneModel.GetDescendant<ProjectileModel>();

        var pierce = Math.Max(shouldBeMore * proj.pierce, 1);

        proj.pierce += pierce;

        var attack = model.GetAttackModel();
        if (!attack.HasBehavior<TargetDivideAndConquerModel>())
        {
            attack.AddBehavior(new TargetDivideAndConquerModel("", true, false));
            model.UpdateTargetProviders();
        }
    }

    protected override void Apply2(TowerModel model)
    {
        // TODO drone swarm 2
    }
}