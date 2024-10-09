using System.Collections.Generic;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.GeraldoItems;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Weapons;

namespace AbilityChoice.AbilityChoices.Hero.Geraldo;

public class CreepyIdol : GeraldoAbilityChioce
{
    public override Dictionary<int, string> Descriptions1 => new()
    {
        {
            18,
            "Genie gains a second close-target attack and does bonus damage to MOAB-class Bloons. Creepy Idol occasionally covers nearby MOAB-class Bloons with a Mystical Shroud, causing damage when popped."
        }
    };

    public override Dictionary<int, string> Descriptions2 => new();

    protected override int CostMult => 15;


    protected override void Apply(GeraldoItemModel geraldoItem)
    {
    }

    public override void Apply(GameModel gameModel)
    {
        base.Apply(gameModel);

        foreach (var towerModel in gameModel.GetTowersWithBaseId(TowerType.CreepyIdol))
        {
            towerModel.GetDescendant<TowerExpireModel>().rounds = -1;

            var onDestroy = towerModel.GetDescendant<CreateProjectileOnTowerDestroyModel>();

            if (onDestroy == null) continue;

            var weapon = new WeaponModel("", projectile: onDestroy.projectileModel, emission: onDestroy.emissionModel,
                fireWithoutTarget: true, rate: 30, startInCooldown: true);

            towerModel.GetAttackModel().AddWeapon(weapon);
        }
    }
}