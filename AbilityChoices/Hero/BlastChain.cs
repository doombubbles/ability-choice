using System.Collections.Generic;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Weapons;

namespace AbilityChoice.AbilityChoices.Hero;

public class BlastChain : HeroAbilityChoice
{
    public override string HeroId => TowerType.AdmiralBrickell;

    public override string AbilityName => "Blast Chain Ability";

    public override Dictionary<int, string> Descriptions1 => new()
    {
        { 7, "Sea mines fire more frequently. Brickell gains increased attack range and Camo Bloon detection." }
    };

    public override Dictionary<int, string> Descriptions2 => new()
    {
        { 7, "Sea mines last longer. Brickell gains increased attack range and Camo Bloon detection." }
    };


    protected override void Apply1(TowerModel model)
    {
        var ability = AbilityModel(model);
        var buff = ability.GetDescendant<BrickellFreezeMinesAbilityBuffModel>();

        var bonus = CalcAvgBonus(buff.lifespan / ability.Cooldown, 1 / buff.multiplier);

        model.GetAttackModel("SeaMine").GetDescendant<WeaponModel>().Rate /= bonus;
    }

    protected override void Apply2(TowerModel model)
    {
        var ability = AbilityModel(model);
        var buff = ability.GetDescendant<BrickellFreezeMinesAbilityBuffModel>();

        var bonus = CalcAvgBonus(buff.lifespan / ability.Cooldown, buff.projectileSpeedMultiplier);

        model.GetAttackModel("SeaMine").GetDescendants<AgeModel>().ForEach(ageModel =>
        {
            if (ageModel.useRoundTime)
            {
                ageModel.Lifespan *= bonus;
            }
        });
    }
}