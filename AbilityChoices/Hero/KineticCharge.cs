using System.Collections.Generic;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Weapons.Behaviors;

namespace AbilityChoice.AbilityChoices.Hero;

public class KineticCharge : HeroAbilityChoice
{
    public override string HeroId => TowerType.Rosalia;

    public override Dictionary<int, string> Descriptions1 => new()
    {
        {10, "Workshop emits Kinetic Charges that attach to Bloons, harnessing energy when the target takes damage. They then release the accumulated energy in a huge detonation."},
        {16, "More frequent Kinetic Charge and Scatter Missile."}
    };

    private const int Factor = 5;
    
    public override void Apply1(TowerModel model)
    {
        var abilityModel = AbilityModel(model);

        var activateAttackModel = abilityModel.GetBehavior<ActivateAttackModel>();
        var abilityAttack = activateAttackModel.attacks[0].Duplicate();
        abilityAttack.name = "AttackModel_Attack_ScatterMissile";
        abilityAttack.range = model.range;

        var abilityWeapon = abilityAttack.weapons[0];

        abilityWeapon.rate = abilityModel.Cooldown / Factor;

        abilityWeapon.GetDescendants<KineticChargeProjectileModel>().ForEach(kc =>
        {
            kc.duration /= Factor;
            kc.durationFrames /= Factor;
            kc.maxAdditionalDamageAmount /= Factor;
            kc.damageTakenMaxAmount /= Factor;
        });
        abilityWeapon.GetDescendants<DamageModel>().ForEach(damageModel => damageModel.damage /= Factor);


        var effect = abilityModel.GetBehavior<CreateEffectOnAbilityModel>().effectModel;
        var sound = abilityModel.GetBehavior<CreateSoundOnAbilityModel>();

        abilityWeapon.AddBehavior(new EjectEffectModel("", effect.assetId, effect, effect.lifespan, effect.fullscreen,
            false, false, false, false));

        abilityWeapon.AddBehavior(new CreateSoundOnProjectileCreatedModel("", sound.sound, sound.sound, sound.sound,
            sound.sound, sound.sound, ""));

        model.AddBehavior(abilityAttack);
    }
}