using System.Collections.Generic;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using Il2CppAssets.Scripts.Models.Towers.Filters;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
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

    /*public override Dictionary<int, string> Descriptions2 => new()
    {
        {
            3, ""
        },
        {
            7, "Heat it up has increased radius, "
        },
        {
            14, ""
        }
    };*/

    protected override void Apply1(TowerModel model)
    {
        var ability = AbilityModel(model);

        var attack = ability.GetDescendant<AttackModel>().Duplicate();
        var weapon = attack.weapons[0];
        weapon.projectile.RemoveBehavior<CreateSoundOnProjectileExhaustModel>();
        var projectile = weapon.projectile.GetDescendant<ProjectileModel>();
        var ageModel = projectile.GetBehavior<AgeModel>();

        var uptime = ageModel.Lifespan / ability.Cooldown;

        ageModel.Lifespan /= 2f;

        weapon.Rate = ageModel.Lifespan / uptime;


        weapon.Rate /= 2;
        var filterModel = projectile.GetBehavior<ProjectileFilterModel>();
        if (filterModel == null)
        {
            filterModel = new ProjectileFilterModel("", null);
            projectile.AddBehavior(filterModel);
        }

        filterModel.filters = (filterModel.filters ?? new Il2CppReferenceArray<FilterModel>(0))
            .AddTo(new FilterWithChanceModel("", 1 / 2f));
        projectile.UpdateCollisionPassList();

        model.AddBehavior(attack);
    }

    protected override void Apply2(TowerModel model)
    {
        // TODO cocktail of fire 2
    }
}