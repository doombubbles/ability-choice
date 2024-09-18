using System.Collections.Generic;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Effects;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Weapons.Behaviors;

namespace AbilityChoice.AbilityChoices.Hero.Gwendolin;

public class Firestorm : HeroAbilityChoice
{
    public override string HeroId => TowerType.Gwendolin;

    public override Dictionary<int, string> Descriptions1 => new()
    {
        { 10, "Periodically emits storms of fire, burning nearby Bloons. Duration of Heat it Up is increased." },
        {
            16,
            "Firestorms have increased duration and do more damage. Purple Bloons are no longer immune to Gwendolin's attacks."
        },
        { 20, "Firestorms does hugely increased damage." }
    };

    public override Dictionary<int, string> Descriptions2 => new()
    {
        { 10, "Range and duration of heat it up increased, and its blast wave deals much more damage." },
        {
            16,
            "Blast wave burn damage and duration increased. Purple Bloons are no longer immune to Gwendolin's attacks."
        },
        { 20, "Blast wave does hugely increased damage." }
    };

    private const int Factor = 6;

    public override void Apply1(TowerModel model)
    {
        var ability = AbilityModel(model);

        ability.GetDescendants<AttackModel>().ForEach(attackModel =>
        {
            var attack = attackModel.Duplicate();
            var weapon = attack.weapons[0];
            var projectile = weapon.projectile;

            weapon.Rate = ability.Cooldown / Factor;
            attack.fireWithoutTarget = false;

            projectile.GetDamageModel().damage /= Factor;

            projectile.GetBehavior<AddBehaviorToBloonModel>().lifespan /= Factor;
            projectile.RemoveBehavior<PierceUpTowersModel>();
            projectile.RemoveBehavior<HeatItUpDamageBuffModel>();
            projectile.radius = 70;
            attack.range = model.range;

            var effect = ability.GetBehavior<CreateEffectOnAbilityModel>().effectModel;
            effect.scale = .6f;

            weapon.AddBehavior(new EjectEffectModel("", effect.assetId, effect, effect.lifespan, Fullscreen.No, false,
                false, false, false));

            attack.name += "Firestorm";

            model.AddBehavior(attack);
        });
    }

    public override void Apply2(TowerModel model)
    {
        var ability = AbilityModel(model);
        var heatItUp = model.GetDescendant<BonusProjectileAfterIntervalModel>();
        var interval = heatItUp.interval * model.GetWeapon().Rate;
        var fraction = interval / ability.Cooldown;

        heatItUp.projectileModel.scale = 2;
        heatItUp.projectileModel.radius *= 1.5f;

        foreach (var abilityProj in ability.GetDescendants<ProjectileModel>().ToList())
        {
            var newProj = abilityProj.Duplicate();
            newProj.RemoveBehavior<HeatItUpDamageBuffModel>();
            newProj.RemoveBehavior<PierceUpTowersModel>();

            newProj.GetDamageModel().damage *= fraction;
            newProj.GetBehavior<AddBehaviorToBloonModel>().lifespan *= fraction;

            newProj.radius = heatItUp.projectileModel.radius;

            heatItUp.projectileModel.AddBehavior(new CreateProjectileOnExhaustFractionModel("", newProj,
                new SingleEmissionModel("", null), 1, -1, false, false, false));
        }
    }

    protected override void ApplyBoth(TowerModel towerModel)
    {
        var ability = AbilityModel(towerModel);
        var heatItUp = towerModel.GetDescendant<BonusProjectileAfterIntervalModel>();

        var increase = 1 + ability.GetDescendant<HeatItUpDamageBuffModel>().lifespan / ability.Cooldown;

        heatItUp.GetDescendants<HeatItUpDamageBuffModel>().ForEach(model => model.lifespan *= increase);
        heatItUp.GetDescendants<PierceUpTowersModel>().ForEach(model => model.lifespan *= increase);
        heatItUp.GetDescendants<DamageUpTowersModel>().ForEach(model => model.lifespan *= increase);
        heatItUp.GetDescendants<DamageUpTagTowersModel>().ForEach(model => model.lifespan *= increase);
    }
}