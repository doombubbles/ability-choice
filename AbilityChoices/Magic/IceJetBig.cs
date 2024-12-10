using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Weapons.Behaviors;

namespace AbilityChoice.AbilityChoices.Magic;

public class IceJetBig : IceJet
{
    public override string AbilityName => "MermonkyAbilityBig";
    public override string UpgradeId => UpgradeType.Popseidon;

    private const int Factor = 3;

    public override string Description1 =>
        "Popseidon draws such power that a rogue wave periodically floods the map with supercooled water, freezing all Bloons and reclaiming the weak for the sea.";

    public override void Apply1(TowerModel model)
    {
        base.Apply1(model);

        var abilityModel = AbilityModel(model);

        var abilityAttack = model.GetAttackModel("IceBalls");

        var global = abilityAttack.weapons[1];
        global.Rate = abilityModel.Cooldown / Factor;
        global.GetDescendant<DamageModel>().damage /= Factor;
        global.GetDescendant<SlowModel>().Lifespan /= Factor;

        var effect = abilityModel.GetBehavior<CreateEffectOnAbilityModel>().effectModel;
        var sound = abilityModel.GetBehavior<CreateSoundOnAbilityModel>();

        effect.useCenterPosition = true;

        global.AddBehavior(new EjectEffectModel("", effect, effect.lifespan, effect.fullscreen, false,
            false, false, false));

        global.AddBehavior(new CreateSoundOnProjectileCreatedModel("", sound.sound, sound.sound, sound.sound,
            sound.sound, sound.sound, ""));
    }
}