using System.Collections.Generic;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Weapons;

namespace AbilityChoice.AbilityChoices.Hero.Quincy;

public class RapidShot : HeroAbilityChoice
{
    public override string HeroId => TowerType.Quincy;

    public override Dictionary<int, string> Descriptions1 => new()
    {
        {3, "Increased attack speed."},
        {13, "Small range and attack speed increase increase."},
        {15, "Further increased attack speed."}
    };

    public override Dictionary<int, string> Descriptions2 => new()
    {
        {3, "Quincy's attacks now ACTUALLY never miss. Slightly increased attack speed."},
        {13, "Small range and attack speed increase increase."},
        {15, "Further increased attack speed."}
    };

    public override void Apply1(TowerModel model)
    {
        var ability = AbilityModel(model);

        var turbo = ability.GetDescendant<TurboModel>();

        var bonus = CalcAvgBonus(turbo.Lifespan / ability.Cooldown, 1 / turbo.multiplier);

        model.GetDescendants<WeaponModel>().ForEach(weaponModel => weaponModel.Rate /= bonus);
    }

    public override void Apply2(TowerModel model)
    {
        var ability = AbilityModel(model);

        var turbo = ability.GetDescendant<TurboModel>();

        var bonus = CalcAvgBonus(0.5f * turbo.Lifespan / ability.Cooldown, 1 / turbo.multiplier);

        model.GetDescendants<WeaponModel>().ForEach(weaponModel => weaponModel.Rate /= bonus);

        foreach (var proj in model.GetAttackModel().GetDescendants<ProjectileModel>().ToList()
                     .Where(proj => proj.HasBehavior<RetargetOnContactModel>()))
        {
            proj.RemoveBehavior<RetargetOnContactModel>();
            proj.AddBehavior(new TrackTargetModel("", 100, true, true, 360, false, 99999, false, false));
        }
    }
}