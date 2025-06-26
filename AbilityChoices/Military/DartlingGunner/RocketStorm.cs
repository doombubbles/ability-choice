using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Weapons.Behaviors;
namespace AbilityChoice.AbilityChoices.Military.DartlingGunner;

public class RocketStorm : TowerAbilityChoice
{
    public override string AbilityName => Name; // There's no space in RocketStorm in the ability model /shrug

    public override string UpgradeId => UpgradeType.RocketStorm;

    public override string Description1 => "Occasionally shoots a wave of Rocket Storm missiles.";

    public override string Description2 =>
        "Shoots a single stream of Rocket Storm missiles with the same accuracy of its main attack.";

    public override void Apply1(TowerModel model)
    {
        var abilityModel = AbilityModel(model);
        var activateAttackModel = abilityModel.GetBehavior<ActivateAttackModel>();
        var abilityAttack = activateAttackModel.attacks[0].Duplicate();
        var uptime = activateAttackModel.Lifespan / abilityModel.Cooldown;

        var abilityWeapon = abilityAttack.weapons[0];

        abilityWeapon.rate /= uptime;


        model.GetAttackModels()[0].AddWeapon(abilityWeapon);
    }

    public override void Apply2(TowerModel model)
    {
        var abilityModel = AbilityModel(model);
        var activateAttackModel = abilityModel.GetBehavior<ActivateAttackModel>();
        var abilityAttack = activateAttackModel.attacks[0].Duplicate();
        var uptime = activateAttackModel.Lifespan / abilityModel.Cooldown;

        var abilityWeapon = abilityAttack.weapons[0];

        var realWeapon = model.GetWeapon();
        var count = 1;
        if (abilityWeapon.emission.IsType(out RandomEmissionModel randomEmissionModel))
        {
            count = randomEmissionModel.count;
        }
        else if (abilityWeapon.emission.IsType(out EmissionWithOffsetsModel emissionWithOffsetsModel))
        {
            count = emissionWithOffsetsModel.projectileCount;
        }
        else
        {
            MelonLogger.Msg("Couldn't find count ?");
        }

        abilityWeapon.emission = realWeapon.emission;

        abilityWeapon.Rate /= uptime * count / 2;

        abilityWeapon.GetDescendants<SlowModel>().ForEach(slowModel => slowModel.dontRefreshDuration = true);

        abilityWeapon.GetBehavior<EjectEffectModel>().effectModel.lifespan *= uptime;

        model.GetAttackModels()[0].AddWeapon(abilityWeapon);
    }
}