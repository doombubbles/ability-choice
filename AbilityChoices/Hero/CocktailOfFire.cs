using System.Collections.Generic;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using Il2CppAssets.Scripts.Models.GenericBehaviors;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions;
using Il2CppAssets.Scripts.Models.Towers.Filters;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Weapons;
using Il2CppAssets.Scripts.Models.Towers.Weapons.Behaviors;
using Il2CppAssets.Scripts.Simulation.Towers.Weapons.Behaviors;
using Il2CppAssets.Scripts.Utils;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace AbilityChoice.AbilityChoices.Hero;

public class CocktailOfFire : HeroAbilityChoice
{
    public override string AbilityName => "Cocktail of Fire";

    public override string HeroId => TowerType.Gwendolin;

    public override Dictionary<int, string> Descriptions1 => new()
    {
        {
            3,
            "Frequently hurls a flask of flammable liquid, with a chance of burning Bloons that pass through the fire."
        }
    };

    public override Dictionary<int, string> Descriptions2 => new()
    {
        {
            3, "Fire blasts leave behind pools of liquid fire on the track."
        },
        {
            7,
            "Heat it up has increased radius, pools of fire are more effective."
        },
        {
            14,
            "Pools of Fire do extra damage and set MOAB class Bloons alight."
        }
    };

    private const int Factor = 2;

    public override void Apply1(TowerModel model)
    {
        var ability = AbilityModel(model);

        var attack = ability.GetDescendant<AttackModel>().Duplicate();
        var weapon = attack.weapons[0];
        weapon.projectile.RemoveBehavior<CreateSoundOnProjectileExhaustModel>();
        var projectile = weapon.projectile.GetDescendant<ProjectileModel>();
        var ageModel = projectile.GetBehavior<AgeModel>();

        var uptime = ageModel.Lifespan / ability.Cooldown;

        ageModel.Lifespan /= Factor;

        weapon.Rate = ageModel.Lifespan / uptime;

        var addBehavior = projectile.GetDescendant<AddBehaviorToBloonModel>();
        addBehavior.lifespan /= Factor;
        addBehavior.lifespanFrames /= Factor;

        weapon.Rate /= Factor;
        var filterModel = projectile.GetBehavior<ProjectileFilterModel>();
        if (filterModel == null)
        {
            filterModel = new ProjectileFilterModel("", null);
            projectile.AddBehavior(filterModel);
        }

        filterModel.filters = (filterModel.filters ?? new Il2CppReferenceArray<FilterModel>(0))
            .AddTo(new FilterWithChanceModel("", 1f / Factor));
        projectile.UpdateCollisionPassList();

        model.AddBehavior(attack);
    }

    public override void Apply2(TowerModel model)
    {
        var ability = AbilityModel(model);

        var projectile = ability.GetDescendant<ProjectileModel>().GetDescendant<ProjectileModel>().Duplicate();

        projectile.AddFilter(new FilterWithChanceModel("", .2f));
        projectile.GetBehavior<AgeModel>().Lifespan /= 5f;
        projectile.UpdateCollisionPassList();

        var addBehavior = projectile.GetDescendant<AddBehaviorToBloonModel>();
        if (addBehavior != null)
        {
            addBehavior.lifespan /= 5;
            addBehavior.lifespanFrames /= 5;
        }

        var newAttack = model.GetAttackModel().Duplicate("PoolsOfFire");
        var newWeapon = newAttack.GetChild<WeaponModel>();
        var newProj = newWeapon.projectile;

        newWeapon.Rate *= 10;
        newWeapon.SetEmission(new SingleEmissionModel("", null));
        newWeapon.RemoveBehavior<CreateSoundOnProjectileCreatedModel>();
        newWeapon.RemoveBehavior<BonusProjectileAfterIntervalModel>();

        newProj.display = newProj.GetBehavior<DisplayModel>().display = new PrefabReference { guidRef = "" };
        newProj.pierce = 9999;
        newProj.RemoveBehavior<DamageModel>();
        newProj.RemoveBehavior<AddBehaviorToBloonModel>();

        newProj.AddBehavior(new CreateProjectileOnExhaustPierceModel("", projectile, new SingleEmissionModel("", null),
            1f, 5, 5, true, new PrefabReference { guidRef = "" }, 1, false));
        
        model.AddBehavior(newAttack);
    }
}