using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions;
using Il2CppAssets.Scripts.Models.Towers.Weapons.Behaviors;

namespace AbilityChoice.AbilityChoices.Magic.Druid;

public class VineCrush : TowerAbilityChoice
{
    public override string UpgradeId => UpgradeType.JunglesBounty;

    public override string Description1 =>
        "Vines create stronger thorn piles. Druid generates cash and lives at the end of each round, plus extra cash per Banana Farm near the Druid. Periodically crushes multiple Bloons at once.";

    public virtual int Factor => 5;

    public override void Apply1(TowerModel model)
    {
        var ability = AbilityModel(model);

        var attack = ability.GetDescendant<AttackModel>();

        attack.weapons[0]!.Rate = ability.Cooldown / Factor;
        attack.GetDescendant<MultiInstantEmissionModel>().amount /= Factor;
        attack.GetDescendant<JungleVineLimitProjectileModel>().limit /= Factor;

        model.AddBehavior(attack);
    }
}