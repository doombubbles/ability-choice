using System.Collections.Generic;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using Il2Cpp;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
namespace AbilityChoice.AbilityChoices.Magic.NinjaMonkey;

public class GrandSabotage : Sabotage
{
    public override string UpgradeId => UpgradeType.GrandSaboteur;

    public override string Description1 =>
        "All Bloons move at further reduced speed. MOAB-Class Bloons spawn with slightly reduced health. Permanent Shinobi buff.";

    public override string Description2 =>
        "Ninja's attack have further increased range and pierce, and do more damage to stronger Bloon types. Permanent Shinobi buff.";

    public override void Apply1(TowerModel model)
    {
        base.Apply1(model);
        var abilityModel = AbilityModel(model);

        var uptime = abilityModel.GetDescendant<AgeModel>().Lifespan / abilityModel.Cooldown;

        model.GetDescendants<DamagePercentOfMaxModel>().ForEach(maxModel =>
        {
            var percent = CalcAvgBonus(uptime, 1 + maxModel.percent) - 1;
            maxModel.percent = percent;
        });
    }

    public override void Apply2(TowerModel model)
    {
        base.Apply2(model);
        model.IncreaseRange(10);

        var tags = new List<string>
        {
            BloonTag.Moab,
            BloonTag.Bfb,
            BloonTag.Zomg,
            BloonTag.Ddt,
            BloonTag.Bad
        };

        foreach (var weaponModel in model.GetWeapons())
        {
            for (var i = 0; i < tags.Count; i++)
            {
                if (weaponModel.projectile.GetDamageModel().IsType(out DamageModel damageModel))
                {
                    var behavior = new DamageModifierForTagModel("DamageModifierForTagModel_" + i, tags[i], 1.0f,
                        10 * (i + 1), false, false) { tags = new[] { tags[i] } };
                    weaponModel.projectile.AddBehavior(behavior);
                    weaponModel.projectile.pierce += 10;

                    damageModel.immuneBloonProperties = BloonProperties.None;
                }
            }
        }
    }

    protected override void ApplyBoth(TowerModel model)
    {
        var abilityModel = AbilityModel(model);

        var range = abilityModel.GetDescendant<ActivateRangeSupportZoneModel>();
        var permaRange = new RangeSupportModel("RangeSupportModel_", true, range.multiplier, range.additive / 2f,
            range.mutatorId, range.filters, range.isGlobal, range.buffLocsName, range.buffIconName);
        model.AddBehavior(permaRange);

        var damage = abilityModel.GetDescendant<ActivateDamageModifierSupportZoneModel>();
        var permaDamage = new DamageModifierSupportModel("DamageSupportModel_", damage.isUnique, damage.mutatorId,
            damage.filters, damage.isGlobal, damage.damageModifierModel);
        model.AddBehavior(permaDamage);
    }
}