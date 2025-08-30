using System.Collections.Generic;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;

namespace AbilityChoice.AbilityChoices.Hero.Silas;

public class FrozenBurial : HeroAbilityChoice
{
    public override string HeroId => TowerType.Silas;

    public override Dictionary<int, string> Descriptions1 => new()
    {
        {
            10,
            "Periodically freezes and damages all Bloons, dealing more damage to already frozen Bloons, then fills the track with mini Ice Walls."
        }
    };

    public override Dictionary<int, string> Descriptions2 => new()
    {
        {
            10,
            "Silas creates more Ice Walls, and Ice Walls now deal damage to Bloons."
        }
    };

    private const int Factor = 6;

    public override void Apply1(TowerModel model)
    {
        var ability = AbilityModel(model);
        ability.Cooldown /= Factor;

        ability.RemoveBehavior<CreateSoundOnAbilityModel>();
        ability.RemoveBehavior<CreateEffectOnAbilityModel>("CornerFX");
        ability.RemoveBehavior<CreateEffectOnAbilityModel>("Snow");

        var explosion = ability.GetBehavior<CreateProjectileOnAbilityActivateModel>().projectile;

        explosion.GetDescendants<DamageModel>().ForEach(damage => damage.damage /= Factor);
        explosion.GetDescendants<DamageModifierForBloonStateModel>().ForEach(state => state.damageAdditive /= Factor);
        explosion.GetDescendants<FreezeModel>().ForEach(freeze => freeze.Lifespan /= Factor);
        explosion.GetDescendants<AddHeatToBloonModel>().ForEach(heat => heat.heatAmount /= Factor);


        var frozenBurial = ability.GetBehavior<FrozenBurialModel>();
        frozenBurial.icewallDelay /= Factor / 2;
        frozenBurial.timePerIcewall /= Factor / 2;

        var iceWall = frozenBurial.icewallProjectile;

        iceWall.scale = .67f;
        iceWall.radius = 2f;

        iceWall.GetDescendants<AgeModel>().ForEach(age => age.Lifespan /= 2);

        iceWall.GetDescendants<DamageModel>().ForEach(damage => damage.damage /= Factor / 2f);
        iceWall.GetDescendants<FreezeModel>().ForEach(freeze => freeze.Lifespan /= Factor / 2f);
    }

    public override void Apply2(TowerModel model)
    {
        var ability = AbilityModel(model);
        var explosion = ability.GetBehavior<CreateProjectileOnAbilityActivateModel>().projectile;

        var iceWallAttack = model.GetAttackModel("Icewall");
        var iceWallWeapon = iceWallAttack.weapons[0]!;
        var iceWallProj = iceWallWeapon.projectile;

        iceWallAttack.range = model.range;
        iceWallWeapon.Rate *= ability.Cooldown / 180f;

        var damage = explosion.GetBehavior<DamageModel>().Duplicate();
        damage.damage = 2;

        iceWallProj.AddBehavior(damage);

        foreach (var collidePierce in iceWallProj.GetBehaviors<CollideExtraPierceReductionModel>())
        {
            iceWallProj.AddBehavior(new DamageModifierForTagModel("", collidePierce.bloonTag, 1,
                collidePierce.extraAmount * 2, false, false));
        }
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