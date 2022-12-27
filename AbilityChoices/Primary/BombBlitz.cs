using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;

namespace AbilityChoice.AbilityChoices.Primary;

public class BombBlitz : AbilityChoice
{
    private const float ExpectedPierce = 4;

    public override string UpgradeId => UpgradeType.BombBlitz;
    public override string Description1 => "Deals even further increased damage.";

    public override void Apply1(TowerModel model)
    {
        var abilityModel = AbilityModel(model);
        var baseWeaponRate = BaseTowerModel.GetWeapon().Rate;
        var proj = model.GetWeapon().projectile.GetBehavior<CreateProjectileOnExhaustFractionModel>().projectile;
        var damage = abilityModel.GetDescendant<AttackModel>().weapons[1].GetDescendant<DamageModel>().damage;
            
        var addedDpsIGuess = damage / abilityModel.Cooldown;

        proj.GetDamageModel().damage += addedDpsIGuess * baseWeaponRate / ExpectedPierce;
    }
}