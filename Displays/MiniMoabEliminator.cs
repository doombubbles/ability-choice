using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.Display;
using Il2CppAssets.Scripts.Utils;
using BTD_Mod_Helper.Api.Display;

namespace AbilityChoice.Displays;

public class MiniMoabEliminator : ModDisplay
{
    public override PrefabReference BaseDisplayReference => Game.instance.model.GetTower(TowerType.BombShooter, 0, 5, 0)
        .GetDescendant<ActivateAttackModel>().attacks[0].weapons[0].projectile.display;

    public override float Scale => .5f;
}