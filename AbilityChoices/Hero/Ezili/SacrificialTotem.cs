using System.Collections.Generic;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using Il2CppAssets.Scripts.Models.Towers.Weapons;

namespace AbilityChoice.AbilityChoices.Hero.Ezili;

public class SacrificialTotem : HeroAbilityChoice
{
    public override string HeroId => TowerType.Ezili;

    public override Dictionary<int, string> Descriptions1 => new()
    {
        { 7, "Summons a totem that grants camo detection and pierce to nearby Monkeys, especially Wizard Monkeys." },
        { 16, "Totem now has increased uptime." }
    };

    public override Dictionary<int, string> Descriptions2 => new()
    {
        { 7, "Summons a totem that grants attack speed to nearby Monkeys, especially Wizard Monkeys." },
        { 16, "Totem now has increased uptime" }
    };

    protected override void ApplyBoth(TowerModel model)
    {
        var ability = AbilityModel(model);

        var attack = ability.GetDescendant<AttackModel>().Duplicate();

        var weapon = attack.GetChild<WeaponModel>();
        weapon.Rate = ability.Cooldown / 3;

        var totem = weapon.GetDescendant<TowerModel>();

        totem.RemoveBehaviors<RangeSupportModel>();
        if (Mode2)
        {
            totem.RemoveBehaviors<RateSupportModel>();
            totem.RemoveBehaviors<ProjectileSpeedSupportModel>();
        }
        else
        {
            totem.RemoveBehaviors<PierceSupportModel>();
            totem.RemoveBehaviors<VisibilitySupportModel>();
        }

        totem.GetBehavior<TowerExpireModel>().Lifespan /= 3;

        model.AddBehavior(attack);
    }
}