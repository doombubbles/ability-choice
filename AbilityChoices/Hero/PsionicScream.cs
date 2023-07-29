using System.Collections.Generic;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Effects;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using Il2CppAssets.Scripts.Models.Towers.Filters;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Weapons;
using Il2CppAssets.Scripts.Models.Towers.Weapons.Behaviors;

namespace AbilityChoice.AbilityChoices.Hero;

public class PsionicScream : HeroAbilityChoice
{
    public override string HeroId => TowerType.Psi;

    public override Dictionary<int, string> Descriptions1 => new()
    {
        { 10, "Psi unleashes silent screams that throw some Bloons into utter chaos." },
        { 20, "Psionic Screams hold and damage mores Bloons on screen. Psi can now target DDTs and ZOMGs." }
    };

    private const float Factor = 6;

    protected override void Apply1(TowerModel model)
    {
        var ability = AbilityModel(model);

        var activate = ability.GetBehavior<ActivateAttackModel>();

        var attack = activate.GetChild<AttackModel>();
        attack.fireWithoutTarget = false;

        var weapon = attack.GetChild<WeaponModel>();
        weapon.Rate = ability.Cooldown / Factor;

        var effect = ability.GetBehaviors<CreateEffectOnAbilityModel>()
            .Select(e => e.effectModel)
            .First(e => e.fullscreen == Fullscreen.No);
        weapon.AddBehavior(new EjectEffectModel("", effect.assetId, effect, effect.lifespan, effect.fullscreen, false,
            false, false, false));

        var projectile = weapon.projectile;

        projectile.pierce /= Factor;

        projectile.GetBehavior<ProjectileFilterModel>().filters = projectile.filters =
            projectile.filters.AddTo(new FilterWithChanceModel("", 1 / Factor));

        model.AddBehavior(attack);
    }

    protected override void Apply2(TowerModel model)
    {
        // TODO psionic scream 2
    }
}