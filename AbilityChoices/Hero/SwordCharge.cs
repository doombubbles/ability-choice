using System.Collections.Generic;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Weapons;

namespace AbilityChoice.AbilityChoices.Hero;

public class SwordCharge : HeroAbilityChoice
{
    public override string HeroId => TowerType.Sauda;

    public override Dictionary<int, string> Descriptions1 => new()
    {
        { 10, "Sauda periodically sends mirages of herself along the track, devastating Bloons as she goes." },
        { 20, "Sword Charge and Leaping Sword power greatly increased." }
    };

    public override Dictionary<int, string> Descriptions2 => new()
    {
        { 10, "Sauda's blades riocochet and damage bloons along the track." },
        { 20, " Leaping Sword power greatly increased." }
    };

    public const float Factor = 5;

    protected override void Apply1(TowerModel model)
    {
        var ability = AbilityModel(model);
        var swordCharge = ability.GetBehavior<SwordChargeModel>();

        ability.RemoveBehavior<CreateSoundOnAbilityModel>();
        ability.RemoveBehavior<CreateEffectOnAbilityModel>();
        swordCharge.landingSound = null;
        swordCharge.spawnSound = null;
        swordCharge.effectAtEndModel = null;

        ability.Cooldown /= Factor;

        var factor = swordCharge.iterations / Factor;

        swordCharge.GetDescendants<DamageModel>().ForEach(damage => { damage.damage *= factor; });
        swordCharge.GetDescendants<DamageModifierForTagModel>().ForEach(modifier =>
        {
            modifier.damageAddative *= factor;
        });
        swordCharge.GetDescendants<SaudaAfflictionDamageModifierModel>().ForEach(modifier =>
        {
            modifier.lv7NonMoabBonus *= factor;
            modifier.lv7MoabBonus *= factor;
            modifier.lv11NonMoabBonus *= factor;
            modifier.lv11MoabBonus *= factor;
            modifier.lv19NonMoabBonus *= factor;
            modifier.lv19MoabBonus *= factor;
        });

        swordCharge.iterations = 1;
    }

    protected override void Apply2(TowerModel model)
    {
        // TODO sword charge 2
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