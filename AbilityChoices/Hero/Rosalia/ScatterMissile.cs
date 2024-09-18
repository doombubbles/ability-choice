using System.Collections.Generic;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Weapons.Behaviors;

namespace AbilityChoice.AbilityChoices.Hero.Rosalia;

public class ScatterMissile : HeroAbilityChoice
{
    public override string HeroId => TowerType.Rosalia;

    private const int Factor = 5;

    public override Dictionary<int, string> Descriptions1 => new()
    {
        {3, "Rosalia's Workshop periodically launches a missile that rains destruction from above."},
        {9, "Scatter Missile releases more mini-missiles when fired."},
        {16, "More frequent Kinetic Charge and Scatter Missile."}
    };

    public override void Apply1(TowerModel model)
    {
        var abilityModel = AbilityModel(model);

        var activateAttackModel = abilityModel.GetBehavior<ActivateAttackModel>();
        var abilityAttack = activateAttackModel.attacks[0].Duplicate();
        abilityAttack.name = "AttackModel_Attack_ScatterMissile";
        abilityAttack.range = model.range;

        var abilityWeapon = abilityAttack.weapons[0];

        abilityWeapon.rate = abilityModel.Cooldown / Factor;

        abilityWeapon.GetDescendants<DamageModel>().ForEach(damageModel => damageModel.damage /= Factor);
        abilityWeapon.GetDescendants<SlowModel>().ForEach(slowModel => slowModel.Lifespan /= Factor);


        var effect = abilityModel.GetBehavior<CreateEffectOnAbilityModel>().effectModel;
        var sound = abilityModel.GetBehavior<CreateSoundOnAbilityModel>();

        abilityWeapon.AddBehavior(new EjectEffectModel("", effect.assetId, effect, effect.lifespan, effect.fullscreen,
            false, false, false, false));

        abilityWeapon.AddBehavior(new CreateSoundOnProjectileCreatedModel("", sound.sound, sound.sound, sound.sound,
            sound.sound, sound.sound, ""));

        model.AddBehavior(abilityAttack);
    }
}