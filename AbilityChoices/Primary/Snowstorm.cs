using System.Linq;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Upgrades;
using Il2CppAssets.Scripts.Unity;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;

namespace AbilityChoice.AbilityChoices.Primary;

public class Snowstorm : AbilityChoice
{
    public override string UpgradeId => UpgradeType.Snowstorm;

    public override string Description1 => "Periodically freezes all Bloons on screen for 1s.";

    public override string Description2 => "Cold aura can partially slows MOAB class bloons.";

    protected virtual int Factor => 6;
    
    public override void Apply1(TowerModel model)
    {
        var ability = AbilityModel(model);

        var activateAttackModel = ability.GetBehavior<ActivateAttackModel>();
        var abilityAttack = activateAttackModel.attacks[0].Duplicate();
        var abilityWeapon = abilityAttack.weapons[0];
        abilityWeapon.Rate = ability.Cooldown / Factor;
        var proj = abilityWeapon.projectile;
        proj.RemoveBehavior<DamageModel>();
        proj.GetBehavior<FreezeModel>().Lifespan /= Factor;
        
        model.AddBehavior(abilityAttack);
    }

    public override void Apply2(TowerModel model)
    {
        var realSlow = model.GetBehavior<SlowBloonsZoneModel>();

        var totem = Game.instance.model.GetTowerFromId("NaturesWardTotem");

        var slow = totem.GetBehaviors<SlowBloonsZoneModel>().First(b => !b.name.Contains("NonMoabs")).Duplicate();
        slow.zoneRadius = realSlow.zoneRadius;
        slow.bindRadiusToTowerRange = true;
        slow.radiusOffset = realSlow.radiusOffset;

        model.AddBehavior(slow);
    }
}