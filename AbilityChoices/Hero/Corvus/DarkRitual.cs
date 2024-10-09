using System.Collections.Generic;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Weapons.Behaviors;

namespace AbilityChoice.AbilityChoices.Hero.Corvus;

public class DarkRitual : HeroAbilityChoice
{
    public override string HeroId => TowerType.Corvus;

    public override Dictionary<int, string> Descriptions1 => new()
    {
        {
            10,
            "Performs Dark Rituals that harvest a huge number of Bloons near Corvus. Trample does more damage to more Bloons. Learns: Recovery"
        }
    };

    public override Dictionary<int, string> Descriptions2 => new()
    {
        {
            10,
            "Constantly performs a Dark Ritual that harvests Bloons near Corvus. Trample does more damage to more Bloons. Learns: Recovery"
        }
    };


    private const int Factor = 6;

    public override void Apply1(TowerModel model)
    {
        var ability = AbilityModel(model);

        var attack = ability.GetDescendant<AttackModel>().Duplicate();

        attack.weapons[0].Rate = ability.Cooldown / Factor;

        attack.GetDescendant<AgeModel>().Lifespan /= Factor;

        model.AddBehavior(attack);
    }

    public override void Apply2(TowerModel model)
    {
        var ability = AbilityModel(model);

        var attack = ability.GetDescendant<AttackModel>().Duplicate();
        var lifespan = attack.GetDescendant<AgeModel>().Lifespan;

        var weapon = attack.weapons[0];
        weapon.Rate = lifespan;
        weapon.AddBehavior(new WeaponRateMinModel("", lifespan));
        weapon.animation = -1;

        var clear = attack.GetDescendant<ClearHitBloonsModel>();
        clear.interval *= ability.Cooldown / lifespan;
        clear.intervalFrames = (int) (clear.interval * 60);

        var refresh = attack.GetDescendant<RefreshPierceModel>();
        refresh.interval *= ability.Cooldown / lifespan;
        refresh.intervalFrames = (int) (refresh.interval * 60);

        model.AddBehavior(attack);
    }
}