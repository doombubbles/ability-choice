﻿using Assets.Scripts.Models.Towers;
using Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Assets.Scripts.Unity;
using Assets.Scripts.Unity.Display;
using Assets.Scripts.Utils;
using BTD_Mod_Helper.Api.Display;

namespace AbilityChoice.Displays;

public class MiniMoabEliminator : ModDisplay
{
    public override PrefabReference BaseDisplayReference => Game.instance.model.GetTower(TowerType.BombShooter, 0, 5, 0)
        .GetDescendant<ActivateAttackModel>().attacks[0].weapons[0].projectile.display;

    public override float Scale => .5f;

    public override void ModifyDisplayNode(UnityDisplayNode node)
    {
    }
}