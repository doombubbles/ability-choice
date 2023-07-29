using System.Collections.Generic;
using BTD_Mod_Helper.Extensions;
using Il2Cpp;
using Il2CppAssets.Scripts.Models.GenericBehaviors;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Weapons;
using Il2CppAssets.Scripts.Models.Towers.Weapons.Behaviors;

namespace AbilityChoice.AbilityChoices.Hero;

public class LongArmOfLight : HeroAbilityChoice
{
    public override string AbilityName => "The Long Arm of Light";

    public override string HeroId => TowerType.Adora;

    public override Dictionary<int, string> Descriptions1 => new()
    {
        { 3, "Further increased attack range. Everything third shot has bonus power & damages all Bloon types." },
        { 16, "Long Arm of Light projectile becomes even more deadly, and now happens every other attack." }
    };

    protected override void Apply1(TowerModel model)
    {
        var ability = AbilityModel(model);

        var longArmOfLight = ability.GetBehavior<LongArmOfLightModel>();

        var interval = model.tier >= 16 ? 2 : 3;

        var uptime = longArmOfLight.Lifespan / ability.Cooldown;

        var bonus = CalcAvgBonus(uptime, longArmOfLight.multiplier);

        model.range *= bonus;
        foreach (var attackModel in model.GetAttackModels())
        {
            attackModel.range *= bonus;
        }

        var weapon = model.GetAttackModel().GetChild<WeaponModel>();
        var proj = weapon.projectile.Duplicate();

        proj.display = proj.GetBehavior<DisplayModel>().display = longArmOfLight.projectileDisplay.assetPath;

        proj.pierce *= longArmOfLight.multiplier;
        proj.radius *= longArmOfLight.projectileRadiusMultiplier;
        proj.GetDescendants<DamageModel>().ForEach(damageModel =>
        {
            damageModel.immuneBloonProperties = longArmOfLight.immuneBloonProperties;
            damageModel.damage += longArmOfLight.damageIncrease;
        });

        var trackTarget = proj.GetBehavior<AdoraTrackTargetModel>();
        trackTarget.minimumSpeed *= longArmOfLight.multiplier;
        trackTarget.minimumSpeedFrames *= longArmOfLight.multiplier;
        trackTarget.rotation *= longArmOfLight.multiplier;
        trackTarget.maximumSpeed *= longArmOfLight.multiplier;
        trackTarget.maximumSpeedFrames *= longArmOfLight.multiplier;
        trackTarget.acceleration *= longArmOfLight.multiplier;
        trackTarget.accelerationFrames *= longArmOfLight.multiplier;
        trackTarget.Lifespan *= longArmOfLight.multiplier;


        var alt = new AlternateProjectileModel("", proj, null, interval);

        weapon.AddBehavior(alt);
    }

    protected override void Apply2(TowerModel model)
    {
        // TODO long arm of light 2 ?
    }
}