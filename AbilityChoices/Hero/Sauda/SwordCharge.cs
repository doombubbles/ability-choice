using System.Collections.Generic;
using BTD_Mod_Helper.Api.Helpers;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;

namespace AbilityChoice.AbilityChoices.Hero.Sauda;

public class SwordCharge : HeroAbilityChoice
{
    public override string HeroId => TowerType.Sauda;

    public override Dictionary<int, string> Descriptions1 => new()
    {
        {10, "Sauda sends mirages of herself along the track, devastating Bloons as she goes."},
        {14, "Sauda attacks even faster, and Sword Charge mirages are sent twice as often!"},
        {20, "Sword Charge and Leaping Sword power greatly increased."}
    };

    public override Dictionary<int, string> Descriptions2 => new()
    {
        {10, "Sauda sends mirages of herself forward towards Bloons."},
        {14, "Sauda attacks even faster, and Sword Charge mirages are sent twice as often!"},
        {20, "Sword Charge and Leaping Sword power greatly increased."}
    };

    public const float Factor = 5;
    public override void Apply1(TowerModel model)
    {
        var ability = AbilityModel(model);
        var swordCharge = ability.GetBehavior<SwordChargeModel>();

        ability.RemoveBehavior<CreateSoundOnAbilityModel>();
        ability.RemoveBehavior<CreateEffectOnAbilityModel>();
        swordCharge.landingSound = null;
        swordCharge.spawnSound = null;
        swordCharge.effectAtEndModel = null;

        ability.Cooldown /= Factor;

        var factor = swordCharge.iterations / Factor;

        swordCharge.GetDescendants<DamageModel>().ForEach(damage => damage.damage *= factor);
        swordCharge.GetDescendants<DamageModifierForTagModel>().ForEach(modifier => modifier.damageAddative *= factor);
        swordCharge.GetDescendants<SaudaAfflictionDamageModifierModel>().ForEach(modifier =>
        {
            modifier.lv7NonMoabBonus *= factor;
            modifier.lv7MoabBonus *= factor;
            modifier.lv11NonMoabBonus *= factor;
            modifier.lv11MoabBonus *= factor;
            modifier.lv19NonMoabBonus *= factor;
            modifier.lv19MoabBonus *= factor;
        });

        swordCharge.iterations = 1;
    }
    public override void Apply2(TowerModel model)
    {
        var ability = AbilityModel(model);
        var swordCharge = ability.GetBehavior<SwordChargeModel>();

        var proj = swordCharge.projectileModel;

        proj.AddBehavior(new TravelStraitModel("", proj.GetBehavior<TravelAlongPathModel>().speedFrames * 60, 1f));
        proj.AddBehavior(new TrackTargetModel("", 999, true, false, 360, false, 1000, false, true));

        proj.RemoveBehavior<AgeModel>();
        proj.RemoveBehavior<TravelAlongPathModel>();

        swordCharge.GetDescendants<DamageModel>().ForEach(damage => damage.damage /= Factor);
        swordCharge.GetDescendants<DamageModifierForTagModel>().ForEach(modifier => modifier.damageAddative /= Factor);
        swordCharge.GetDescendants<SaudaAfflictionDamageModifierModel>().ForEach(modifier =>
        {
            modifier.lv7NonMoabBonus /= Factor;
            modifier.lv7MoabBonus /= Factor;
            modifier.lv11NonMoabBonus /= Factor;
            modifier.lv11MoabBonus /= Factor;
            modifier.lv19NonMoabBonus /= Factor;
            modifier.lv19MoabBonus /= Factor;
        });

        var rate = ability.Cooldown / Factor / swordCharge.iterations;

        model.AddBehavior(new AttackHelper(Name)
        {
            Range = 9999,
            AddToSharedGrid = false,
            CanSeeCamo = true,
            Behaviors = new Model[]
            {
                new RotateToTargetModel("", true, false, false, 0, true, false)
            },
            Weapon = new WeaponHelper
            {
                Animation = -1,
                Rate = rate,
                Projectile = proj
            },
        });

        /*model.AddBehavior(new AttackModel(Name, new[]
        {
            new WeaponModel("", -1, rate, proj, emission: new SingleEmissionModel("", null))
        }, 9999, new Model[]
        {
            new AttackFilterModel("", new FilterModel[]
            {
                new FilterInvisibleModel("", false, false)
            }),
            new RotateToTargetModel("", true, false, false, 0, true, false)
        }, null, 0, 0, 0, true, false, 0, false, 0));*/
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