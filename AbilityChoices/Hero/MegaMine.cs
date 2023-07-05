using System.Collections.Generic;
using AbilityChoice.Displays;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Effects;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Filters;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Weapons;
using Il2CppAssets.Scripts.Models.Towers.Weapons.Behaviors;
using Il2CppAssets.Scripts.Unity;

namespace AbilityChoice.AbilityChoices.Hero;

public class MegaMine : HeroAbilityChoice
{
    public override string HeroId => TowerType.AdmiralBrickell;

    public override Dictionary<int, string> Descriptions1 => new()
    {
        {
            10,
            "Deploys Mega Mines to a targeted water location that trigger on MOABs and stun nearby Bloons. Mega Mines last 3 rounds."
        },
        { 13, "Mega Mines are created more frequently." },
        { 18, "Mega Mines are created even more frequently." },
        { 20, "Mega Mines do massively increased damage and stun longer." }
    };

    public override Dictionary<int, string> Descriptions2 => new()
    {
        { 10, "Sea Mine explosions are larger, deal bonus damage to Moabs, and stun Bloons." },
        { 13, "Sea Mine pierce and Moab damage increased." },
        { 18, "Sea Mine stun duration and Moab Damage increased." },
        { 20, "Sea Mines have more pierce, stun for longer, and their Moab damage is massively increased." }
    };

    protected override void Apply1(TowerModel model)
    {
        var ability = AbilityModel(model);

        var placeProjectile = ability.GetBehavior<PlaceProjectileAtModel>();
        var megaMine = placeProjectile.projectileModel.Duplicate();
        megaMine.AddBehavior(new ArriveAtTargetModel("", 1, new float[] { 0, 1 }, true, false, 0, true, false, false,
            0, true));

        var newAttack = Game.instance.model.GetTower(TowerType.WizardMonkey, 1, 2, 0).GetAttackModel("Wall")
            .Duplicate();
        newAttack.name = "AttackModel_MegaMine";

        newAttack.RemoveBehavior<TargetTrackOrDefaultModel>();

        var targetPoint = newAttack.GetBehavior<TargetSelectedPointModel>();
        newAttack.targetProvider = targetPoint;

        targetPoint.displayInvalid = CreatePrefabReference<MegaMineInvalid>();

        targetPoint.lockToInsideTowerRange = false;
        targetPoint.startWithClosestTrackPoint = false;
        targetPoint.projectileToExpireOnTargetChangeModel = null;

        var filters = newAttack.GetBehavior<AttackFilterModel>();
        var offTrack = filters.GetDescendant<FilterOfftrackModel>();
        filters.RemoveChildDependant(offTrack);
        filters.filters = filters.filters.RemoveItem(offTrack);

        var weapon = newAttack.GetDescendant<WeaponModel>();

        weapon.animation = 3;
        weapon.RemoveBehavior<CreateSoundOnProjectileCreatedModel>();

        weapon.RemoveChildDependant(weapon.projectile);
        weapon.projectile = megaMine;
        weapon.AddChildDependant(megaMine);

        weapon.Rate = ability.Cooldown;
        weapon.startInCooldown = false;
        weapon.customStartCooldown = 5;
        weapon.ejectZ = 9999;


        var effectAtTower = placeProjectile.effectAtTowerModel;
        weapon.AddBehavior(new EjectEffectModel("", effectAtTower.assetId, effectAtTower, effectAtTower.lifespan,
            Fullscreen.No, false, false, false, false));


        model.AddBehavior(newAttack);

        model.towerSelectionMenuThemeId = "SelectPointInput";
        model.UpdateTargetProviders();
    }

    protected override void Apply2(TowerModel model)
    {
        var ability = AbilityModel(model);

        var megaMine = ability.GetDescendants<ProjectileModel>().ToList()
            .First(projectileModel => projectileModel.id == "MegaMineExplosion");

        var seaMines = model.GetAttackModel("SeaMine");

        var proj = seaMines.GetDescendants<ProjectileModel>().ToList()
            .First(projectileModel => projectileModel.id == "Explosion");

        var damageFactor = ability.Cooldown;
        var pierceFactor = ability.Cooldown / 2;
        var stunFactor = ability.Cooldown / 5;
        var radiusFactor = 10;

        proj.AddBehavior(new DamageModifierForTagModel("", BloonTag.Moabs, 1,
            megaMine.GetDamageModel().damage / damageFactor, false, false));

        proj.pierce += megaMine.pierce / pierceFactor;

        var slow = megaMine.GetBehavior<SlowModel>().Duplicate();
        slow.Lifespan /= stunFactor;

        proj.radius += megaMine.radius / radiusFactor;

        proj.UpdateCollisionPassList();
        proj.AddBehavior(slow);

        proj.hasDamageModifiers = true;
    }
}