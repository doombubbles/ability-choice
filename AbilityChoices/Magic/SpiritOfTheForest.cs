using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;

namespace AbilityChoice.AbilityChoices.Magic;

public class SpiritOfTheForest : JunglesBounty
{
    public override string UpgradeId => UpgradeType.SpiritOfTheForest;

    public override string Description1 =>
        "Grows thorned vines along the path that deal constant damage and bonus damage to ceramics. Vines nearest the Spirit of the Forest do more damage. Periodically gives lives as well as more cash.";

    public override string Description2 =>
        "Grows thorned vines along the path that deal constant damage and bonus damage to ceramics. Vines nearest the Spirit of the Forest do more damage. Income bonus increased to 30%";
    
    protected override float Income => 1.3f;

    protected override int Order => -1;

    protected override void ApplyBoth(TowerModel model)
    {
        var vineRupture = model.GetBehaviors<AbilityModel>()
            .FirstOrDefault(ability => ability.displayName == "Vine Rupture");
        
        if (vineRupture == null) return;

        var damage = vineRupture.GetBehavior<VineRuptureModel>().projectileModel.GetDamageModel().damage;
        var cooldown = vineRupture.Cooldown;
        
        var thorns = model.GetBehavior<SpiritOfTheForestModel>();

        var customDot = thorns.damageOverTimeZoneModelFar.behaviorModel;
        
        customDot.damage += customDot.Interval * damage / cooldown;

        model.RemoveBehavior(vineRupture);
    }
}