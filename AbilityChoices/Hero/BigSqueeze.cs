using System.Collections.Generic;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Filters;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Weapons;

namespace AbilityChoice.AbilityChoices.Hero;

public class BigSqueeze : HeroAbilityChoice
{
    public override string HeroId => TowerType.PatFusty;

    public override Dictionary<int, string> Descriptions1 => new()
    {
        {
            10,
            "Pat can grab MOAB-Class Bloons below ZOMGs and crush them to bits while still continuing his other attacks."
        },
    };

    public override Dictionary<int, string> Descriptions2 => new()
    {
        { 10, "Main attack does bonus % health damage to MOAB-class Bloons." },
        { 20, "Main attack slam now does AOE bonus % health damage to MOAB-class Bloons." }
    };

    protected override void Apply1(TowerModel model)
    {
        var ability = AbilityModel(model);

        var attack = ability.GetDescendant<AttackModel>();

        var filter = new FilterOutTagModel(BloonTag.Zomg, BloonTag.Zomg, null);

        var attackFilter = attack.GetBehavior<AttackFilterModel>();
        if (model.tier < 20)
        {
            attackFilter.filters = attackFilter.filters.AddTo(filter);
        }

        var weapon = attack.GetChild<WeaponModel>();

        weapon.Rate = ability.Cooldown;

        var proj = weapon.projectile;

        // proj.GetBehavior<SlowModel>().Lifespan /= 5;
        // proj.GetBehavior<CreateSoundOnDelayedCollisionModel>().delay /= 5;
        // proj.GetBehavior<DelayBloonChildrenSpawningModel>().Lifespan /= 5;

        var projFilter = proj.GetBehavior<ProjectileFilterModel>();
        if (model.tier < 20)
        {
            projFilter.filters = projFilter.filters.AddTo(filter);
        }

        model.AddBehavior(attack);
    }

    private static readonly List<string> NonBADs = new() { BloonTag.Moab, BloonTag.Bfb, BloonTag.Zomg, BloonTag.Ddt, };

    protected override void Apply2(TowerModel model)
    {
        var ability = AbilityModel(model);

        var percent = .5f / ability.Cooldown;

        var projectileModel = model.tier < 20
            ? model.GetDescendant<ProjectileModel>().GetDescendant<ProjectileModel>()
            : model.GetDescendant<ProjectileModel>();

        foreach (var tag in NonBADs)
        {
            projectileModel.AddBehavior(new DamagePercentOfMaxModel(tag, percent, new[] { tag }, false));
        }
    }
}