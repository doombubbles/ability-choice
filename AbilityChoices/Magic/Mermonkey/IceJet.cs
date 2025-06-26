using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors;
namespace AbilityChoice.AbilityChoices.Magic.Mermonkey;

public class IceJet : TowerAbilityChoice
{
    public override string AbilityName => "MermonkyAbility";
    public override string UpgradeId => UpgradeType.ArcticKnight;

    public override string Description1 =>
        "Tridents can pop Lead Bloons and grow in power faster. Also launches bouncing ice balls following target priority, or targeted point.";

    public override void Apply1(TowerModel model)
    {
        var abilityModel = AbilityModel(model);
        
        var activateAttackModel = abilityModel.GetBehavior<ActivateAttackModel>();
        var abilityAttack = activateAttackModel.attacks[0].Duplicate();
        abilityAttack.name = "AttackModel_Attack_IceBalls";
        abilityAttack.range = model.range;
        var uptime = activateAttackModel.Lifespan / abilityModel.Cooldown;

        abilityAttack.GetBehavior<CreateEffectWhileAttackingModel>().effectModel.lifespan /= 3;
        
        var abilityWeapon = abilityAttack.weapons[0];

        abilityWeapon.rate /= uptime;
        
        model.AddBehavior(abilityAttack);
    }
}