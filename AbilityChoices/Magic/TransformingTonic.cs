﻿using Assets.Scripts.Models.GenericBehaviors;
using Assets.Scripts.Models.Towers;
using Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;

namespace AbilityChoice.AbilityChoices.Magic;

public class TransformingTonic : AbilityChoice
{
    public override string UpgradeId => UpgradeType.TransformingTonic;

    public override string Description1 => "Gains a monstrous laser beam attack.";
    
    public override string Description2 => "Gains a monstrous laser beam attack (with the looks to match it).";
        
    public override void Apply1(TowerModel model)
    {
        var ability = AbilityModel(model);
        var activateAttackModel = ability.GetBehavior<ActivateAttackModel>();
        var abilityAttack = activateAttackModel.attacks[0].Duplicate();
        var abilityWeapon = abilityAttack.weapons[0];
        var uptime = activateAttackModel.Lifespan / ability.Cooldown;

        abilityAttack.range = model.range;
        abilityWeapon.rate /= uptime;

        model.AddBehavior(abilityAttack);
            
            
        model.IncreaseRange(ability.GetBehavior<IncreaseRangeModel>().addative * uptime);
    }

    public override void Apply2(TowerModel model)
    {
        Apply1(model);
        
        var ability = AbilityModel(model);
        var display = ability.GetBehavior<SwitchDisplayModel>().display;
        model.display = model.GetBehavior<DisplayModel>().display = display;
    }
}