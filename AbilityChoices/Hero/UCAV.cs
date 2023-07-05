using System.Collections.Generic;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Weapons;

namespace AbilityChoice.AbilityChoices.Hero;

public class UCAV : HeroAbilityChoice
{
    public override string HeroId => TowerType.Etienne;

    public override Dictionary<int, string> Descriptions1 => new()
    {
        { 10, "A devastating combat drone circles the map at high altitude." },
        { 13, "UCAV attack speed increased." },
        { 15, "UCAV damage output increased." },
        { 17, "UCAV damage output increased again!" },
        { 20, "UCAV becomes even more powerful and can pop all Bloon types." }
    };

    protected override void ApplyBoth(TowerModel model)
    {
        var ability = AbilityModel(model);

        var ucav = ability.GetBehavior<UCAVModel>();

        var uptime = ucav.duration / ability.Cooldown;

        var uavSpawner = model.GetDescendant<CreateTowerModel>();

        uavSpawner.RemoveChildDependant(uavSpawner.tower);
        var tower = uavSpawner.tower = ucav.ucavTowerModel;
        uavSpawner.AddChildDependant(tower);

        if (model.tier < 20)
        {
            tower.GetDescendant<WeaponModel>().Rate /= uptime;
        }
        else
        {
            var baseDamage = ucav.uavTowerModel.GetDescendant<DamageModel>();
            var towerDamage = tower.GetDescendant<DamageModel>();

            var avgDamage = baseDamage.damage * CalcAvgBonus(uptime, towerDamage.damage / baseDamage.damage);

            if (Mode2)
            {
                towerDamage.damage = avgDamage;
            }
            else
            {
                tower.GetDescendant<WeaponModel>().Rate *= towerDamage.damage / avgDamage;
            }

            model.GetBehavior<DestroyChildTowersOnUpgradeModel>().towerId = "UCAV";
        }

        if (model.tier == 10)
        {
            model.AddBehavior(new DestroyChildTowersOnUpgradeModel("", "UAV"));
        }
    }
}