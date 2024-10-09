using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using Il2Cpp;
using Il2CppAssets.Scripts.Models.GenericBehaviors;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using Il2CppAssets.Scripts.Unity;

namespace AbilityChoice.AbilityChoices.Primary;

public class PlasmaMonkeyFanClub : SuperMonkeyFanClub
{
    public override string UpgradeId => UpgradeType.PlasmaMonkeyFanClub;

    public override string Description1 =>
        "Up to 5 nearby Dart Monkeys including itself are permanently Plasma Monkey Fans.";

    public override string Description2 => "Permanently shoots powerful plasma blasts itself.";

    protected override float SuperMonkeyAttackSpeed => .03f;

    public override void Apply2(TowerModel model)
    {
        base.Apply2(model);

        if (Mode2)
        {
            var plasmaModel = Game.instance.model.GetTower(TowerType.SuperMonkey, 2).GetWeapon().projectile;

            foreach (var weaponProjectile in model.GetDescendants<ProjectileModel>().ToList()
                         .Where(weaponProjectile => !string.IsNullOrEmpty(weaponProjectile.display.AssetGUID)))
            {
                weaponProjectile.display = plasmaModel.display;
                weaponProjectile.GetBehavior<DisplayModel>().display = plasmaModel.display;
                weaponProjectile.GetDamageModel().damage += 2;
                weaponProjectile.GetDamageModel().immuneBloonProperties = BloonProperties.Purple;
                weaponProjectile.GetDamageModel().immuneBloonPropertiesOriginal = BloonProperties.Purple;
                weaponProjectile.pierce += 5;
            }
        }
    }
}